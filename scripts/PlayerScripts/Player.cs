using Godot;

public partial class Player : Area2D
{
	[Export] public float Acceleration = 600f;        // ускорение при move_up
	[Export] public float MaxSpeed = 400f;            // максимальная скорость
	[Export] public float RotationSpeed = 180f;       // градусов/сек
	[Export] public float Friction = 170f;            // коэффициент автоторможения
	[Export] public float DashPower = 450f;
	[Export] public float DashMaxSpeed = 1000f;
	[Signal] public delegate void HitEventHandler();

    private Vector2 velocity = Vector2.Zero;          // текущая скорость корабля
	private Vector2 screenSize;

	public override void _Ready()
	{
		screenSize = GetViewportRect().Size;
		//Hide();
	}

    private void OnBodyEntered(Node2D body)
    {
        Hide();
        EmitSignal(SignalName.Hit);
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
    }

    public override void _Process(double delta)
	{
		float dt = (float)delta;

		// ---------- ROTATION ----------
		if (Input.IsActionPressed("move_left"))
			RotationDegrees -= RotationSpeed * dt;

		if (Input.IsActionPressed("move_right"))
			RotationDegrees += RotationSpeed * dt;

		Vector2 forward = -Transform.Y;

        // ---------- ACCELERATION ----------
        if (Input.IsActionPressed("move_up"))
		{
			velocity += forward * Acceleration * dt;
        }

		// ---------- BACKWARD ACCELERATION ----------
		if (Input.IsActionPressed("move_down"))
		{
			velocity -= forward * Acceleration * 0.6f * dt; // назад обычно медленнее
		}

		if (Input.IsActionJustPressed("dash"))
		{
            velocity += forward * DashPower;
        }

        float currentMax = Input.IsActionJustPressed("dash") ? DashMaxSpeed : MaxSpeed;
        // ---------- LIMIT SPEED ----------
        if (velocity.Length() > currentMax)
		velocity = velocity.Normalized() * MaxSpeed;

		// ---------- AUTO FRICTION ----------
		if (!Input.IsActionPressed("move_up") && !Input.IsActionPressed("move_down"))
		{
			velocity = velocity.MoveToward(Vector2.Zero, Friction * dt);
		}

		// ---------- APPLY MOTION ----------
		Position += velocity * dt;

		// ---------- SCREEN BOUNDS ----------
		Position = new Vector2(
			Mathf.Clamp(Position.X, 0, screenSize.X),
			Mathf.Clamp(Position.Y, 0, screenSize.Y)
		);
	}
}
