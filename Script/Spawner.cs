using Godot;
using System;
using System.Collections.Generic;

public class Spawner : Node
{
	// List of animals to pull from when spawning.
	[Export]
	public List<PackedScene> Animals;
	// Camera used by the game to look at the tower.
	public Position3D GameCamera;
	public int Score;	
	private bool canSpawn = false;
	private Queue<int> queue;
	private const int QUEUE_SIZE = 3;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		initQueue();
		canSpawn = true;
		GetNode<Timer>("AnimalTimer").Start();
		Score = 0;
	}
	
	private void initQueue() {
		queue = new Queue<int>();
		for (int i = 0; i < QUEUE_SIZE; ++i) {
			enqueueAnimal();
		}
	}
	
	/** Randomly select an index for an animal from the possible set of initialized animals.
	* This index is invalid if `Animal` mutates after being called.
	*/
	private int chooseAnimal() {
		return (int)(GD.Randi() % Animals.Count);
	}
	
	private void enqueueAnimal() {
		int next = chooseAnimal();
		GD.Print("Enqueing animal ", next);
		queue.Enqueue(next);
	}
	
	private int rotateQueue() {
		int index = queue.Dequeue();
		enqueueAnimal();
		return index;
	}

	public void SpawnAnimal(PackedScene animalScene, Vector2 pos) {
		var animal = (Spatial)animalScene.Instance();
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
			PackedScene animal = Animals[rotateQueue()];
			SpawnAnimal(animal, eventMouseButton.Position);
		}
	}

	public void _on_AnimalTimer_timeout()
	{
		rotateQueue();
		Score--;
	}

}
