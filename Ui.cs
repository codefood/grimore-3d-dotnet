using Godot;
using System.Linq;
using Grimore;
using Grimore.Entities;

public partial class Ui : Node2D
{
    public void UpdateCurrentTurn(IActor actor) =>
        GetChildren()
                .OfType<Label>()
                .Single()
                .Text = $"Current Turn: {actor.Name}";

    public void UpdateHealth(int health)
    {
        health = int.Max(health, 0);
        var container = GetChildren().OfType<FlowContainer>()
            .Single();
        if (health == container.GetChildCount()) return;
        while(health < container.GetChildCount()) container.RemoveChild(container.GetChild(0));
        while (health > container.GetChildCount()) container.AddChild(new ColorRect()
        {
            Color = Color.FromHtml("FF0000"),
            Size = new Vector2(32, 32),
            CustomMinimumSize = new Vector2(32, 32)
        });
    }

    public delegate void OnToggleCamera();
    public event OnToggleCamera ToggleCamera;
    
    public override void _Ready()
    {
        var changeCameraButton = GetChildren().OfType<Button>().First(c => c.Name == "ChangeCamera");
        changeCameraButton.Pressed += () =>
        {
            ToggleCamera?.Invoke();
        };
        GameState.State.Ended.OnEnter += () =>
        {
            GetChildren().OfType<Control>().First(c => c.Name == "GameOver").Show();
        };
        GameState.State.Started.OnEnter += () =>
        {
            GetChildren().OfType<Control>().First(c => c.Name == "GameOver").Hide();
        };
        
    }
    
    private static void CloseSpellEditor(BoxContainer spellEditor, Button openSpellEditorButton)
    {
        spellEditor.Visible = !spellEditor.Visible;
        openSpellEditorButton.ReleaseFocus();
    }

}
