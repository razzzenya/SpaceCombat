using Godot;

public partial class Interface : Sprite2D
{
    [Export] public Player Player;
    [Export] public Texture2D[] HpLevels;
    public float Health;
    public override void _Ready()
    {
        Position = new Vector2(70, 50);
    }

    public override void _Process(double delta)
    {
        if (Player == null) return;

        float hp = Player.Health;
        float max = Player.MaxHealth;

        int index = Mathf.RoundToInt((hp / max) * (HpLevels.Length - 1));
        index = Mathf.Clamp(index, 0, HpLevels.Length - 1);

        Texture = HpLevels[index];
    }
}
