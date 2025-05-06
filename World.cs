using System.Linq;
using Godot;

namespace Grimore;

public partial class World : Node3D
{
	public const int TileSize = 2;
	private readonly LevelLoader _levelLoader = new();
	public TurnManager TurnManager;
	
	private Timer _timer;
	private Node Interface => GetChildren().First(x => x.Name == "UI");
	private Player Player => GetChildren().OfType<Player>().First();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TurnManager = new(this);
		
		TurnManager.OnTurnStart += actor => 
			Interface
					.GetChildren()
					.OfType<Label>()
					.Single()
					.Text = $"Current Turn: {actor.Name}";

		var openSpellEditorButton = Interface.GetChildren().OfType<Button>().First();
		var spellEditor = Interface.GetChildren().OfType<BoxContainer>().First(c => c.Name == "SpellEditor");
		openSpellEditorButton.Pressed += () =>
		{
			CloseSpellEditor(spellEditor, openSpellEditorButton);
		};

		foreach (var button in spellEditor.GetChildren().OfType<OptionButton>())
		{
			button.ItemSelected += x =>
			{
				Player.SpellColor = button.GetItemText((int)x);
				CloseSpellEditor(spellEditor, openSpellEditorButton);
			};
		}
		
		_levelLoader.Load(this, Levels.One);
	}

	private static void CloseSpellEditor(BoxContainer spellEditor, Button openSpellEditorButton)
	{
		spellEditor.Visible = !spellEditor.Visible;
		openSpellEditorButton.ReleaseFocus();
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
}