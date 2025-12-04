using Godot;
using System;

public partial class AsteroidSpawner : Node2D
{
	[Export] public PackedScene AsteroidScene { get; set; }
	[Export] public int Count { get; set; }
	[Export] public int SpawnArea { get; set; }
	[Export] public float PlayerSafeRadius { get; set; }
    [Export] public Node2D Player;
    public override void _Ready()
	{
        var rng = new RandomNumberGenerator();
        rng.Randomize();

        for (int i = 0; i < Count; i++)
        {
            Vector2 pos;

            do
            {
                pos = new Vector2(
                    rng.RandfRange(-SpawnArea, SpawnArea),
                    rng.RandfRange(-SpawnArea, SpawnArea)
                );
            }
            while (pos.DistanceTo(Player.Position) < PlayerSafeRadius);

            var asteroid = AsteroidScene.Instantiate<RigidBody2D>();
            asteroid.Position = pos;
            asteroid.RotationDegrees = rng.RandfRange(0, 360);
            AddChild(asteroid);
        }
    }

	public override void _Process(double delta)
	{
	}
}
