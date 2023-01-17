using Godot;
using System;

public class Game : Spatial
{
	private Position3D camera;
	private Spawner spawner;
	private Control hud;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		camera = GetNode<Position3D>("Camera");
		spawner = GetNode<Spawner>("Spawner");
		spawner.GameCamera = camera;
		
		hud = GetNode<Control>("HUD");
		spawner.Connect(nameof(Spawner.AnimalQueueUpdated), this, nameof(onAnimalQueueUpdated));
		
		demoUpcoming();
	}
	
	private void demoUpcoming() {
		var mesh = hud.GetNode<MeshInstance>("AnimalQueueContainer/Viewport/MeshInstance");
		var catMesh = ResourceLoader.Load("res://Scenes/Animals/Seal.tscn::2") as Mesh;
		mesh.Mesh = catMesh;
		mesh.Scale = new Vector3(0.005f, 0.005f, 0.005f);
	}

	private void onAnimalQueueUpdated(Godot.Collections.Array queue)
	{
		GD.Print("Animal queue updated");
	}
}
