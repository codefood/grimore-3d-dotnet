using System;
using System.Linq;
using Godot;

namespace Grimore.Entities;

[GlobalClass]
public partial class Player : CharacterBody3D, IActor
{
	private PackedScene _spellScene = ResourceLoader.Load<PackedScene>("res://spell.tscn");
	private Shader _damage = ResourceLoader.Load<Shader>("res://damage.gdshader");
	
	private bool _allowInput;
	private Vector2 _currentDirection;
	private Timer _timer;
	private string SpellColor { get; set; } = "white";
	public event Action<int> HealthChanged;
	private int health = 3;

	private Node3D PlayerEntity => GetChildren()
		.OfType<Node3D>()
		.First(f => f.Name == "player");

	void SelectSpellColour(string colour)
	{
		GD.Print($"Setting spell colour to {colour}");
		SpellColor = colour;
	}

	public void StartTurn()
	{
		HealthChanged!.Invoke(health);
		_allowInput = true;
	}
	
	private void SetShaderTo(Material material)
	{
		foreach (var mesh in PlayerEntity.GetChildren().OfType<MeshInstance3D>())
		{
			mesh.MaterialOverlay = material;
		}
	}

	public void TakeDamage()
	{
		health--;
		HealthChanged!.Invoke(health);
		GD.Print($"Player taking damage, down to {health} HP");
		
		var damageMaterial = new ShaderMaterial()
		{
			Shader = _damage,
		};
		damageMaterial.SetShaderParameter("active", true);
		SetShaderTo(damageMaterial);
		
		_timer = new Timer()
		{
			Autostart = true,
			WaitTime = 3f,
			OneShot = true,
			Name = $"damage timer",
		};
		_timer.Timeout += ResetDamageCallback;
		AddChild(_timer);
	}

	private void ResetDamageCallback()
	{
		SetShaderTo(null);
		_timer.QueueFree();
		_timer = null;
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

			var angle = Angle(direction);
			PlayerEntity.SetBasis(new Basis(new Vector3(0, 1, 0), angle));
			_currentDirection = direction;

			IActor.InvokeActing(new Move(this, direction));
			_allowInput = false;
		}

		if (!ev.IsActionPressed(Actions.Act)) return;
		
		var instance = (Spell)_spellScene.Instantiate();
		instance.Name = "Spell";

		instance.Position = Position + new Vector3(_currentDirection.X, 0.5f, _currentDirection.Y);
			
		var spellColour = Color.FromString(SpellColor, Color.FromHtml("000000"));
		instance.Setup(spellColour, 1, _currentDirection);
			
		IActor.InvokeActing(new Summon(this, instance));
		_allowInput = false;
	}

	float Angle(Vector2 direction)
	{
		//horrible. there's definitely a good way of doing this.
		if (direction == Vector2.Up)
			return 0;
		if (direction == Vector2.Left)
			return Mathf.Pi / 2;
		if (direction == Vector2.Down)
			return Mathf.Pi;
		if (direction == Vector2.Right)
			return -(Mathf.Pi / 2);
		return 0;
	}

}