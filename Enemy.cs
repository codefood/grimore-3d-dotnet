using Godot;
using Grimore;

public partial class Enemy : AnimatableBody3D, IActor
{
    public void TakeDamage() => 
        QueueFree();

    public void Move(Vector2 direction) => 
        Position += new Vector3(direction.X, 0, direction.Y);
}
