using System.Collections.Generic;
using Godot;

namespace grimore3ddotnet;

public static class Actions
{
    private const string Right = "right";
    private const string Up = "up";
    private const string Down = "down";
    private const string Left = "left";
    public const string Act = "act";
    public const string Clear = "clear";
    
    public static readonly Dictionary<string, Vector2> Directions = new()
    {
        { Right, Vector2.Right },
        { Up, Vector2.Up },
        { Down, Vector2.Down },
        { Left, Vector2.Left },
    };
}