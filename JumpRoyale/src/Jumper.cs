using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Godot;

public partial class Jumper : CharacterBody2D
{
    private const string SpriteNodeName = "Sprite";
    private const string NameNodeName = "Name";
    private const string ParticleSystemNodeName = "Glow";
    private const float NameFadeoutTime = 5000f;

    /// <summary>
    /// Stores this jumper's position in the last frame. See <see cref="StorePosition"/>.
    /// </summary>
    private readonly HashSet<Vector2> _recentPosition = [];

    private AnimatedSprite2D _animatedSprite2D = null!;

    /// <summary>
    /// Used to block the fadeout in some situations, e.g. at the start of the game. This is automatically set to true
    /// on every jump.
    /// </summary>
    private bool _canFadePlayerName;
    private bool _lastJumpZeroAngle;
    private bool _wasOnFloor;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    private float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    /// <summary>
    /// Indicated for how many frames this jumper has been in the same place under certain conditions. See <see
    /// cref="StorePosition"/>.
    /// </summary>
    private int _framesSincePositionChange;

    /// <summary>
    /// Previous X Velocity value of this jumper before MoveAndSlide was called. Used to bounce the jumper off the wall.
    /// </summary>
    private float _previousXVelocity;

    private Vector2 _jumpVelocity;

    /// <summary>
    /// Stores the game time as timestamp for the font fadeout timer.
    /// </summary>
    private ulong _fontVisibilityTimerStartTime;

    // This property is externally set by Init and there is no constructor on this class, so it can not be initialized
    // inside here. It will never become null unless some external method does a really bad job parsing the player
    // data from json
    [AllowNull]
    public PlayerData PlayerData { get; private set; }

    public void Init(int x, int y, [NotNull] PlayerData playerData)
    {
        PlayerData = playerData;

        Position = new Vector2(x, y);
        Name = PlayerData.Name;

        SetCharacter();
        SetPlayerName();
        SetGlow();
    }

    /// <summary>
    /// Sets the text value on this jumper's label to the current twitch username. This will also change the name color
    /// if the twitch chatter was privileged (subscribed/mod/vip/streamer).
    /// </summary>
    public void SetPlayerName()
    {
        // Note: ToHTML() excludes alpha component to avoid transparent names
        string colorCode = Color.FromString(PlayerData.PlayerNameColor, GameConstants.DefaultNameColor).ToHtml(false);

        RichTextLabel nameLabel = GetNode<RichTextLabel>(NameNodeName);

        nameLabel.Text = $"[center][color={colorCode}]{PlayerData.Name}[/color][/center]";
    }

    public void SetCrazyParticles()
    {
        CpuParticles2D particles = GetGlowNode();

        // Make sure we can repeatedly call this function without unbounded growth.
        particles.Amount = Math.Min(particles.Amount * 5, 500);
    }

    public void SetCharacter()
    {
        AnimatedSprite2D sprite = GetNode<AnimatedSprite2D>(SpriteNodeName);
        int choice = PlayerData.CharacterChoice;
        string gender = choice > 9 ? "f" : "m";
        int charNumber = ((choice - 1) % 9 / 3) + 1;
        int clothingNumber = ((choice - 1) % 3) + 1;

        GD.Print("Choice: " + choice + " Gender: " + gender + " Char: " + charNumber + " Clothing: " + clothingNumber);

        sprite.SpriteFrames = SpriteFrameCreator.Instance.GetSpriteFrames(gender, charNumber, clothingNumber);

        if (IsOnFloor())
        {
            sprite.Play(JumperAnimations.AnimationIdle);
        }
    }

    /// <summary>
    /// Enables the Glow (particles) on this Jumper, if privileged (automatically disables glow otherwise).
    /// </summary>
    public void SetGlow()
    {
        string colorString = PlayerData.GlowColor;

        if (!PlayerData.IsPrivileged)
        {
            DisableGlow();
            return;
        }

        CpuParticles2D particles = GetGlowNode();
        Color color = Color.FromHtml(colorString);
        color.A = 1f;

        particles.SelfModulate = color;
        particles.Visible = true;
    }

