using Godot;
using JourneyCameraPrototype;
using System;

public partial class TriggerCinematic : StaticBody3D
{
	[Export]
	Node3D player;

	[Export]
	bool isCinematic;

	[Export]
	float triggerDistance = 50;

	bool didTrigger = true;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if ((Position - player.Position).Length() < triggerDistance)
		{
			((ICameraController)player).ChangeToCinematic();

        }
	}
}
