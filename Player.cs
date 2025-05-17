using System.Linq;
using Godot;

namespace Grimore;

public partial class Player : CharacterBody3D, IActor
{
	
	private PackedScene _spellScene = ResourceLoader.Load<PackedScene>("res://spell.tscn");
	private Shader _damage = ResourceLoader.Load<Shader>("res://damage.gdshader");
	
	private bool _allowInput;
	
	public event IActor.OnActing Acting;
	public void StartTurn() => 
		_allowInput = true;

	public void TakeDamage()
	{
		var theFuckingModel = GetChildren().First(f => f.Name == "fooman");
		var damageMaterial =  new ShaderMaterial()
		{
			Shader = _damage,
		};
		foreach (var mesh in theFuckingModel.GetChildren().OfType<MeshInstance3D>())
		{
			mesh.MaterialOverlay = mesh.MaterialOverlay != null 
				? null 
				: damageMaterial;
			
			for (var surf = 0; surf < mesh.Mesh.GetSurfaceCount(); surf++)
				mesh.Mesh.SurfaceSetMaterial(surf, damageMaterial);
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
			
			GetChildren()
				.OfType<Node3D>()
				.First(f => f.Name == "fooman")
				.SetBasis(new Basis(new Vector3(0, 1, 0), direction.Angle() + Mathf.Pi / 2));
	
			Acting!.Invoke(new Move(this, direction));
			_allowInput = false;

		}
		if (ev.IsActionPressed(Actions.Act))
		{
			var instance = (Spell)_spellScene.Instantiate();
			instance.Name = "Spell";
			
			instance.SetPosition(Position + (Vector3.Forward * World.TileSize) + Vector3.Up / 2);
			
			var spellColour = Color.FromString(SpellColor, Color.FromHtml("000000"));
			instance.Setup(spellColour, 1, Vector2.Up);
			
			Acting!.Invoke(new Summon(this, instance));
			_allowInput = false;
		}
	}

	public string SpellColor { get; set; } = "white";
	
}