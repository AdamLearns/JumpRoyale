using System;
using Godot;

public partial class Jumper : CharacterBody2D
{
    private const string SpriteNodeName = "Sprite";
    private const string NameNodeName = "Name";
    private const string ParticleSystemNodeName = "Glow";
    private bool _wasOnFloor = false;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    private Vector2 _jumpVelocity;

    public PlayerData playerData;

    private bool _hasJumped = false;
    private ulong _fontVisibilityTimerStartTime;

    public void Init(int x, int y, string userName, PlayerData playerData)
    {
        Position = new Vector2(x, y);
        Name = userName;
        this.playerData = playerData;
        GetNode<RichTextLabel>(NameNodeName).Text = "[center]" + userName + "[/center]";

        SetCharacter(playerData.CharacterChoice);

        if (playerData.GlowColor != null)
        {
            SetGlow(playerData.GlowColor);
        }
    }

    public void SetCrazyParticles()
    {
        var particles = GetGlowNode();
        particles.Amount *= 5;
    }

    public void SetCharacter(int choice)
    {
        playerData.CharacterChoice = choice;
        var gender = choice > 9 ? "f" : "m";
        var charNumber = ((choice - 1) % 9 / 3) + 1;
        var clothingNumber = ((choice - 1) % 3) + 1;
        GD.Print("Choice: " + choice + " Gender: " + gender + " Char: " + charNumber + " Clothing: " + clothingNumber);
        var sprite = GetNode<AnimatedSprite2D>(SpriteNodeName);
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
            var color = Color.FromHtml(colorString);
            color = new Color(color.R, color.G, color.B, 1f);
            var particles = GetGlowNode();
            particles.SelfModulate = color;
            particles.Visible = true;
            playerData.GlowColor = color.ToHtml(false);
        }
        catch (Exception e)
        {
            GD.Print($"Failed to set glow color to {colorString}", e);
        }
    }

    private CpuParticles2D GetGlowNode()
    {
        return GetNode<CpuParticles2D>(ParticleSystemNodeName);
    }

    public void DisableGlow()
    {
        var particles = GetGlowNode();
        particles.Visible = false;
        playerData.GlowColor = null;
    }

    public override void _Ready()
    {
        GetNode<AnimatedSprite2D>(SpriteNodeName).AnimationFinished += OnSpriteAnimationFinished;
    }

    public void RandomJump()
    {
        var rng = new RandomNumberGenerator();
        Jump(rng.RandiRange(45, 135), rng.RandiRange(10, 100));
    }

    private bool _lastJumpZeroAngle = false;

    public void Jump(int angle, int power)
    {
        if (IsOnFloor())
        {
            double normalizedPower = Math.Sqrt(power * 5 * gravity);

            _jumpVelocity.X = Mathf.Cos(Mathf.DegToRad(angle + 180));
            _jumpVelocity.Y = Mathf.Sin(Mathf.DegToRad(angle + 180));
            _jumpVelocity = _jumpVelocity.Normalized() * (float)normalizedPower;
            playerData.NumJumps++;
            setNameAlpha(1f);

            _hasJumped = true;
            resetNameTimer();
            _lastJumpZeroAngle = angle == 90; // 0 in the command is expressed here as 90.
        }
    }

    private void resetNameTimer()
    {
        _fontVisibilityTimerStartTime = Time.GetTicksMsec();
        GetNode<RichTextLabel>(NameNodeName).Visible = true;
    }

    public void playerWon()
    {
        // Reset their _hasJumped property so that their name will be visible until they jump again.
        _hasJumped = false;
    }

    private void updateNameTransparency()
    {
        float alpha;
        if (!_hasJumped)
        {
            alpha = 1f;
        }
        else
        {
            const ulong totalTimeMs = 5000;

            ulong now = Time.GetTicksMsec();
            ulong diffMs = now - _fontVisibilityTimerStartTime;
            float diff = diffMs / (float)totalTimeMs;

            alpha = Math.Max(0, 1 - diff);
        }
        setNameAlpha(alpha);
    }

    private void setNameAlpha(float alpha) => GetNode<RichTextLabel>(NameNodeName).Modulate = new Color(1, 1, 1, alpha);

    public void SetColor(string hexColor)
    {
        var sprite = GetNode<AnimatedSprite2D>(SpriteNodeName);
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
            velocity.Y += gravity * (float)delta;

        var sprite = GetNode<AnimatedSprite2D>(SpriteNodeName);

        if (_jumpVelocity != Vector2.Zero)
        {
            velocity = _jumpVelocity;
            if (!this._lastJumpZeroAngle)
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

        var justLanded = !_wasOnFloor && IsOnFloor();
        var stuckInAir =
            (sprite.Animation == JumperAnimations.AnimationFall || sprite.Animation == JumperAnimations.AnimationJump)
            && Velocity.Y == 0;

        if (justLanded || stuckInAir)
        {
            sprite.Play(JumperAnimations.AnimationLand);
        }

        _wasOnFloor = IsOnFloor();

        updateNameTransparency();

        MoveAndSlide();
    }

    public void OnSpriteAnimationFinished()
    {
        var sprite = GetNode<AnimatedSprite2D>(SpriteNodeName);
        if (sprite.Animation == JumperAnimations.AnimationLand)
        {
            sprite.Play(JumperAnimations.AnimationIdle);
        }
    }
}
