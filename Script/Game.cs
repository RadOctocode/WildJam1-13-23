using Godot;
using System;

public class Game : Spatial
{
	private Position3D camera;
	private Spawner spawner;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		camera = GetNode<Position3D>("Camera");
		spawner = GetNode<Spawner>("Spawner");
		spawner.GameCamera = camera;
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
