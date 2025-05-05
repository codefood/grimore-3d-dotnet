using System.Linq;
using Godot;

namespace Grimore;

public partial class Player : CharacterBody3D, IActor
{
	private Node3D _cameraPivot;
	private PackedScene _spellScene = ResourceLoader.Load<PackedScene>("res://spell.tscn"); 
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() =>
		_cameraPivot = GetChildren()
			.OfType<Node3D>()
			.FirstOrDefault(d => d.Name == "CameraPivot");

	const float MouseSensitivity = 0.01f;
	private static readonly float XLimit = Mathf.DegToRad(30);
	private static readonly float YLimit = Mathf.DegToRad(90);

	public override void _UnhandledInput(InputEvent @event)
	{
		//nicked from https://docs.godotengine.org/en/stable/tutorials/3d/spring_arm.html
		base._UnhandledInput(@event);
		if (@event is not InputEventMouseMotion mouseMotion) return;
		
		var x = _cameraPivot.Rotation.X;
		x -= mouseMotion.Relative.Y * MouseSensitivity;
		x = Mathf.Clamp(x, -XLimit, XLimit);
		
		var y = _cameraPivot.Rotation.Y;
		y += -mouseMotion.Relative.X * MouseSensitivity; 
		y = Mathf.Clamp(y, -YLimit, YLimit);
		
		_cameraPivot.Rotation = new Vector3(x, y, _cameraPivot.Rotation.Z);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void CastSpell(World world)
	{
		var instance = (Spell)_spellScene.Instantiate();
		instance.SetPosition(Position + Vector3.Forward + Vector3.Up / 2);
		instance.Direction = Vector3.Forward;
		world.AddChild(instance);
	}
	
	public void Move(Vector2 direction) => 
		Position += new Vector3(direction.X, 0, direction.Y);
}