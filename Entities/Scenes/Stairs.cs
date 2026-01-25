using Godot;
using Grimore.Entities.Quests;

namespace Grimore.Entities.Scenes;

public partial class Stairs : StaticBody3D, IInteractable
{
	public bool Interact(Player player)
	{
		GameState.GameOver(true);
		return true;
	}
}
