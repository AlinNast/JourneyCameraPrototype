using Godot;
using System;

public partial class CameraController : Node3D
{
	[Export]
	Node3D lookTarget;
	[Export]
	Camera3D camera;

    // The lookTarget is the node that the controller takes position of
    // Camera controller is both the one containing the logic and the pivot point for the camera,
    // so it will be positioned at the same place as the target, and the camera will be offset from it based on the azimuth and zenith angles.
    // Camera will always look at the target. and will be positioned at a distance (radius) from the target.


    // Azimuth is the angle between the Forward vector of the target (where az is 0)
    // and the actual orientation of the camera
    float azimuth;
	// Zenith is the Pitch or elevation of the camera.
	// Here used as the angle between the vertical vector of the target and the ground paralel
    float zenith;


    // Distance from the target
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
        // 1. Keep the Controller (Pivot) locked to the target's position
        this.GlobalPosition = lookTarget.GlobalPosition;


        // 2. Update azimuth and zenith based on input
		zenith +=  (float)delta * (float)(Input.GetJoyAxis(0, JoyAxis.RightY)) ;
		azimuth += (float)delta * (float)(Input.GetJoyAxis(0, JoyAxis.RightX)) ;

        // 3. Clamp Zenith so the camera doesn't flip upside down at the poles
        // Clamping between approx 0 and -80 degrees (in radians)
        zenith = Mathf.Clamp(zenith, Mathf.DegToRad(-80), Mathf.DegToRad(0));

        // 4. Apply rotation to the Controller Node
        // We create a Basis from Euler angles (X, Y, Z)
        this.Basis = Basis.FromEuler(new Vector3(zenith, azimuth, 0));

        // 5. Position the Camera relative to the Controller
        // Since the Controller is rotating, we just push the camera back 
        // along its local Z axis by the radius.
        camera.Position = new Vector3(0, 0, radius);

        // 6. Ensure the camera is looking at the pivot point
        camera.LookAt(this.GlobalPosition, Vector3.Up);


        // draw target and info
        DebugDraw3D.DrawSphere(lookTarget.GlobalPosition, 1, new Color(0.5f, 0.5f, 0.5f));
        //DebugDraw3D.DrawArrow(Vector3.Zero, new Vector3(0,azimuth*10,0), new Color(0.5f, 0.2f, 0));
        //DebugDraw3D.DrawArrow(Vector3.Zero, new Vector3(0, zenith * 10, 0), new Color(0.5f, 0.2f, 0));



        // Clamp Zenith to prevent "Gimbal Lock" or flipping over the top (pole)
        // 1.55 radians is roughly 89 degrees.
        //zenith = Mathf.Clamp(zenith, -1.55f, 1.55f);

        // Apply position and look at the target
        //camera.GlobalPosition = CalculateCameraPosition();
        //camera.LookAt(lookTarget.GlobalPosition, Vector3.Up);

        //Basis yaw = new Basis(Vector3.Up, azimuth);
        //Basis pitch = new Basis(Vector3.Right, zenith);


        //this.RotateX((float)(Input.GetJoyAxis(0, JoyAxis.RightY) * -0.2));
        //this.RotateY((float)(Input.GetJoyAxis(0, JoyAxis.RightX) * 0.2));
        //this.RotateZ((float)(Input.GetJoyAxis(0, JoyAxis.RightY) * -0.2));
        //this.Basis = pitch * yaw;

        // Debug Zenith
        //DebugDraw3D.DrawArrow(Vector3.Zero, pitch.X, Colors.Red, 0.1f, true);
        //DebugDraw3D.DrawArrow(Vector3.Zero, pitch.Y, Colors.Green, 0.1f, true);
        //DebugDraw3D.DrawArrow(Vector3.Zero, pitch.Z, Colors.Blue, 0.1f, true);

        // Debug Azimuth
        //DebugDraw3D.DrawArrow(Vector3.Zero, yaw.X, Colors.Red, 0.1f, true);
        //DebugDraw3D.DrawArrow(Vector3.Zero, yaw.Y, Colors.Green, 0.1f, true);
        //DebugDraw3D.DrawArrow(Vector3.Zero, yaw.Z, Colors.Blue, 0.1f, true);

        //DebugDraw3D.DrawArrow(Vector3.Zero, (Vector3.Right * 10) * (pitch * yaw));


        //camera.Position = Position + CalculateCameraPosition();
        //camera.LookAt(lookTarget.GlobalPosition);
    }

    private Vector3 CalculateCameraPosition()
	{
        float x = radius * Mathf.Sin(azimuth) * Mathf.Cos(zenith);
        float y = radius * Mathf.Sin(zenith);
        float z = radius * Mathf.Cos(azimuth) * Mathf.Cos(zenith);

        Vector3 offset = new Vector3(x, y, z);

        return this.GlobalPosition + offset;
    }
}
