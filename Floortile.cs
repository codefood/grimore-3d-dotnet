using Godot;

namespace Grimore;

public partial class Floortile : StaticBody3D
{
    public override void _Ready()
    {
        MouseEntered += () =>
        {
            var mesh = GetNode<MeshInstance3D>("Cube");
            var material = mesh.GetActiveMaterial(0);
            var shader = material?.NextPass as ShaderMaterial;
            shader?.SetShaderParameter("color", new Vector4(0, 1, 0, 1));
        };
        
        MouseExited += () =>
        {
            var mesh = GetNode<MeshInstance3D>("Cube");
            var material = mesh.GetActiveMaterial(0);
            var shader = material?.NextPass as ShaderMaterial;
            shader?.SetShaderParameter("color", new Vector4(0.1f, 0.1f, 0.3f, .8f));
        };
    }
}