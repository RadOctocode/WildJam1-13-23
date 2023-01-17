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
	private const float MINSPEED = 0.01f;
	private Vector3 tallestPoint;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		initQueue();
		canSpawn = true;
		GetNode<Timer>("AnimalTimer").Start();
		Score = 0;
		tallestPoint = new Vector3(0.0f,0.0f,0.0f);
	}

	public override void _Process(float delta)
	{
		var sceneAnimals = GetTree().GetNodesInGroup("Animals");
		if(sceneAnimals.Count != 0){
			//If there are animals in the scene
			for (int i =0; i < sceneAnimals.Count; i++){
				Vector3 currentTranslation = ((Spatial)sceneAnimals[i]).GlobalTransform.origin;
				if(currentTranslation.y < 0){
					GetTree().Quit();
				}
			}
		}
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
		animal.AddToGroup("Animals");
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
