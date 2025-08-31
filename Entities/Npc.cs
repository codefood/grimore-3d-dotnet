using DialogueManagerRuntime;
using Godot;

namespace Grimore.Entities;

[GlobalClass]
public partial class Npc : Node3D, IActor
{
    public event IActor.OnActing Acting;
    public event IActor.OnDying Dying;
    
    public class NoopCommand(Npc actor) : Command(actor) { }
    
    public void StartTurn() => 
        Acting?.Invoke(new NoopCommand(this));

    public void TakeDamage() { }

    public void StartDialog() => 
        DialogueManager.ShowExampleDialogueBalloon(GD.Load<Resource>("res://assets/npc.dialogue"));
}