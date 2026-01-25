using Godot;
using System;
using Grimore;
using Grimore.Entities;
using Grimore.Entities.Quests;
using Grimore.FSM;
using Grimore.Loaders;

public partial class Game : Node3D
{
	private World World => GetChild<World>(0);
	
	public override void _Ready()
	{
		base._Ready();
		var mainMenu = new State<World>()
			.OnEnter(w =>
			{
				//add "new game", "load game", event handlers
			});

		var userInterface = new StateMachine<Ui>();
			// .State(new State<Ui>());
			// .DefaultState(ui =>
			// {
			// 	ui.UpdateQuest(quest);
			// 	ui.UpdateHealth(player.Health);
			// });

		var turner = new StateMachine<TurnManager>()
			.DefaultState(t => t.StartNextTurn());
		
		
		var paused = new State<World>()
			.OnEnter(w =>
			{

			});

		
		var player = new StateMachine<Player>()
			.State(new State<Player>()
				.OnEnter(p =>
				{
					//set/reset player variables
					p.Keys = 0; 
					p.Health = 3;
				}));

		var quest = new StateMachine<Quest>();
		
		var playing = new State<World>()
			.OnEnter(w =>
			{
				//load level - a lot of this can be children
				LevelLoader.Load(w, 1);
				QuestLoader.Load(w);
				TurnLoader.Load(w);
				
				w.Camera.SetMode(Camera.Mode.Isometric);
			})
			//player is a child state, and it resolves its instance from the parent (world)
			.AddChild(
				player,
				w => w.Player,
				(w, p) => p.HealthChanged += w.Interface.UpdateHealth
			)
			.AddChild(
				userInterface, 
				w => w.Interface,
				(w, u) => u.ToggleCamera += () => ToggleCamera(w.Camera))
			.AddChild(
				quest, 
				w => w.Quest,
				(w, q) => Quest.OnUpdate += () => w.Interface.UpdateQuest(w.Quest))
			.AddChild(turner, w => w.Turner);

		var gameState = new StateMachine<World>()
			//TODO: .State(mainMenu)
			.State(playing)
			.State(paused);

		gameState.Start(World);
	}

	private void ToggleCamera(Camera camera) =>
		camera.SetMode(camera.CurrentMode == Camera.Mode.Isometric
			? Camera.Mode.ThirdPerson
			: Camera.Mode.Isometric);
}
