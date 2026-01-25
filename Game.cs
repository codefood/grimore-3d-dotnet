using Godot;
using Grimore.Entities;
using Grimore.Entities.Quests;
using Grimore.FSM;
using Grimore.Loaders;

namespace Grimore;

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
            .Default(q => QuestLoader.Load(World));

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
            .AddChild(turner, World.Turner);

        var gameState = new StateMachine<World>()
            //TODO: .State(mainMenu)
            .State(playing)
            .State(paused);

        gameState.Start(World);
    }

    private Ui.OnToggleCamera DoCameraToggle() => () => ToggleCamera(World.Camera);

    private void ToggleCamera(Camera camera) =>
        camera.SetMode(camera.CurrentMode == Camera.Mode.Isometric
            ? Camera.Mode.ThirdPerson
            : Camera.Mode.Isometric);
}