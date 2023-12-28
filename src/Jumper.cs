using Godot;
using System;

public partial class Jumper : CharacterBody2D
{
	private const string AnimationIdle = "idle";
	private const string AnimationJump = "jump";
	private const string AnimationFall = "fall";
	private const string AnimationLand = "land";
	private const string SpriteNodeName = "Sprite";
	private const string NameNodeName = "Name";
	private bool wasOnFloor = false;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	private Vector2 jumpVelocity;

	public void Reset()
	{
		RandomNumberGenerator rng = new RandomNumberGenerator();

		Position = new Vector2(rng.RandiRange(0, 1000), 0);
		jumpVelocity = new Vector2(0, 0);
		Velocity = new Vector2(0, 0);
	}

	public void Init(int xCoord, string userName)
	{
		Position = new Vector2(xCoord, 0);
		Name = userName;
		GetNode<Label>(NameNodeName).Text = userName;
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
			sprite.Play(AnimationJump);
		}
		else if (Velocity.Y < 0)
		{
			sprite.Play(AnimationFall);
		}

		var justLanded = !wasOnFloor && IsOnFloor();
		var stuckInAir = (sprite.Animation == AnimationFall || sprite.Animation == AnimationJump) && Velocity.Y == 0;

		if (justLanded || stuckInAir)
		{
			sprite.Play(AnimationLand);
		}

		wasOnFloor = IsOnFloor();

		MoveAndSlide();
	}

	public void OnSpriteAnimationFinished()
	{
		var sprite = GetNode<AnimatedSprite2D>(SpriteNodeName);
		if (sprite.Animation == AnimationLand)
		{
			sprite.Play(AnimationIdle);
		}
	}
}
