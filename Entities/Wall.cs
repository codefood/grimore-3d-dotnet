using Godot;

namespace Grimore.Entities;

public partial class Wall : StaticBody3D, IInteractable
{
    public bool Interact(Player player) => false;
}