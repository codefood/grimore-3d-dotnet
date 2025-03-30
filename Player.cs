using System.Linq;
using Godot;

namespace grimore3ddotnet;

public partial class Player : CharacterBody3D
{
	private Node3D _cameraPivot;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_cameraPivot = GetChildren().OfType<Node3D>().FirstOrDefault(d => d.Name == "CameraPivot");
	}
	
	const float MouseSensitivity = 0.01f;
	private static readonly float TiltLimit = Mathf.DegToRad(75);

	public override void _UnhandledInput(InputEvent @event)
	{
		//nicked from https://docs.godotengine.org/en/stable/tutorials/3d/spring_arm.html
		base._UnhandledInput(@event);
		if (@event is not InputEventMouseMotion mouseMotion) return;
		var x = _cameraPivot.Rotation.X;
		x -= mouseMotion.Relative.Y * MouseSensitivity;
		x = Mathf.Clamp(x, -TiltLimit, TiltLimit);
		_cameraPivot.Rotation = new Vector3(x, -mouseMotion.Relative.X * MouseSensitivity, _cameraPivot.Rotation.Z);


	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}