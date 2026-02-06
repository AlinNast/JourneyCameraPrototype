using Godot;
using JourneyCameraPrototype;
using System;

public partial class CameraController : Node3D, ICameraController
{
	[Export]
	Node3D lookTarget;
    [Export]
    Node3D cameraTargetPosition;
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
    float idealRadius = 10;
    float availableRadius = 10;

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


        UpdateCameraCollision((float)delta);


        // Apply rotation to the Controller Node
        this.Basis = Basis.FromEuler(new Vector3(zenith, azimuth, 0));

        // Position the Camera relative to the Controller
        // Since the Controller is rotating, we just push the camera back 
        // along its local Z axis by the radius.

        cameraTargetPosition.Position = new Vector3(0, 0, radius);
        
        // this should be in the camere but it isnt but the camera works anyway :))
        DebugDraw3D.DrawSphere(cameraTargetPosition.GlobalPosition, 0.5f, new Color(1, 0, 0));

        camera.GlobalPosition = cameraTargetPosition.GlobalPosition;

        // Ensure the camera is looking at the pivot point
        camera.LookAt(this.GlobalPosition, Vector3.Up);


    }




    private void ProcessInput(float delta)
    {
        // Apply Acceleration only when input is above the deadzone, otherwise apply friction to slow down the camera when the joystick is released.
        if (Mathf.Abs(Input.GetJoyAxis(0, JoyAxis.RightX)) > joystickDeadzone)
        {
            azimuthVelocity += delta * -(float)(Input.GetJoyAxis(0, JoyAxis.RightX)) * acceleration;
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

    private void UpdateCameraCollision(float delta)
    {

        // Calculate the 'Desired' Global Position of the camera
        // take the current orientation (Basis) and look at the 'Back' vector (Z axis)
        // and multiply it by our maximum radius.
        Vector3 rayStart = this.GlobalPosition;
        Vector3 rayEnd = rayStart + (this.GlobalBasis.Z * idealRadius); // Main Ray

        // additional ray ends to create a fan
        Vector3 rayEnd2 = rayStart + (this.GlobalBasis.Z.Rotated(Vector3.Up, Mathf.DegToRad(10)) * idealRadius);
        Vector3 rayEnd3 = rayStart + (this.GlobalBasis.Z.Rotated(Vector3.Up, Mathf.DegToRad(-10)) * idealRadius);
        Vector3 rayEnd4 = rayStart + (this.GlobalBasis.Z.Rotated(this.GlobalBasis.X, Mathf.DegToRad(5)) * idealRadius);
        Vector3 rayEnd5 = rayStart + (this.GlobalBasis.Z.Rotated(this.GlobalBasis.X, Mathf.DegToRad(-5)) * idealRadius);


        // Access the Physics Space
        var spaceState = GetWorld3D().DirectSpaceState;

        // Create the Ray Parameters
        // Main Ray
        var query = PhysicsRayQueryParameters3D.Create(rayStart, rayEnd);
        DebugDraw3D.DrawLine(rayStart, rayEnd, new Color(1, 0, 0));

        // Additional Rays for better collision detection
        var query2 = PhysicsRayQueryParameters3D.Create(rayStart, rayEnd2);
        DebugDraw3D.DrawLine(rayStart, rayEnd2, new Color(1, 0, 0));

        var query3 = PhysicsRayQueryParameters3D.Create(rayStart, rayEnd3);
        DebugDraw3D.DrawLine(rayStart, rayEnd3, new Color(1, 0, 0));

        var query4 = PhysicsRayQueryParameters3D.Create(rayStart, rayEnd4);
        DebugDraw3D.DrawLine(rayStart, rayEnd4, new Color(1, 0, 0));

        var query5 = PhysicsRayQueryParameters3D.Create(rayStart, rayEnd5);
        DebugDraw3D.DrawLine(rayStart, rayEnd5, new Color(1, 0, 0));

        // Set the collision mask to 1
        query.CollisionMask = 1;
        query2.CollisionMask = 1;
        query3.CollisionMask = 1;
        query4.CollisionMask = 1;
        query5.CollisionMask = 1;

        // Perform the Trace
        var result = spaceState.IntersectRay(query);
        var result2 = spaceState.IntersectRay(query2);
        var result3 = spaceState.IntersectRay(query3);
        var result4 = spaceState.IntersectRay(query4);
        var result5 = spaceState.IntersectRay(query5);

        float lerpSpeed = (availableRadius < radius) ? 30.0f : 10.0f;
        float minDistance = idealRadius;

        // Investigate
        if (result.Count > 0) // Main Ray
        {
            Vector3 hitPos = (Vector3)result["position"];
            float distance = rayStart.DistanceTo(hitPos);
            if (distance < minDistance)
                minDistance = distance;
        }
        if (result2.Count > 0) // Right Ray
        {
            Vector3 hitPos = (Vector3)result2["position"];
            float distance = rayStart.DistanceTo(hitPos);
            azimuth += -0.0007f;
            if (distance < minDistance)
                minDistance = distance;
        }
        if (result3.Count > 0) // Left Ray
        {
            Vector3 hitPos = (Vector3)result3["position"];
            float distance = rayStart.DistanceTo(hitPos);
            azimuth += 0.0007f;
            if (distance < minDistance)
                minDistance = distance;
        }
        if (result4.Count > 0) // Up Ray
        {
            Vector3 hitPos = (Vector3)result4["position"];
            float distance = rayStart.DistanceTo(hitPos);
            if (distance < minDistance)
                minDistance = distance;
        }
        if (result5.Count > 0) // Down Ray
        {
            Vector3 hitPos = (Vector3)result5["position"];
            float distance = rayStart.DistanceTo(hitPos);
            if (distance < minDistance)
                minDistance = distance;
        }
        GD.Print($"WALL DETECTED! Distance: {minDistance}");

        // Adjust the radius based on the closest wall detected
        if (minDistance < idealRadius)
        {
            // Wall hit!
            availableRadius = minDistance - 1;
            // Lerp the radius towards the available radius to create a smooth transition when hitting walls
            radius = Mathf.Lerp(radius, availableRadius, delta * lerpSpeed);
            //GD.Print($"WALL DETECTED! Distance: {minDistance}");
        }
        else
        {
            // No wall, we can go back to our ideal radius, But speed is slower for beauty
            radius = Mathf.Lerp(radius, idealRadius, delta * 2);
            //GD.Print("Path is clear.");
        }
        radius = Mathf.Clamp(radius, 0.5f, idealRadius);
    }

    public void ChangeToCinematic()
    {
        GD.Print("voila");
    }

    public void ChangeToPlayer()
    {
        throw new NotImplementedException();
    }
}
