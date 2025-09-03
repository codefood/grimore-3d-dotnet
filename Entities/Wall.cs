using Godot;

namespace Grimore.Entities;

public partial class Wall : StaticBody3D, IInteractable
{
    public bool PlayerInteraction(Player player) => false;
}