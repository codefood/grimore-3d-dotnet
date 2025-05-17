using System.Linq;
using Godot;

namespace Grimore;

public partial class Player : CharacterBody3D, IActor
{
	private Node3D _thirdPersonCameraPivot;
	private PackedScene _spellScene = ResourceLoader.Load<PackedScene>("res://spell.tscn");
	private bool _allowInput = false;
	private bool _cameraFree = false;
	private Node3D _isometricCameraPivot;

	const float MouseSensitivity = 0.01f;
	private static readonly float XLimit = Mathf.DegToRad(30);
	private static readonly float YLimit = Mathf.DegToRad(120);

	public event IActor.OnActing Acting;
	public void StartTurn() => 
		_allowInput = true;


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

		if (CameraMode == World.CameraMode.isometric) return;
		
		if (ev is not InputEventMouseMotion mouseMotion) return;
		
		var x = _thirdPersonCameraPivot.Rotation.X;
		x -= mouseMotion.Relative.Y * MouseSensitivity;
		x = Mathf.Clamp(x, -XLimit, XLimit);
		
		var y = _thirdPersonCameraPivot.Rotation.Y;
		y += -mouseMotion.Relative.X * MouseSensitivity; 
		y = Mathf.Clamp(y, -YLimit, YLimit);
		
		_thirdPersonCameraPivot.Rotation = new Vector3(x, y, _thirdPersonCameraPivot.Rotation.Z);
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
	public World.CameraMode CameraMode { get; private set; }

	public void SetCameraMode(World.CameraMode mode)
	{
		CameraMode = mode;
		if (mode == World.CameraMode.isometric)
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
}