using Godot;

public partial class Enemy1 : RigidBody2D
{
    [Export] public float MoveSpeed = 250f;
    [Export] public float RotationSpeed = 3f;
    [Export] public float AsteroidCollisionDamage = 0.5f;
    [Export] public float Health { get; set; }
    [Export] public float Damage { get; set; }
    [Export] public Player Player { get; set; }

    public override void _PhysicsProcess(double delta)
    {
        if (!GodotObject.IsInstanceValid(Player))
            return;

        float dt = (float)delta;

        Vector2 toPlayer = Player.GlobalPosition - GlobalPosition;
        Vector2 dir = toPlayer.Normalized();
        //float dist = toPlayer.Length();

        // угол на игрока
        float targetAngle = dir.Angle();
        float angleDiff = Mathf.AngleDifference(Rotation, targetAngle);

        AngularVelocity = Mathf.Clamp(angleDiff * 4f, -RotationSpeed, RotationSpeed);
        LinearVelocity += dir * MoveSpeed * dt;

        //if (dist < EvadeDistance)
        //{
        //    Vector2 evadeDir = toPlayer.Rotated(Mathf.Pi / 2).Normalized();
        //    LinearVelocity = evadeDir * EvadeStrength;
        //}
        //else
        //{
        //    // плавный поворот
        //    AngularVelocity = Mathf.Clamp(angleDiff * 4f, -RotationSpeed, RotationSpeed);

        //    //float forceAmount = 300f;
        //    //ApplyCentralImpulse(dir * forceAmount * (float)delta);
        //    LinearVelocity += dir * MoveSpeed * dt;
        //    Vector2 forward = Transform.X;
        //    //LinearVelocity = forward * MoveSpeed;
        //}
    }

    private void OnBodyEntered(Node body)
    {
        if (body is Asteroid)
        {
            Health -= AsteroidCollisionDamage;
        }
        else if (body is Player)
        {
            GD.Print("Collided with Player");
            Health -= Player.CollisionDamage;
        }
        else if (body is LaserBeam)
        {
            Health -= (body as LaserBeam).Damage;
        }
        if (Health <= 0f)
        {
            GD.Print("I'm dead!");
            QueueFree();
        }
    }
}

//[Export] public PackedScene BulletScene;
//[Export] public Node2D FirePoint;
//[Export] public float ShootCooldown = 1f;
//private float shootTimer = 0f;
//private void TryShoot(float dt)
//{
//    shootTimer -= dt;

//    if (shootTimer <= 0f)
//    {
//        shootTimer = ShootCooldown;

//        var bullet = BulletScene.Instantiate<RigidBody2D>();
//        bullet.GlobalPosition = FirePoint.GlobalPosition;
//        bullet.Rotation = FirePoint.GlobalRotation;

//        GetTree().CurrentScene.AddChild(bullet);
//    }
//}

//private RayCast2D rayForward;
//private RayCast2D rayLeft;
//private RayCast2D rayRight;

//public override void _Ready()
//{
//    rayForward = GetNode<RayCast2D>("RayForward");
//    rayLeft = GetNode<RayCast2D>("RayLeft");
//    rayRight = GetNode<RayCast2D>("RayRight");
//}

//private Vector2 ObstacleAvoidance()
//{
//    Vector2 steer = Vector2.Zero;

//    if (rayForward.IsColliding())
//        steer += -Transform.X * 300f;

//    if (rayLeft.IsColliding())
//        steer += Transform.Y * 300f;

//    if (rayRight.IsColliding())
//        steer += -Transform.Y * 300f;

//    return steer;
//}

//PhysicsProcess
//Vector2 avoidance = ObstacleAvoidance();
//LinearVelocity += avoidance * dt;
