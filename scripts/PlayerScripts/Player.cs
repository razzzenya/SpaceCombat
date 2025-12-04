using Godot;

public partial class Player : RigidBody2D
{
    [Export] public float Acceleration = 600f;
    [Export] public float MaxSpeed = 400f;
    [Export] public float RotationSpeed = 5.2f;
    [Export] public float Friction = 170f;
    [Export] public float DashPower = 950f;
    [Export] public float DashMaxSpeed = 1000f;
    [Export] public float DashDecay = 200f;
    [Export] public float AsteroidCollisionDamage = 1f;
    [Export] public float Health { get; set; }
    [Export] public float MaxHealth { get; set; }

    /// guns
    [Export] public float CollisionDamage { get; set; }
    [Export] public float CollisionDamageCoolDown { get; set; }
    [Export] public float Damage { get; set; }
    [Export] public Node2D FirePoint { get; set; }
    /// laser beam
    [Export] public PackedScene LaserBeamScene { get; set; }
    [Export] public float BeamFireCooldown { get; set; }
    private float fireTimer = 0f;
    
    private float currentSpeedLimit;
    private Vector2 gameZone;
    private float collisionDamageTimer = 0f;

    // camera
    [Signal] public delegate void DashEventHandler();


    public override void _Ready()
    {
        currentSpeedLimit = MaxSpeed;
    }

    private void LaserBeamShoot()
    {
        fireTimer = BeamFireCooldown;
        var projectile = LaserBeamScene.Instantiate<LaserBeam>();
        projectile.Damage = Damage;
        projectile.GlobalPosition = FirePoint.GlobalPosition + FirePoint.Transform.Y * -30f + FirePoint.Transform.X * 1.7f;
        projectile.Rotation = FirePoint.GlobalRotation;
        GetTree().CurrentScene.AddChild(projectile);
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;
        if (collisionDamageTimer > 0f)
            collisionDamageTimer -= dt;
        // == SHOOTING ===
        fireTimer -= dt;
        if (Input.IsActionPressed("shoot") && fireTimer <= 0f)
        {
            LaserBeamShoot();
        }

        // === ROTATION ===
        float targetAngularVelocity = 0f;

        if (Input.IsActionPressed("move_left"))
            targetAngularVelocity = -RotationSpeed;
        else if (Input.IsActionPressed("move_right"))
            targetAngularVelocity = RotationSpeed;
        // плавное торможение вращения
        AngularVelocity = Mathf.MoveToward(AngularVelocity, targetAngularVelocity, RotationSpeed * dt * 2f);

        Vector2 forward = -Transform.Y;

        // === ACCELERATION ===
        Vector2 vel = LinearVelocity;

        if (Input.IsActionPressed("move_up"))
            vel += forward * Acceleration * dt;

        if (Input.IsActionPressed("move_down"))
            vel -= forward * (Acceleration * 0.6f) * dt;

        // === DASH ===
        if (Input.IsActionJustPressed("dash"))
        {
            vel += forward * DashPower;
            currentSpeedLimit = DashMaxSpeed;
            EmitSignal(SignalName.Dash);
        }

        // === SPEED LIMIT ===
        if (vel.Length() > currentSpeedLimit)
            vel = vel.Normalized() * currentSpeedLimit;

        currentSpeedLimit = Mathf.MoveToward(
            currentSpeedLimit,
            MaxSpeed,
            DashDecay * dt
        );

        // === FRICTION ===
        if (!Input.IsActionPressed("move_up") && !Input.IsActionPressed("move_down"))
            vel = vel.MoveToward(Vector2.Zero, Friction * dt);

        // === APPLY VELOCITY ===
        LinearVelocity = vel;
    }

    private void OnBodyEntered(Node body)
    {
        if (collisionDamageTimer > 0f)
            return;
        GD.Print("Collision with: " + body.Name);
        if (body is Asteroid)
        {
            Health -= AsteroidCollisionDamage;
            collisionDamageTimer = CollisionDamageCoolDown;
            GD.Print("Player Health: " + Health);
        }
        else if (body is Enemy1)
        {
            Health -= (body as Enemy1).Damage;
            collisionDamageTimer = CollisionDamageCoolDown;
            GD.Print("Player Health: " + Health);
        }
        if (Health <= 0)
        {
            GD.Print("Player destroyed!");
            QueueFree();
        }
    }
}