using System;
using System.Linq;
using Godot;

namespace Grimore;

public partial class Player : CharacterBody3D, IActor
{
	
	private PackedScene _spellScene = ResourceLoader.Load<PackedScene>("res://spell.tscn");
	private Shader _damage = ResourceLoader.Load<Shader>("res://damage.gdshader");
	
	private bool _allowInput;
	private Vector2 _currentDirection;

	private Node3D Fooman => GetChildren()
		.OfType<Node3D>()
		.First(f => f.Name == "fooman");

	public event IActor.OnActing Acting;
	public void StartTurn() => 
		_allowInput = true;

	public void TakeDamage()
	{
		var damageMaterial =  new ShaderMaterial()
		{
			Shader = _damage,
		};
		SetMaterial(damageMaterial);
		// await Task.Delay(TimeSpan.FromSeconds(1));
	}

	private void SetMaterial(ShaderMaterial material)
	{
		foreach (var mesh in Fooman.GetChildren().OfType<MeshInstance3D>())
		{
			mesh.MaterialOverlay = mesh.MaterialOverlay != material
				? null 
				: material;
		
			//this horror makes the head also go red, wtf?
			for (var surf = 0; surf < mesh.Mesh.GetSurfaceCount(); surf++)
				mesh.Mesh.SurfaceSetMaterial(surf, material);
		}
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
			Fooman.SetBasis(new Basis(new Vector3(0, 1, 0), angle));
			_currentDirection = direction;

			Acting!.Invoke(new Move(this, direction));
			_allowInput = false;

		}
		if (ev.IsActionPressed(Actions.Act))
		{
			var instance = (Spell)_spellScene.Instantiate();
			instance.Name = "Spell";

			instance.Position = Position + new Vector3(_currentDirection.X, 0.5f, _currentDirection.Y);
			
			var spellColour = Color.FromString(SpellColor, Color.FromHtml("000000"));
			instance.Setup(spellColour, 1, _currentDirection);
			
			Acting!.Invoke(new Summon(this, instance));
			_allowInput = false;
		}
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
	public string SpellColor { get; set; } = "white";
	
}