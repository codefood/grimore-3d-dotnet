using DialogueManagerRuntime;
using Godot;

namespace Grimore.Entities;

[GlobalClass]
public partial class Npc : Node3D, IActor, IInteractable
{
    public class NoopCommand(Npc actor) : Command(actor) { }
    
    public void StartTurn() => 
        IActor.InvokeActing(new NoopCommand(this));

    public bool PlayerInteraction()
    {
        DialogueManager.ShowExampleDialogueBalloon(GD.Load<Resource>("res://assets/npc.dialogue"));
        return false;
    }
}