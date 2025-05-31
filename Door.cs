using Godot;

public partial class Door : Node3D
{
    private MeshInstance3D _doorMesh;
    private bool IsOpen { get; set; }

    public override void _Ready()
    {
        _doorMesh = GetChild<MeshInstance3D>(0);
        ToggleMesh();
    }

    private void ToggleMesh() => 
        _doorMesh.Visible = !IsOpen;

    public void Open()
    {
        GD.Print("Door is opening");
        IsOpen = true;
        ToggleMesh();
    }
}
