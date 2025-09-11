using Godot;

namespace Grimore.Entities;

public partial class Key : Node3D, IInteractable
{

    public bool Interact(Player player)
    {
        player.Keys++;
        QueueFree();
        return true;
    }
}