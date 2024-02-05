using System.Diagnostics.CodeAnalysis;
using Godot;

public partial class Fireball : RigidBody2D
{
    private Arena? _arena;

    public void Init([NotNull] Arena arena)
    {
        _arena = arena;
    }

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node body)
    {
        _arena.CreateExplosion(Position);
        QueueFree();
    }
}
