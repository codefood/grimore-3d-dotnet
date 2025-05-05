using System.Linq;
using Godot;

namespace Grimore;

public partial class World : Node3D
{
	private readonly WorldManager _worldManager = new();
	public TurnManager TurnManager;
	
	private Timer _timer;
	private Node Interface => GetChildren().First(x => x.Name == "UI");

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
		
		_worldManager.LoadLevel(this, Levels.One);
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
}