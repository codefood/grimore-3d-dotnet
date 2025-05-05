using Godot;
using Grimore;

public partial class Enemy : AnimatableBody3D, IActor
{
    public void TakeDamage()
    {
        this.QueueFree();
    }
}
