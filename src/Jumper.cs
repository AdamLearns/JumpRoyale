using Godot;
using System;

public partial class Jumper : CharacterBody2D
{
	private const string SpriteNodeName = "Sprite";
	private const string NameNodeName = "Name";
	private const string ParticleSystemNodeName = "Glow";
	private bool wasOnFloor = false;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	private Vector2 jumpVelocity;

	public PlayerData playerData;

	public void Init(int x, int y, string userName, PlayerData playerData)
	{
		Position = new Vector2(x, y);
		Name = userName;
		this.playerData = playerData;
		GetNode<Label>(NameNodeName).Text = userName;

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
		sprite.SpriteFrames = SpriteFrameCreator.getInstance().GetSpriteFrames(gender, charNumber, clothingNumber);
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

	public void Jump(int angle, int power)
	{
		if (IsOnFloor())
		{
			// Only allow percentage inputs
			power = Math.Clamp(power, 1, 100);

			double normalizedPower = Math.Sqrt(power * 5 * gravity);

			jumpVelocity.X = Mathf.Cos(Mathf.DegToRad(angle + 180));
			jumpVelocity.Y = Mathf.Sin(Mathf.DegToRad(angle + 180));
			jumpVelocity = jumpVelocity.Normalized() * (float)normalizedPower;
			playerData.NumJumps++;
		}
	}

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

		if (jumpVelocity != Vector2.Zero)
		{
			velocity = jumpVelocity;
			sprite.FlipH = velocity.X < 0;
			jumpVelocity = Vector2.Zero;
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

		var justLanded = !wasOnFloor && IsOnFloor();
		var stuckInAir = (sprite.Animation == JumperAnimations.AnimationFall || sprite.Animation == JumperAnimations.AnimationJump) && Velocity.Y == 0;

		if (justLanded || stuckInAir)
		{
			sprite.Play(JumperAnimations.AnimationLand);
		}

		wasOnFloor = IsOnFloor();

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
