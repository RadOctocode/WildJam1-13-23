using Godot;
using System;

public class Animal : Spatial
{


	public override void _Ready(){
		GetNode<Timer>("AnimalTimer").Start();
	}
	
	public void _on_Timer_timeout()
	{
		body.queue_free();
	}
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}

