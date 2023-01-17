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
		// check to see if the rigid body has an exteremely low speed
		// check to see if the object has at least one collision
			//get the tallest Item from this
		if(sceneAnimals.Count != 0){
			//If there are animals in the scene
			List<Spatial> relevantAnimals = new List<Spatial>();
			for (int i =0; i < sceneAnimals.Count; i++){
				Spatial currentNode = (Spatial)sceneAnimals[i];
				RigidBody currentBody = currentNode.GetNode<RigidBody>("RigidBody");
				if (isStacked(currentBody)){
					relevantAnimals.Add(currentNode);
				}
			}
			
			Spatial newTallestObject = getTallestAnimal(relevantAnimals);
			Vector3 newTallestPoint = newTallestObject.GlobalTranslation;
			GD.Print(tallestPoint.y);
			if (newTallestPoint.y > tallestPoint.y){
				tallestPoint = newTallestPoint;
			}
			
		}
	}

	private bool isStacked(RigidBody currentBody){
		Vector3 currentSpeed = currentBody.LinearVelocity;
		if (currentSpeed.x <= MINSPEED && currentSpeed.y <= MINSPEED && currentSpeed.z <= MINSPEED){
			if (currentBody.GetCollidingBodies().Count != 0){
				return true;
			}
		}
		return false;
	}

	private Spatial getTallestAnimal(List<Spatial> animals){
		Spatial returnVal = new Spatial{};
		Vector3 currentTallest = new Vector3(0.0f,0.0f,0.0f);
		for (int i = 0; i< animals.Count; i++){
			var currentSpatial = animals[i];
			Vector3 currentTranslation = currentSpatial.GetGlobalTranslation();
			if(currentTallest.y < currentTranslation.y){
				currentTallest = currentTranslation;
				returnVal = animals[i];
			}
		}
		return returnVal;
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
