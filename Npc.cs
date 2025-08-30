using DialogueManagerRuntime;
using Godot;
using Grimore;

public partial class Npc : Node3D, IActor
{
    public event IActor.OnActing Acting;
    public event IActor.OnDying Dying;

    public class NoopCommand(Npc actor) : Command(actor) { }
    public void StartTurn()
    {
        Acting?.Invoke(new NoopCommand(this));
    }

    public void TakeDamage()
    {
        
    }

    public void StartDialog()
    {
        var dialog = DialogueManager.CreateResourceFromText("~ HelloWorld\nNPC: Hello world!\nHow are you doing?\n...Can we do multiple lines?");
        DialogueManager.ShowExampleDialogueBalloon(dialog);
    }
}
