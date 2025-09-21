using Godot;

namespace Grimore.Entities;

public partial class Wall : StaticBody3D, IInteractable
{
    private bool _semiTransparent;

    public bool SemiTransparent
    {
        get => _semiTransparent;
        set
        {
            _semiTransparent = value;
            if (!_semiTransparent) return;
            var mesh = GetNode<MeshInstance3D>("Cube");
            var sm = mesh.GetActiveMaterial(0) as StandardMaterial3D;
            if (sm == null) return;
            sm.AlbedoColor = new Color(sm.AlbedoColor.R, sm.AlbedoColor.G, sm.AlbedoColor.B, 0.2f);
        }
    }
    public bool Interact(Player player) => false;
}