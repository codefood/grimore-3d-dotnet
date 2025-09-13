using System.Linq;
using Godot;

namespace Grimore.Entities;

public partial class Door : Node3D, IInteractable
{
    private MeshInstance3D _doorMesh;
    private CollisionShape3D _collision;
    private bool IsOpen { get; set; }

    public override void _Ready()
    {
        _doorMesh = GetChild<MeshInstance3D>(0);
        _collision = GetChildren().OfType<CollisionShape3D>().First();
        SetMeshVisibility();
    }

    private void SetMeshVisibility()
    {
        _doorMesh.Visible = !IsOpen;
        _collision.Disabled = IsOpen;
    }

    public bool Interact(Player player)
    {
        if (IsOpen) return true;
        if (player.Keys <= 0) return false;
        
        player.Keys--;
        GD.Print($"{Name} is opening, {player.Keys} keys left");
        IsOpen = true;
        SetMeshVisibility();
        return true;

    }
}