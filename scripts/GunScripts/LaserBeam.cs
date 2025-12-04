using Godot;
using System;

public partial class LaserBeam : RigidBody2D
{
    [Signal] public delegate void HitEventHandler();
    [Export] public float Speed { get; set; }
    [Export] public float LifeTime { get; set; }
    [Export] public float Damage { get; set; }
    private float timer = 0f;
    public override void _PhysicsProcess(double delta)
    {
        var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animatedSprite2D.Play();
        LinearVelocity = -Transform.Y * Speed;
        timer += (float)delta;
        if (timer >= LifeTime)
        {
            QueueFree();
        }
    }
    public void OnBodyEntered(Node body)
    {
        QueueFree();
        Hide();
        EmitSignal(SignalName.Hit);
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
    }
    public override void _Ready()
	{

	}

	public override void _Process(double delta)
	{
	}
}
