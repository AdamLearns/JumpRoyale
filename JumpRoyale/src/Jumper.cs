using System;
using System.Diagnostics.CodeAnalysis;
using Godot;

public partial class Jumper : CharacterBody2D
{
    public const string DefaultColorName = "white";

    public static readonly Color DefaultPlayerNameColor = Colors.White;

    private const string SpriteNodeName = "Sprite";
    private const string NameNodeName = "Name";
    private const string ParticleSystemNodeName = "Glow";
    private const float NameFadeoutTime = 5000f;

    private bool _wasOnFloor;
    private bool _lastJumpZeroAngle;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    private float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    private Vector2 _jumpVelocity;

    /// <summary>
    /// Stores the game time as timestamp for the font fadeout timer.
    /// </summary>
    private ulong _fontVisibilityTimerStartTime;

    private bool _hasJumped;

    // This property is externally set by Init and there is no constructor on this class, so it can not be initialized
    // inside here. It will never become null unless some external method does a really bad job parsing the player
    // data from json
    [AllowNull]
    public PlayerData PlayerData { get; private set; }

    public void Init(int x, int y, string userName, [NotNull] PlayerData playerData)
    {
        Position = new Vector2(x, y);
        Name = userName;
        PlayerData = playerData;

        // Initialize player name with default color. This will use the color from PlayerData, if set
        SetPlayerName();

        if (playerData.GlowColor != null)
        {
            SetGlow(playerData.GlowColor);
        }
    }

    public void SetPlayerName(string? newColor = null)
    {
        // If no color was specified use Default instead
        string colorOverride = newColor ?? DefaultColorName;

        // If JSON data was incomplete, e.g. first run or property was just null (new player), use the above override
        string colorName = PlayerData.NameColor ?? colorOverride;

        // Note: ToHTML() excludes alpha component to avoid transparent names
        string colorCode = Color.FromString(colorName, DefaultPlayerNameColor).ToHtml(false);

        GetNode<RichTextLabel>(NameNodeName).Text = "[center]";
        GetNode<RichTextLabel>(NameNodeName).Text += $"[color={colorCode}]";
        GetNode<RichTextLabel>(NameNodeName).Text += PlayerData.Name;
        GetNode<RichTextLabel>(NameNodeName).Text += "[/color]";
        GetNode<RichTextLabel>(NameNodeName).Text += "[/center]";
    }

    public void SetCrazyParticles()
    {
        CpuParticles2D particles = GetGlowNode();

        // Make sure we can repeatedly call this function without unbounded growth.
        particles.Amount = Math.Min(particles.Amount * 5, 500);
    }

    public void SetCharacter(int choice)
    {
        PlayerData.CharacterChoice = choice;

        AnimatedSprite2D sprite = GetNode<AnimatedSprite2D>(SpriteNodeName);
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

    public void SetGlow(string colorString)
    {
        try
        {
            CpuParticles2D particles = GetGlowNode();
            Color color = Color.FromHtml(colorString);

            color = new Color(color.R, color.G, color.B, 1f);
            particles.SelfModulate = color;
            particles.Visible = true;
            PlayerData.GlowColor = color.ToHtml(false);
        }
        catch (Exception e)
        {
            GD.Print($"Failed to set glow color to {colorString}", e);
            throw;
        }
    }

    public void DisableGlow()
    {
        CpuParticles2D particles = GetGlowNode();

        particles.Visible = false;
    }

    public override void _Ready()
    {
        GetNode<AnimatedSprite2D>(SpriteNodeName).AnimationFinished += OnSpriteAnimationFinished;
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
            SetNameAlpha(1f);

            _hasJumped = true;
            _lastJumpZeroAngle = angle == 90; // 0 in the command is expressed here as 90.

            ResetNameTimer();
        }
    }

    public void PlayerWon()
    {
        // Reset their _hasJumped property so that their name will be visible until they jump again.
        _hasJumped = false;
    }

    public void SetColor(string hexColor)
    {
        AnimatedSprite2D sprite = GetNode<AnimatedSprite2D>(SpriteNodeName);

        sprite.Modulate = Color.FromHtml(hexColor);
        sprite.Modulate = new Color(sprite.Modulate.R, sprite.Modulate.G, sprite.Modulate.B, 1f);
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        if (IsOnFloor())
        {
            velocity.Y = 0;
            velocity.X = 0;
        }

        // Add the gravity.
        if (!IsOnFloor())
        {
            velocity.Y += _gravity * (float)delta;
        }

        AnimatedSprite2D sprite = GetNode<AnimatedSprite2D>(SpriteNodeName);

        if (_jumpVelocity != Vector2.Zero)
        {
            velocity = _jumpVelocity;

            if (!_lastJumpZeroAngle)
            {
                sprite.FlipH = velocity.X < 0;
            }

            _jumpVelocity = Vector2.Zero;
        }

        Velocity = velocity;

        if (Velocity.Y > 0)
        {
            sprite.Play(JumperAnimations.AnimationJump);
        }
        else if (Velocity.Y < 0)
        {
            sprite.Play(JumperAnimations.AnimationFall);
        }

        bool justLanded = !_wasOnFloor && IsOnFloor();
        bool stuckInAir =
            (sprite.Animation == JumperAnimations.AnimationFall || sprite.Animation == JumperAnimations.AnimationJump)
            && Velocity.Y == 0;

        if (justLanded || stuckInAir)
        {
            sprite.Play(JumperAnimations.AnimationLand);
        }

        _wasOnFloor = IsOnFloor();

        UpdateNameTransparency();

        MoveAndSlide();
    }

    public void OnSpriteAnimationFinished()
    {
        AnimatedSprite2D sprite = GetNode<AnimatedSprite2D>(SpriteNodeName);

        if (sprite.Animation == JumperAnimations.AnimationLand)
        {
            sprite.Play(JumperAnimations.AnimationIdle);
        }
    }

    private void ResetNameTimer()
    {
        _fontVisibilityTimerStartTime = Time.GetTicksMsec();

        GetNode<RichTextLabel>(NameNodeName).Visible = true;
    }

    private void UpdateNameTransparency()
    {
        float alpha = _hasJumped ? CalculateFontAlpha() : 1f;

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
}
