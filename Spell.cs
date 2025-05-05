using Godot;

namespace Grimore;

public partial class Spell : AnimatableBody3D, IActor
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

    public override void _PhysicsProcess(double delta)
    {
        // base._PhysicsProcess(delta);
        // var collision = MoveAndCollide(new Vector3(Direction.X, 0.1f, Direction.Z));
        // if (collision == null) return;
        // var node = collision.GetCollider();
        // if (node is Enemy enemy)
        // {
        //     enemy.TakeDamage();
        // }
        //
        // QueueFree();
    }

    public event IActor.OnActing Acting;
    public void StartTurn()
    {
        
    }
}