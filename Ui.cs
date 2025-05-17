using Godot;
using System;
using System.Linq;
using Grimore;

public partial class Ui : Node2D
{
    public void UpdateCurrentTurn(IActor actor) =>
        GetChildren()
                .OfType<Label>()
                .Single()
                .Text = $"Current Turn: {actor.Name}";

    public delegate void OnChangeSpellColour(string colour);
    public event OnChangeSpellColour ChangeSpellColour;

    public delegate void OnToggleCamera();
    public event OnToggleCamera ToggleCamera;
    
    public override void _Ready()
    {
        var openSpellEditorButton = GetChildren().OfType<Button>().First(c => c.Name == "OpenGrimore");
        var changeCameraButton = GetChildren().OfType<Button>().First(c => c.Name == "ChangeCamera");
        var spellEditor = GetChildren().OfType<BoxContainer>().First(c => c.Name == "SpellEditor");
        openSpellEditorButton.Pressed += () =>
        {
            CloseSpellEditor(spellEditor, openSpellEditorButton);
        };
        changeCameraButton.Pressed += () =>
        {
            ToggleCamera?.Invoke();
        };

        foreach (var button in spellEditor.GetChildren().OfType<OptionButton>())
        {
            button.ItemSelected += x =>
            {
                ChangeSpellColour?.Invoke(button.GetItemText((int)x));
                
                CloseSpellEditor(spellEditor, openSpellEditorButton);
            };
        }
    }
    
    private static void CloseSpellEditor(BoxContainer spellEditor, Button openSpellEditorButton)
    {
        spellEditor.Visible = !spellEditor.Visible;
        openSpellEditorButton.ReleaseFocus();
    }

}
