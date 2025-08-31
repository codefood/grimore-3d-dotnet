using Godot;
using System.Linq;
using Grimore.Entities;

public partial class Ui : Node2D
{
    public void UpdateCurrentTurn(IActor actor) =>
        GetChildren()
                .OfType<Label>()
                .Single()
                .Text = $"Current Turn: {actor.Name}";

    public delegate void OnToggleCamera();
    public event OnToggleCamera ToggleCamera;
    
    public override void _Ready()
    {
        var changeCameraButton = GetChildren().OfType<Button>().First(c => c.Name == "ChangeCamera");
        changeCameraButton.Pressed += () =>
        {
            ToggleCamera?.Invoke();
        };
    }
    
    private static void CloseSpellEditor(BoxContainer spellEditor, Button openSpellEditorButton)
    {
        spellEditor.Visible = !spellEditor.Visible;
        openSpellEditorButton.ReleaseFocus();
    }

}
