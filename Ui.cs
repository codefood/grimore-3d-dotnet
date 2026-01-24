using System.Linq;
using Godot;
using Grimore.Entities;
using Grimore.Entities.Quests;

namespace Grimore;

public partial class Ui : Node2D
{
	private Label _fpsControl;

	public void UpdateCurrentTurn(IActor actor) =>
		GetNode<Label>("CurrentTurn")
				.Text = $"Current Turn: {actor.Name}";

	public void UpdateQuest(Quest quest) =>
		FindChildren("CurrentQuest")
				.OfType<Label>()
				.First()
				.Text = "Current Quest:\n" + 
						string.Join('\n', 
							quest.Requirements.Select(x => x.DisplayText)) + "\n" + 
						(quest.Complete ? "COMPLETE" : "");

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
		GetNode<Button>("ChangeCamera").Pressed += () => ToggleCamera?.Invoke();
		GetNode<Button>("MenuButton").Pressed += () =>
		{
			var menu = GetNode<TabContainer>("Menu");
			ToggleVisibility(menu);
			GameState.Toggle();
		};
		
		States.Ended.OnEnter += () => GetNode<Control>("GameOver").Show();
		States.Playing.OnEnter += () => GetNode<Control>("GameOver").Hide();

		_fpsControl = GetNode<Label>("FPS");
	}

	private static void ToggleVisibility(Container node) => 
		node.Visible = !node.Visible;

	public override void _Process(double delta)
	{
		base._Process(delta);
		_fpsControl.Text = Mathf.RoundToInt(Engine.GetFramesPerSecond()).ToString();
	}

	private static void CloseSpellEditor(BoxContainer spellEditor, Button openSpellEditorButton)
	{
		ToggleVisibility(spellEditor);
		openSpellEditorButton.ReleaseFocus();
	}
}
