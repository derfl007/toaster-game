using Godot;

namespace ToasterGame.scripts;

/**
 *  This is the main game script. It handles scene loading, pausing, etc.
 */
public partial class Game : Node {
	public levels.Level CurrentLevel;
	public GameState GameState = GameState.Menu;

	public override void _Ready() {
	}

	public override void _Process(double delta) {
	}
}

public enum GameState {
	Playing,
	Paused,
	Menu
}