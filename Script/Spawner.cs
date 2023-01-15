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
	public int Score;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		canSpawn = true;
		GetNode<Timer>("AnimalTimer").Start();
		Score = 0;
	}

	public Spatial RotateAnimal(){
		var choice = GD.Randi() % Animals.Count;
		return (Spatial)Animals[(int)choice].Instance();
	}

	public void SpawnAnimal(Vector2 pos) {
		var animal = RotateAnimal();
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

	public void _on_AnimalTimer_timeout()
	{
		RotateAnimal();
		Score--;
	}

}
