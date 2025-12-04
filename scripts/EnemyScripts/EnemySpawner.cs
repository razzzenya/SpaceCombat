using Godot;
using System.Collections.Generic;

public partial class EnemySpawner : Node2D
{
    [Export] public PackedScene Enemy1Scene { get; set; }
    [Export] public int MaxEnemies1 { get; set; }
    [Export] public int Enemy1Health { get; set; }
    [Export] public Player Player;
    [Export] public float SpawnInterval { get; set; }
    [Export] public float SpawnDistance { get; set; }

    private float timer = 0f;
    private readonly List<Node2D> spawned = [];
    public override void _Ready()
    {

    }

    public override void _Process(double delta)
    {
        if (!GodotObject.IsInstanceValid(Player)) return;
        float dt = (float)delta;
        timer -= dt;
        CleanupDeadEnemies();
        if (spawned.Count >= MaxEnemies1) return;

        if (timer <= 0f)
        {
            SpawnEnemy();
            timer = SpawnInterval;
        }
    }

    private void CleanupDeadEnemies()
    {
        for (int i = spawned.Count - 1; i >= 0; i--)
        {
            if (!IsInstanceValid(spawned[i]))
                spawned.RemoveAt(i);
        }
    }

    private void SpawnEnemy()
    {
        Vector2 spawnPos = GetSpawnPosOutsideView();

        var enemy = Enemy1Scene.Instantiate<Enemy1>();
        enemy.GlobalPosition = spawnPos;
        enemy.Player = Player;
        //enemy.Health = Enemy1Health;

        GetTree().CurrentScene.AddChild(enemy);
        spawned.Add(enemy);
    }

    private Vector2 GetSpawnPosOutsideView()
    {
        Vector2 playerPos = Player.GlobalPosition;

        float angle = (float)GD.RandRange(0, Mathf.Tau);

        Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        return playerPos + dir * SpawnDistance;
    }
}