    public void DisableGlow()
    {
        CpuParticles2D particles = GetGlowNode();

        particles.Visible = false;
    }

    public override void _Ready()
    {
        _animatedSprite2D = GetNode<AnimatedSprite2D>(SpriteNodeName);

        _animatedSprite2D.AnimationFinished += OnSpriteAnimationFinished;
    }

    public void RandomJump()
    {
        Jump(Rng.IntRange(45, 135), Rng.IntRange(10, 100));
    }

    public void Jump(int angle, int power)
    {
        if (IsOnFloor())
        {
            double normalizedPower = Math.Sqrt(power * 5 * _gravity);

            _jumpVelocity.X = Mathf.Cos(Mathf.DegToRad(angle + 180));
            _jumpVelocity.Y = Mathf.Sin(Mathf.DegToRad(angle + 180));
            _jumpVelocity = _jumpVelocity.Normalized() * (float)normalizedPower;

            PlayerData.NumJumps++;

            _canFadePlayerName = true;
            _lastJumpZeroAngle = angle == 90; // 0 in the command is expressed here as 90.
        }
    }

    /// <summary>
    /// Stops the name fadeout until the player jumps again.
    /// </summary>
    public void DisableNameFadeout()
    {
        _canFadePlayerName = false;
    }

    public override void _PhysicsProcess(double delta)
    {
        StopOnFloor();
        ApplyInitialGravity(delta);
        ApplyJumpVelocity();
        RotateInAir(delta);
        BounceOffWall();
        PlayNotGroundedAnimation();
        UpdateNameTransparency();

        _wasOnFloor = IsOnFloor();
        _previousXVelocity = Velocity.X;

        MoveAndSlide();
        StorePosition();
    }

    public void OnSpriteAnimationFinished()
    {
        AnimatedSprite2D sprite = GetNode<AnimatedSprite2D>(SpriteNodeName);

        if (sprite.Animation == JumperAnimations.AnimationLand)
        {
            sprite.Play(JumperAnimations.AnimationIdle);
        }
    }

    /// <summary>
    /// Resets the alpha component to 1 on player's name label and resets the name fadeout timer.
    /// </summary>
    public void FlashPlayerName()
    {
        SetNameAlpha(1f);
        ResetNameTimer();
    }

    private void ApplyJumpVelocity()
    {
        Velocity = !_jumpVelocity.IsEqualApprox(Vector2.Zero) ? _jumpVelocity : Velocity;

        // Flip the sprite based on our x velocity, but only if we recently jumped at a non-zero angle
        if (!Mathf.IsZeroApprox(Velocity.X) && !_lastJumpZeroAngle)
        {
            _animatedSprite2D.FlipH = Velocity.X < 0;
        }

        // Reset the jump velocity to indicate that we should not continuously apply the calculated velocity
        _jumpVelocity = Vector2.Zero;
    }

    private void ApplyInitialGravity(double delta)
    {
        if (!IsOnFloor())
        {
            Velocity = new(Velocity.X, Velocity.Y + _gravity * (float)delta);
        }
    }

    private void StopOnFloor()
    {
        if (IsOnFloor())
        {
            Velocity = Vector2.Zero;
        }
    }

    private void ResetNameTimer()
    {
        _fontVisibilityTimerStartTime = Time.GetTicksMsec();

        GetNode<RichTextLabel>(NameNodeName).Visible = true;
    }

    private void UpdateNameTransparency()
    {
        float alpha = _canFadePlayerName ? CalculateFontAlpha() : 1f;

        SetNameAlpha(alpha);
    }

    /// <summary>
    /// Calculates the <c>alpha</c> color component based on the time difference of timestamps (<c>now</c> and <c>the
    /// last time it was requested</c>).
    /// </summary>
    /// <returns>Alpha color from 0 to 1.</returns>
    private float CalculateFontAlpha()
    {
        ulong now = Time.GetTicksMsec();
        ulong diffMs = now - _fontVisibilityTimerStartTime;
        float diff = diffMs / NameFadeoutTime;

        return Math.Max(0, 1 - diff);
    }

