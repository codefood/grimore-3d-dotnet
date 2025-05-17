using System.Linq;
using Godot;

namespace Grimore;

public partial class Player : CharacterBody3D, IActor
{
	private Node3D _cameraPivot;
	private PackedScene _spellScene = ResourceLoader.Load<PackedScene>("res://spell.tscn");
	private bool _allowInput = false;
	private bool _cameraFree = false;
	
	const float MouseSensitivity = 0.01f;
	private static readonly float XLimit = Mathf.DegToRad(30);
	private static readonly float YLimit = Mathf.DegToRad(120);

	public event IActor.OnActing Acting;
	public void StartTurn()
	{
		_allowInput = true;
	}


	// Called when the node enters the scene tree for the first time.
	public override void _Ready() =>
		_cameraPivot = GetChildren()
			.OfType<Node3D>()
			.FirstOrDefault(d => d.Name == "CameraPivot");
	
	public override void _UnhandledInput(InputEvent ev)
	{
		//nicked from https://docs.godotengine.org/en/stable/tutorials/3d/spring_arm.html
		base._UnhandledInput(ev);

		if (CameraMode == World.CameraMode.isometric) return;
		
		if (ev is not InputEventMouseMotion mouseMotion) return;
		
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

	public override void _Input(InputEvent ev)
	{
		base._Input(ev);
		
		if (!_allowInput) return;
		
		var directionsPressed = Actions.Directions
			.Where(k => ev.IsActionPressed(k.Key))
			.ToList();
		
		if (directionsPressed.Count != 0)
		{
			var direction = directionsPressed.First().Value;
			
			GetChildren()
				.OfType<Node3D>()
				.First(f => f.Name == "fooman")
				.SetBasis(new Basis(new Vector3(0, 1, 0), direction.Angle() + Mathf.Pi / 2));
	
			Acting.Invoke(new Move(this, direction));
			_allowInput = false;

		}
		if (ev.IsActionPressed(Actions.Act))
		{
			var instance = (Spell)_spellScene.Instantiate();
			instance.Name = "Spell";
			
			instance.SetPosition(Position + (Vector3.Forward * World.TileSize) + Vector3.Up / 2);
			
			var spellColour = Color.FromString(SpellColor, Color.FromHtml("000000"));
			instance.Setup(spellColour, 1, Vector2.Up);
			
			Acting.Invoke(new Summon(this, instance));
			_allowInput = false;
		}
	}

	public string SpellColor { get; set; } = "white";
	public World.CameraMode CameraMode { get; set; }
}