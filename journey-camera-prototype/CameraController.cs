using Godot;
using System;

public partial class CameraController : Node3D
{
	[Export]
	Node3D lookTarget;
	[Export]
	Camera3D camera;


	// Azimuth is the angle between the Forward vector of the target (where az is 0)
	// and the actual orientation of the camera
	float azimuth;
	// Zenith is the Pitch or elevation of the camera.
	// Here used as the angle between the vertical vector of the target and the ground paralel
    float zenith;

	float radius = 10;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        azimuth = -lookTarget.Basis.Z.Z;
		zenith = lookTarget.Basis.Y.Y;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//this.GlobalPosition = lookTarget.GlobalPosition;
		// draw target and info
        //DebugDraw3D.DrawSphere(lookTarget.GlobalPosition, 1, new Color(0.5f, 0.5f, 0.5f));
		//DebugDraw3D.DrawArrow(Vector3.Zero, new Vector3(0,azimuth*10,0), new Color(0.5f, 0.2f, 0));
        //DebugDraw3D.DrawArrow(Vector3.Zero, new Vector3(0, zenith * 10, 0), new Color(0.5f, 0.2f, 0));


		zenith += (float)(Input.GetJoyAxis(0, JoyAxis.RightY)) * 0.1f;
		azimuth += (float)(Input.GetJoyAxis(0, JoyAxis.RightX)) *0.1f;

		//zenith = Math.Clamp(zenith, 10, 80);

        Basis yaw = new Basis(Vector3.Up, azimuth);
		Basis pitch = new Basis(Vector3.Right, zenith);
		//this.RotateX((float)(Input.GetJoyAxis(0, JoyAxis.RightY) * -0.2));
		//this.RotateY((float)(Input.GetJoyAxis(0, JoyAxis.RightX) * 0.2));
		//this.RotateZ((float)(Input.GetJoyAxis(0, JoyAxis.RightY) * -0.2));
		//this.Basis = pitch * yaw;

		DebugDraw3D.DrawArrow(Vector3.Zero, pitch.X, Colors.Red, 0.1f, true);
        DebugDraw3D.DrawArrow(Vector3.Zero, pitch.Y, Colors.Green, 0.1f, true);
        DebugDraw3D.DrawArrow(Vector3.Zero, pitch.Z, Colors.Blue, 0.1f, true);

        //DebugDraw3D.DrawArrow(Vector3.Zero, (Vector3.Right * 10) * (pitch * yaw));


        //camera.Position = Position + CalculateCameraPosition();
		//camera.LookAt(lookTarget.GlobalPosition);
	}

	private Vector3 CalculateCameraPosition()
	{
		float x = lookTarget.Basis.X.X + radius * (float)Math.Sin(zenith) * (float)Math.Cos(azimuth);
		float y = lookTarget.Basis.Y.Y + radius * (float)Math.Cos(zenith);
		float z = lookTarget.Basis.Z.Z + radius * (float)Math.Sin(zenith) * (float)Math.Sin(azimuth);
		return new Vector3(x, y, z);
    }
}
