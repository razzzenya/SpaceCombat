using Godot;

public partial class Camera : Camera2D
{
    [Export] public Node2D Target { get; set; }
    [Export] public float DashZoomAmount = 0.2f;
    [Export] public float ZoomReturnSpeed = 1f;

    private Vector2 defaultZoom = new(1, 1);
    private Vector2 dashZoom;
    private Vector2 targetZoom;
    public override void _Ready()
    {
        if (!GodotObject.IsInstanceValid(Target))
            return;
        dashZoom = defaultZoom * DashZoomAmount;
        targetZoom = defaultZoom;

        Target?.Connect("Dash", new Callable(this, nameof(OnPlayerDash)));
    }

    public override void _Process(double delta)
    {
        if (!GodotObject.IsInstanceValid(Target)) return;
        float dt = (float)delta;
        Position = Target.Position;
        Zoom = Zoom.Lerp(targetZoom, dt * ZoomReturnSpeed);
    }

    private void OnPlayerDash()
    {
        targetZoom = dashZoom;

        Timer timer = new()
        {
            WaitTime = 0.15f,
            OneShot = true
        };
        AddChild(timer);
        timer.Start();
        timer.Timeout += () => targetZoom = defaultZoom;
    }
}