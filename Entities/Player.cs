using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Grimore.Entities;

[GlobalClass]
public partial class Player : CharacterBody3D, IActor
{
	private PackedScene _spellScene = ResourceLoader.Load<PackedScene>(Paths.Entities.Spell);

	public Vector2? CurrentDirection;
	private Timer _timer;
	private string SpellColor { get; set; } = "white";
	public event Action<int> HealthChanged;
	public int Health = 3;
	ShaderMaterial _damageMaterial = new()
	{
		Shader = ResourceLoader.Load<Shader>(Paths.Shaders.Damage),
	};

	private Node3D PlayerEntity => GetNode<Node3D>("player");

	public int Keys { get; set; }

	void SelectSpellColour(string colour)
	{
		GD.Print($"Setting spell colour to {colour}");
		SpellColor = colour;
	}

	public void StartTurn() => 
		HealthChanged!.Invoke(Health);

	public override void _Ready()
	{
		base._Ready();
		SetShaderTo(_damageMaterial);
	}

	private void SetShaderTo(Material material)
	{
		foreach (var mesh in PlayerEntity.GetChildren().OfType<MeshInstance3D>())
		{
			//mesh.MaterialOverride = material;
		}
	}

	public void TakeDamage()
	{
		Health--;
		HealthChanged!.Invoke(Health);
		GD.Print($"Player taking damage, down to {Health} HP");
		
		if (Health <= 0)
		{
			IActor.InvokeDying(this);
			return;
		}
		
		_damageMaterial.SetShaderParameter("active", true);
		
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
		_damageMaterial.SetShaderParameter("active", false);
		_timer.QueueFree();
		_timer = null;
	}

	public override void _Input(InputEvent ev)
	{
		base._Input(ev);
				
		var directionsPressed = Actions.Directions
			.Where(k => ev.IsActionPressed(k.Key))
			.ToList();

		if (GameState.Current == States.Ended && directionsPressed.Any())
		{
			GameState.Start();
			return;
		}

		if (GameState.Current != States.Playing)
		{
			return;
		}

		Command action = null;
		
		if (directionsPressed.Count != 0) action = Move(directionsPressed);
		//if (ev.IsActionPressed(Actions.Act)) action = CastSpell();
		if (ev.IsActionPressed(Actions.Act)) action = new TargetSpell(this);

		if (action == null) return;
		
		IActor.InvokeActing(action);
	}

	private Move Move(List<KeyValuePair<string, Vector2>> directionsPressed)
	{
		var direction = directionsPressed.First().Value;

		var angle = Angle(direction);
		PlayerEntity.SetBasis(new Basis(new Vector3(0, 1, 0), angle));
		CurrentDirection = direction;
		var move = new Move(this, direction);
		return move;
	}

	private CastSpell CastSpell()
	{
		var instance = (Spell)_spellScene.Instantiate();
		instance.Name = "Spell";

		CurrentDirection ??= Actions.Directions[Actions.Up];

		instance.Position = Position + new Vector3(CurrentDirection!.Value.X * World.TileSize, 0.5f, CurrentDirection!.Value.Y * World.TileSize);

		var spellColour = Color.FromString(SpellColor, Color.FromHtml("000000"));
		instance.Setup(spellColour, 1, CurrentDirection!.Value);
		return new CastSpell(this, instance);
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