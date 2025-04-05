using Godot;

namespace grimore3ddotnet;

public partial class Spell : RigidBody3D
{
    public override void _Ready()
    {
        var size = new Vector3(0.33f, 0.33f, 0.33f);
        
        AddChild(new CollisionShape3D
        {
            Shape = new BoxShape3D
            {
                Size = size
            }
        });
        AddChild(new MeshInstance3D
        {
            Mesh = new BoxMesh
            {
                Size = size,
                Material = new StandardMaterial3D
                {
                    AlbedoColor = new Color(GD.Randi())
                }
            }
        });
    }
}