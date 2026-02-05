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
    float azimuthVelocity = 0;

    // Zenith is the Pitch or elevation of the camera.
    // Here used as the angle between the vertical vector of the target and the ground paralel
    float zenith;
    float zenithVelocity = 0;

    // Distance from the target
    float radius = 10;

    float joystickDeadzone = 0.2f;

    float acceleration = 2.0f;
    float friction = 3.0f;

    float zenithResistance = 0.5f;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        azimuth = -lookTarget.Basis.Z.Z;
		zenith = lookTarget.Basis.Y.Y;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        // Keep the Controller (Pivot) locked to the target's position
        this.GlobalPosition = lookTarget.GlobalPosition;


        // Update azimuth and zenith based on input
        ProcessInput((float)delta);





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


    }




    private void ProcessInput(float delta)
    {
        // Apply Acceleration only when input is above the deadzone, otherwise apply friction to slow down the camera when the joystick is released.
        if (Mathf.Abs(Input.GetJoyAxis(0, JoyAxis.RightX)) > joystickDeadzone)
        {
            azimuthVelocity += delta * (float)(Input.GetJoyAxis(0, JoyAxis.RightX)) * acceleration;
        }
        else 
        {
            azimuthVelocity -= azimuthVelocity * friction * delta;
        }
        // Clamp the velocity to prevent it from getting too high, which would make the camera uncontrollable.
        azimuthVelocity = Mathf.Clamp(azimuthVelocity, -2.0f, 2.0f);

        // Update the Azimuth based on vecloty and Keep it in the range of 0 to 360 degrees
        azimuth += azimuthVelocity * delta;
        azimuth = Mathf.PosMod(azimuth, Mathf.Tau);


        // Zenith control like azimuth
        if (Mathf.Abs(Input.GetJoyAxis(0, JoyAxis.RightY)) > joystickDeadzone)
        {
            zenithVelocity += delta * (float)(Input.GetJoyAxis(0, JoyAxis.RightY)) * acceleration;
        }
        else
        {
            zenithVelocity -= zenithVelocity * friction * delta;
        }



        // Zenith resistance near the poles approaches 0 (1 means no resistance, 0 means full resistance)
        if (zenith < Mathf.DegToRad(-50) && zenithVelocity < 0)
        {
            zenithResistance = Mathf.Remap(zenith, Mathf.DegToRad(-50), Mathf.DegToRad(-80), 1.0f, 0.0f);
        }
        else if (zenith > Mathf.DegToRad(-30) && zenithVelocity > 0)
        {
            zenithResistance = Mathf.Remap(zenith, Mathf.DegToRad(-30), Mathf.DegToRad(0), 1.0f, 0.0f);
        }
        else
        {
            zenithResistance = 1.0f;
        }

        // Resistance slows zenith from reaching the ceiling or floor and stops its movement entirely at the limits
        zenith += zenithVelocity * delta * (zenithResistance*zenithResistance);

        // Clamp Zenith so the camera doesn't flip upside down at the poles
        zenith = Mathf.Clamp(zenith, Mathf.DegToRad(-80), Mathf.DegToRad(0));
    }
}
