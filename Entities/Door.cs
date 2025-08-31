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

    public bool PlayerInteraction()
    {
        if (IsOpen) return true;
        
        GD.Print("Door is opening");
        IsOpen = true;
        SetMeshVisibility();
        return false;

    }
}