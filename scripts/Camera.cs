using Godot;
using System;

public partial class Camera : Camera3D
{
    public float Sensitivity = 0.001f;
    [Export] public float Speed = 50f;
    private Vector3 Velocity;
    private float Pitch;
    private float Yaw;
	Node3D shapes;
	float angle = 0.5f;

    public override void _Ready()
    {
		shapes = GetTree().CurrentScene.GetNode<Node3D>("shapes");
        //Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotionEvent && Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            Yaw -= mouseMotionEvent.Relative.X * Sensitivity;
            Pitch -= mouseMotionEvent.Relative.Y * Sensitivity;

            // Clamp pitch to avoid flipping
            Pitch = Mathf.Clamp(Pitch, -Mathf.Pi / 2, Mathf.Pi / 2);

            // Set the camera rotation
            Rotation = new Vector3(Pitch, Yaw, 0);
        }
		/*if (@event is InputEventMouseButton mouseButtonEvent){
			if (mouseButtonEvent.ButtonIndex == MouseButton.WheelUp && mouseButtonEvent.Pressed)
				shapes.RotateX(Mathf.DegToRad(angle));

			if (mouseButtonEvent.ButtonIndex == MouseButton.WheelDown && mouseButtonEvent.Pressed)
			shapes.RotateX(Mathf.DegToRad(-angle));
		}*/
    }

    public override void _PhysicsProcess(double delta)
    {
        HandleMovement((float)delta);
    }

    private void HandleMovement(float delta)
    {
        Velocity = Vector3.Zero;

        // Use local basis for movement directions
        if (Input.IsActionPressed("forward"))
            Velocity -= Transform.Basis.Z;

        if (Input.IsActionPressed("backward"))
            Velocity += Transform.Basis.Z;

        if (Input.IsActionPressed("left"))
            Velocity -= Transform.Basis.X;

        if (Input.IsActionPressed("right"))
            Velocity += Transform.Basis.X;

        if (Input.IsActionPressed("ui_space"))
            Velocity += Transform.Basis.Y;

        if (Input.IsActionPressed("ui_shift"))
            Velocity -= Transform.Basis.Y;


		if (Input.IsActionJustPressed("ui_cancel"))
        {
            Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Visible
                ? Input.MouseModeEnum.Captured
                : Input.MouseModeEnum.Visible;
        }


        if (Velocity.Length() > 0)
            Velocity = Velocity.Normalized();


        Transform = new Transform3D(Transform.Basis, Transform.Origin + Velocity * Speed * delta);
    }
}
