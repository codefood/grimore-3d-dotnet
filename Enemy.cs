using Godot;
using grimore3ddotnet;

public partial class Enemy : AnimatableBody3D, IActor
{
    public void TakeDamage()
    {
        this.QueueFree();
    }
}
