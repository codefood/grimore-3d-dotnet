using Godot;

namespace Grimore.Entities;

public partial class Spell : AnimatableBody3D, IActor, IInteractable
{
    public void Setup(Color color, int force, Vector2 direction)
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
                    AlbedoColor = color
                }
            }
        });
        Force = force;
        
        Direction = direction;
    }

    private Vector2 Direction { get; set; }

    private int Force { get; set; }

    public void StartTurn() => 
        IActor.InvokeActing(new Move(this, Direction * Force));

    public bool PlayerInteraction(Player player)
    {
        player?.TakeDamage();
        IActor.InvokeDying(this);
        return true;
    }
}