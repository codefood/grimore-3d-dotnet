using Godot;

namespace Grimore.Entities;

public partial class Key : Node3D, IInteractable
{

    public bool PlayerInteraction(Player player)
    {
        player.Keys++;
        QueueFree();
        return true;
    }
}