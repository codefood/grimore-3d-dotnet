using System.Linq;
using Godot;

public partial class Door : Node3D
{
    private MeshInstance3D _doorMesh;
    private CollisionShape3D _collision;
    private bool IsOpen { get; set; }

    public override void _Ready()
    {
        _doorMesh = GetChild<MeshInstance3D>(0);
        _collision = GetChildren().OfType<CollisionShape3D>().First();
        ToggleMesh();
    }

    private void ToggleMesh()
    {
        _doorMesh.Visible = !IsOpen;
        _collision.Disabled = IsOpen;
    }

    public void Open()
    {
        GD.Print("Door is opening");
        IsOpen = true;
        ToggleMesh();
    }
}
