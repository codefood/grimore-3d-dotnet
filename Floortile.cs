using Godot;
using Grimore.Entities;

namespace Grimore;

public partial class Floortile : StaticBody3D
{
    public override void _Ready()
    {
        var mesh = GetNode<MeshInstance3D>("Cube");
        var shader = mesh.GetActiveMaterial(0)?.NextPass as ShaderMaterial;

        TurnManager.OnTurnStart += actor =>
        {
            if (actor is Player player)
            {
                DefaultColour = 
                    AbsDistanceTo(player) <= 4f
                        ? _green
                        : _red;
            }
            else
            {
                DefaultColour = _gray;
            }
            shader?.SetShaderParameter("color", DefaultColour);
        };
        
        MouseEntered += () =>
        {
            if (GameState.Current != States.Playing || States.Playing.Command is not TargetSpell target) return;
            var actor = target.Actor as Node3D;

            shader?.SetShaderParameter("color",
                AbsDistanceTo(actor) <= 2f
                    ? _green
                    : _red);
            
            shader?.SetShaderParameter("lineThickness", 0.5f);
        };
        MouseExited += () =>
        {
            shader?.SetShaderParameter("color", DefaultColour);
            shader?.SetShaderParameter("lineThickness", 0.05f);
        };
    }

    private Vector4 DefaultColour { get; set; } = _gray;
    private static readonly Vector4 _gray = new(0.1f, 0.1f, 0.3f, .8f);
    private static readonly Vector4 _green = new(0.02f, 0.7f, 0.02f, 1);
    private static readonly Vector4 _red = new(0.72f, 0.02f, 0.02f, 1);

    private float AbsDistanceTo(Node3D actor) => 
        Mathf.Abs(GetGlobalPosition().DistanceTo(actor!.GetGlobalPosition()));
}