using Godot;
using System;
using System.Collections.Generic;

public class Spawner : Node
{
	private bool canSpawn = false;

	// List of animals to pull from when spawning.
	[Export]
	public List<PackedScene> Animals;
	// Camera used by the game to look at the tower.
	public Position3D GameCamera;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		canSpawn = true;
	}

	public void SpawnAnimal(Vector2 pos) {
		var choice = GD.Randi() % Animals.Count;
		var animal = (Spatial)Animals[(int)choice].Instance();
		var distanceToOrigin = GameCamera.Translation.z;
		var spawnPoint = GameCamera.GetNode<Camera>("Camera").ProjectPosition(pos, distanceToOrigin);
		animal.Translation = spawnPoint;
		
		AddChild(animal);
	}

	public override void _Input(InputEvent @event)
	{
		// Mouse in viewport coordinates.
		if (canSpawn
			&& @event is InputEventMouseButton eventMouseButton
			&& eventMouseButton.Pressed)
		{
			SpawnAnimal(eventMouseButton.Position);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
	}
}
