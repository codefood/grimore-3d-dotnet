using Godot;
using System.Linq;
using Grimore;
using Grimore.Entities;

public partial class Ui : Node2D
{
    private Label fpsControl;

    public void UpdateCurrentTurn(IActor actor) =>
        GetNode<Label>("CurrentTurn")
            .Text = $"Current Turn: {actor.Name}";

    public void UpdateQuest(Quest quest) =>
        GetNode<Label>("CurrentQuest")
            .Text = "Current Quest:\n" + 
                    string.Join('\n', 
                    quest.Requirements.Select((x => x.DisplayText)));

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
        var changeCameraButton = GetNode<Button>("ChangeCamera");
        changeCameraButton.Pressed += () => ToggleCamera?.Invoke();
        
        States.Ended.OnEnter += () => GetNode<Control>("GameOver").Show();
        States.Playing.OnEnter += () => GetNode<Control>("GameOver").Hide();

        fpsControl = GetNode<Label>("FPS");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        fpsControl.Text = Mathf.RoundToInt(Engine.GetFramesPerSecond()).ToString();
    }

    private static void CloseSpellEditor(BoxContainer spellEditor, Button openSpellEditorButton)
    {
        spellEditor.Visible = !spellEditor.Visible;
        openSpellEditorButton.ReleaseFocus();
    }

}
