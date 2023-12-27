using Godot;
using System;

public partial class Jumper : CharacterBody2D
{
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
		GetNode<Label>("Name").Text = userName;
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
		var sprite = GetNode<AnimatedSprite2D>("Sprite");
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

		if (jumpVelocity != Vector2.Zero)
		{
			velocity = jumpVelocity;
			GetNode<AnimatedSprite2D>("Sprite").FlipH = velocity.X < 0;
			jumpVelocity = Vector2.Zero;
		}

		Velocity = velocity;

		wasOnFloor = IsOnFloor();

		MoveAndSlide();
	}
}
