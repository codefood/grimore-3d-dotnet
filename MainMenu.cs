using Godot;
using System;
using System.Linq;

public partial class MainMenu : Node2D
{
    public event Action NewGame;

    public override void _Ready()
    {
        base._Ready();
        FindChildren("NewGame").OfType<Button>().First().Pressed += () => NewGame();
    }
}
