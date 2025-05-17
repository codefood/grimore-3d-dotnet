using System.Linq;
using Godot;

namespace Grimore;

public partial class Camera : Node
{
	private Node3D _thirdPersonCameraPivot;
	private Node3D _isometricCameraPivot;
	
	const float MouseSensitivity = 0.01f;
	private static readonly float XLimit = Mathf.DegToRad(30);
	private static readonly float YLimit = Mathf.DegToRad(120);
	public Mode CurrentMode { get; private set; }
	
	public enum Mode 
	{
		isometric,
		thirdPerson,
	}
	
	public void SetMode(Mode mode)
	{
		CurrentMode = mode;
		if (mode == Mode.isometric)
		{
			_isometricCameraPivot.Visible = true;
			_thirdPersonCameraPivot.Visible = false;
			_isometricCameraPivot.GetChildren().OfType<Camera3D>().First().MakeCurrent();
		}
		else
		{
			_isometricCameraPivot.Visible = false;
			_thirdPersonCameraPivot.Visible = true;
			_thirdPersonCameraPivot.GetChild<SpringArm3D>(0).GetChild<Camera3D>(0).MakeCurrent();
		}
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_thirdPersonCameraPivot = GetChildren()
			.OfType<Node3D>()
			.FirstOrDefault(d => d.Name == "ThirdPersonCameraPivot");
		_isometricCameraPivot = GetChildren()
			.OfType<Node3D>()
			.FirstOrDefault(d => d.Name == "IsometricCameraPivot");
	}
	
	public override void _UnhandledInput(InputEvent ev)
	{
		//nicked from https://docs.godotengine.org/en/stable/tutorials/3d/spring_arm.html
		base._UnhandledInput(ev);

		if (CurrentMode == Mode.isometric) return;
		
		if (ev is not InputEventMouseMotion mouseMotion) return;
		
		var x = _thirdPersonCameraPivot.Rotation.X;
		x -= mouseMotion.Relative.Y * MouseSensitivity;
		x = Mathf.Clamp(x, -XLimit, XLimit);
		
		var y = _thirdPersonCameraPivot.Rotation.Y;
		y += -mouseMotion.Relative.X * MouseSensitivity; 
		y = Mathf.Clamp(y, -YLimit, YLimit);
		
		_thirdPersonCameraPivot.Rotation = new Vector3(x, y, _thirdPersonCameraPivot.Rotation.Z);
	}

}