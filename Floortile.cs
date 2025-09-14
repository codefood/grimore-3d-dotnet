using Godot;
using Grimore.Entities;

namespace Grimore;

public partial class Floortile : StaticBody3D
{
    public override void _Ready()
    {
        var mesh = GetNode<MeshInstance3D>("Cube");
        var shader = mesh.GetActiveMaterial(0)?.NextPass as ShaderMaterial;
        
        MouseEntered += () =>
        {
            if (GameState.Current != States.Playing || States.Playing.Command is not TargetSpell target) return;
            var actor = target.Actor as Node3D;

            shader?.SetShaderParameter("color",
                Mathf.Abs(GetGlobalPosition().DistanceTo(actor!.GetGlobalPosition())) <= 2f
                    ? new Vector4(0.02f, 0.7f, 0.02f, 1)
                    : new Vector4(0.72f, 0.02f, 0.02f, 1));
            
            shader?.SetShaderParameter("lineThickness", 0.5f);
        };
        MouseExited += () =>
        {
            shader?.SetShaderParameter("color", new Vector4(0.1f, 0.1f, 0.3f, .8f));
            shader?.SetShaderParameter("lineThickness", 0.05f);
        };
    }
}