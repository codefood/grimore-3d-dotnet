using System;
using System.Linq;
using Godot;

namespace Grimore.Entities;

public partial class Door : Node3D, IInteractable
{
    private MeshInstance3D _doorMesh;
    private CollisionShape3D _collision;
    public event Action<int> LoadLevel;
    private bool IsOpen { get; set; }
    public int DoorIndex { get; set; }

    public void RotateMe()
    {
        _doorMesh.RotateY(Mathf.Pi / 2);
        _collision.RotateY(Mathf.Pi / 2);
    }

    public override void _Ready()
    {
        _doorMesh = GetChild<MeshInstance3D>(0);
        _collision = GetChildren().OfType<CollisionShape3D>().First();
        RotateIfOpen();
    }

    private void RotateIfOpen()
    {
        if (IsOpen) RotateMe();
    }

    public bool Interact(Player player)
    {
        if (!IsOpen)
        {
            if (player.Keys <= 0) return false;

            player.Keys--;
            GD.Print($"{Name} is opening, {player.Keys} keys left");
            Open();
        }
        if (IsOpen)
        {
            LoadLevel!.Invoke(DoorIndex);
        }

        return true;
    }

    public void Open()
    {
        IsOpen = true;
        RotateIfOpen();
    }
}