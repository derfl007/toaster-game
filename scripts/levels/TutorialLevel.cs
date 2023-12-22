using Godot;

namespace ToasterGame.scripts.levels;

public partial class TutorialLevel : Level {
	public override void _Ready() {
		base._Ready();
		TileMap = GetNode<TileMap>("TileMap");
	}

	public override void _Process(double delta) {
	}
}