using Godot;

namespace Grimore.Entities;

public partial class Tile : StaticBody3D
{
	private ShaderMaterial _shader;

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			TurnManager.OnTurnStart -= TurnManagerOnOnTurnStart;
			MouseEntered -= MouseEnter;
			MouseExited -= MouseExit;
		}
		base.Dispose(disposing);
	}

	private void TurnManagerOnOnTurnStart(IActor actor)
	{
		if (actor is Player player)
		{
			DefaultColour = 
				AbsDistanceTo(player) <= 4f
					? Green
					: Red;
		}
		else
		{
			DefaultColour = Gray;
		}
		_shader?.SetShaderParameter("color", DefaultColour);
	}

	public override void _Ready()
	{
		var mesh = GetNode<MeshInstance3D>("Cube");
		_shader = mesh.GetActiveMaterial(0)?.NextPass as ShaderMaterial;
        
		TurnManager.OnTurnStart += TurnManagerOnOnTurnStart;
		MouseEntered += MouseEnter;
		MouseExited += MouseExit;
	}

	private void MouseExit()
	{
		_shader?.SetShaderParameter("color", DefaultColour);
		_shader?.SetShaderParameter("lineThickness", 0.05f);
	}

	private void MouseEnter()
	{
		if (GameState.Current != States.Playing || States.Playing.Command is not TargetSpell target) return;
		var actor = target.Actor as Node3D;

		_shader?.SetShaderParameter("color",
			AbsDistanceTo(actor) <= 2f
				? Green
				: Red);
            
		_shader?.SetShaderParameter("lineThickness", 0.5f);
	}

	private Vector4 DefaultColour { get; set; } = Gray;
	private static readonly Vector4 Gray = new(0.1f, 0.1f, 0.3f, .8f);
	private static readonly Vector4 Green = new(0.02f, 0.7f, 0.02f, 1);
	private static readonly Vector4 Red = new(0.72f, 0.02f, 0.02f, 1);

	private float AbsDistanceTo(Node3D actor) => 
		Mathf.Abs(GetGlobalPosition().DistanceTo(actor!.GetGlobalPosition()));
}