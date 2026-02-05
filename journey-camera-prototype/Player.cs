using Godot;
using System;

public partial class Player : CharacterBody3D
{
    [Export] public float Speed = 5.0f;
    [Export] public float Acceleration = 15.0f;
    [Export] public float Friction = 20.0f;
    public float gravity = 10.0f;

    // This allows you to link your CameraController in the editor
    [Export] public Node3D CameraPivot;

    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = Velocity;

        if (!IsOnFloor())
            velocity.Y -= gravity * (float)delta;

        // Input for jumpig will go here

        // Get Joystick Input (-1.0 to 1.0)
        float inputX = Input.GetJoyAxis(0, JoyAxis.LeftX);
        float inputY = Input.GetJoyAxis(0, JoyAxis.LeftY);
        Vector2 input = new Vector2(inputX, inputY);

        // Apply Deadzone (Prevents "stick drift")
        if (input.Length() < 0.2f)
        {
            input = Vector2.Zero;
        }

        // Convert Input to World Direction
        Vector3 direction = Vector3.Zero;

        if (CameraPivot != null)
        {
            Basis camBasis = CameraPivot.GlobalTransform.Basis;
            // Calculate movement relative to Camera rotation
            Vector3 forward = -camBasis.Z;
            Vector3 right = camBasis.X;

            // Flatten vectors so the player doesn't move into the ground
            forward.Y = 0;
            right.Y = 0;
            forward = forward.Normalized();
            right = right.Normalized();

            direction = (forward * -input.Y) + (right * input.X);
        }
        else
        {
            // Fallback: Move along World Axes if no camera is linked
            direction = new Vector3(input.X, 0, input.Y);
        }

        // Apply Acceleration and Friction
        if (direction != Vector3.Zero)
        {
            velocity.X = Mathf.MoveToward(velocity.X, direction.X * Speed, Acceleration * (float)delta);
            velocity.Z = Mathf.MoveToward(velocity.Z, direction.Z * Speed, Acceleration * (float)delta);
        }
        else
        {
            velocity.X = Mathf.MoveToward(velocity.X, 0, Friction * (float)delta);
            velocity.Z = Mathf.MoveToward(velocity.Z, 0, Friction * (float)delta);
        }

       
        Velocity = velocity;
        MoveAndSlide();
    }
}