using System;
using Godot;
using Grimore.Entities;
using Grimore.Entities.Quests;
using Grimore.FSM;
using Grimore.Loaders;

namespace Grimore;

public partial class Game : Node3D
{
	private World World => GetChild<World>(0);

	private MainMenu MenuScene = ResourceLoader.Load<PackedScene>(Paths.MainMenu).Instantiate<MainMenu>();
	
	public override void _Ready()
	{
		base._Ready();


		var userInterface = new StateMachine<Ui>()
			.Default(
				x => x.ToggleCamera += DoCameraToggle(),
				x => x.ToggleCamera -= DoCameraToggle());

		var turner = new StateMachine<TurnManager>()
			.Default(t =>
			{
				TurnManager.OnTurnStart += World.Quest.Check;
				TurnManager.OnPlayerMove += World.Quest.Check;
				TurnManager.OnInteractionSuccess += World.Quest.Check;
				TurnManager.OnTurnStart += World.Interface.UpdateCurrentTurn;
				t.StartNextTurn();
			},
			_ =>
			{
				TurnManager.OnTurnStart -= World.Quest.Check;
				TurnManager.OnPlayerMove -= World.Quest.Check;
				TurnManager.OnInteractionSuccess -= World.Quest.Check;
				TurnManager.OnTurnStart -= World.Interface.UpdateCurrentTurn;
			});

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
					p.HealthChanged += World.Interface.UpdateHealth;
				})
				.OnExit(p => p.HealthChanged -= World.Interface.UpdateHealth)
			);

		var quest = new StateMachine<Quest>()
			.Default(q =>
			{
				QuestLoader.Load(World);
				Quest.OnUpdate += DoQuestUpdate(q);
			}, 
				q => Quest.OnUpdate -= DoQuestUpdate(q)); //probably don't need to unregister this, it's a singleton really

		var playing = new State<World>()
				.OnEnter(w =>
				{
					//load level - a lot of this can be children
					LevelLoader.Load(w, 1);

					w.Camera.SetMode(Camera.Mode.Isometric);

				})
				//player is a child state, and it resolves its instance from the parent (world)
				.AddChild(userInterface, World.Interface)
				.AddChild(quest, World.Quest)
				.AddChild(player, World.Player)
				.AddChild(turner, World.Turner)
			//.AddChild(inputManager, World.InputManager);
			;
		
		
		var mainMenu = new State<MainMenu>()
			.AddChild(new StateMachine<MainMenu>()
				.Default(x =>
				{
					x.NewGame += 
				}), MenuScene);

		
		var gameState = new StateMachine<World>()
			.State(mainMenu)
			.State(playing)
			.State(paused);

		gameState.Start(World);
	}

	private Action DoQuestUpdate(Quest q)
	{
		return () => World.Interface.UpdateQuest(q);
	}

	private Ui.OnToggleCamera DoCameraToggle() => () => ToggleCamera(World.Camera);

	private void ToggleCamera(Camera camera) =>
		camera.SetMode(camera.CurrentMode == Camera.Mode.Isometric
			? Camera.Mode.ThirdPerson
			: Camera.Mode.Isometric);
}
