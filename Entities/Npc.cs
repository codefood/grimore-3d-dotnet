using DialogueManagerRuntime;
using Godot;

namespace Grimore.Entities;

[GlobalClass]
public partial class Npc : Node3D, IInteractable
{
    public bool Interact(Player player)
    {
        if (player == null) return false;
        DialogueManager.ShowExampleDialogueBalloon(GD.Load<Resource>(Paths.Dialog.Npc));
        return false;
    }
}