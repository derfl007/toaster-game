using Godot;
using Godot.Collections;

namespace ToasterGame.scripts;

public partial class LevelSelection : Control { 
	[Export] private Dictionary<string, PackedScene> _levels;
	public override void _Ready() {
		var buttonGrid = GetNode<GridContainer>("LevelGridContainer");
		foreach (var level in _levels) {
			var button = new Button();
			button.Text = level.Key;
			button.Pressed += () => {
				GetTree().ChangeSceneToPacked(level.Value);
			};
			buttonGrid.AddChild(button);
		}
		buttonGrid.GetChild<Button>(0).GrabFocus();
	}

	public override void _Process(double delta) {
	}

	private void OnLevelButtonPressed(int levelNumber) {
		GetTree().ChangeSceneToFile("res://scenes/levels/level_" + levelNumber + ".tscn");
	}
}