    private CpuParticles2D GetGlowNode()
    {
        return GetNode<CpuParticles2D>(ParticleSystemNodeName);
    }

    private void SetNameAlpha(float alpha)
    {
        GetNode<RichTextLabel>(NameNodeName).Modulate = new Color(1, 1, 1, alpha);
    }

    private void BounceOffWall()
    {
        if (IsOnWall())
        {
            Velocity = new(_previousXVelocity * -0.75f, Velocity.Y);
        }
    }

    /// <summary>
    /// Attempts to store the recent position after MoveAndSlide() call. If the position was not added to the HashSet,
    /// it means that the position already existed and the jumper did not change his position, but this logic will only
    /// be executed if the jumper was constantly touching the wall, was not grounded and was not touching the ceiling.
    /// All this means that the jumper was stuck inside of a wall and his position has to be adjusted after x frames.
    /// </summary>
    private void StorePosition()
    {
        if (IsOnFloor() || !IsOnWall() || IsOnCeiling())
        {
            return;
        }

        bool added = _recentPosition.Add(Position);

        // If the position was different (the result of successful addition to the HashSet), any previous positions will
        // be cleared and the new position is stored. On a duplicate position, it could indicate the jumper got stuck.
        if (added)
        {
            _framesSincePositionChange = 0;

            // Remove the previous position and re-add the current
            _recentPosition.Clear();
            _recentPosition.Add(Position);
        }

        _framesSincePositionChange++;

        if (_framesSincePositionChange >= 60)
        {
            Position += Vector2.Up * 16;
        }
    }

    private void PlayNotGroundedAnimation()
    {
        // Going up -> Jump, down -> Fall. Removing the check causes infinite animation start
        if (!IsOnFloor())
        {
            string animation = Velocity.Y < 0 ? JumperAnimations.AnimationJump : JumperAnimations.AnimationFall;

            _animatedSprite2D.Play(animation);
        }

        // Describes a situation when we stopped, but the animation is still playing the Jump/Fall frames
        bool hasLandedButStillAnimating =
            (
                _animatedSprite2D.Animation == JumperAnimations.AnimationFall
                || _animatedSprite2D.Animation == JumperAnimations.AnimationJump
            ) && Velocity.IsEqualApprox(Vector2.Zero);

        if (hasLandedButStillAnimating)
        {
            _animatedSprite2D.Play(JumperAnimations.AnimationLand);
        }
    }

    /// <summary>
    /// Causes the character to rotate based on its X velocity, but only at non-zero angles.
    /// </summary>
    private void RotateInAir(double delta)
    {
        // We don't want to rotate if we just jumped straight up
        if (_lastJumpZeroAngle)
        {
            // Edge case reset when we jump at the very moment we hit the floor and we keep the previous rotation
            _animatedSprite2D.RotationDegrees = 0;

            return;
        }

        // Formula to automatically calculate the rotation speed based on the character's x jump velocity. The maximum
        // velocity is 700, but it's a bit too fast, so we want to clamp it at around 600. The formula was shortened and
        // tweaked to always output 1 at non-zero angle with low value, with a maximum of 7 at angle of 60, which should
        // not increase linearly, but a bit slower at the start. Assuming the full power jump.
        // Visualization, caps at J60. Approximately every +100 velocity on the plot is the next 10 angles:
        // https://www.wolframalpha.com/input?i=min%28max%28%284sin%28%28abs%28x%29%2F280%29+%2B+300%29%2B5%29%2C1%29%2C+7%29+%3Bx+from+0+to+700
        float velocity = Math.Abs(Velocity.X);
        float rotationSpeedMultiplier = (float)(4 * Math.Sin((velocity / 280) + 300) + 5);
        float clampedMultiplier = Math.Clamp(rotationSpeedMultiplier, 1, 7);

        // Calculate the rotation factor and flip the value if we are going left (rotating in the right direction)
        float rotationFactor = 200 * (float)delta * (Velocity.X < 0 ? -1 : 1) * clampedMultiplier;

        _animatedSprite2D.RotationDegrees = IsOnFloor() ? 0 : _animatedSprite2D.RotationDegrees + rotationFactor;
    }
}
