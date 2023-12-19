using System.Linq;
using Godot;
using Godot.Collections;
using ToasterGame.scripts.levels;

namespace ToasterGame.scripts;

public partial class LevelSelection : Control { 
	/// <summary>
	/// Exported because i thought it's easier (spoiler, it's not)
	/// I'll leave it like this, but just define the levels here.
	/// </summary>
	[Export] private Dictionary<string, PackedScene> _levels = new() {
		{ "Tutorial", GD.Load<PackedScene>("res://scenes/levels/level_0.tscn")},
		{ "Level 1", GD.Load<PackedScene>("res://scenes/levels/level_1.tscn")},
		{ "Level 2", GD.Load<PackedScene>("res://scenes/levels/level_2.tscn")},
		{ "Level 3", GD.Load<PackedScene>("res://scenes/levels/level_3.tscn")}
	};
	
	public override void _Ready() {
		var save = SaveGame.LoadSave();
		var buttonGrid = GetNode<GridContainer>("LevelGridContainer");
		var lastCompleted = -1;
		var index = 0;
		var buttonScene = GD.Load<PackedScene>("res://scenes/blueprints/level_button.tscn");
		var totalButter = 0;
		foreach (var level in _levels) {
			var button = buttonScene.Instantiate<LevelButton>();
			buttonGrid.AddChild(button);
			var levelButter = save.TryGetValue(level.Value.ResourcePath.Split('/').Last(), out var value) ? (int) value : -1;
			totalButter += levelButter;
			// if levelButter -1 = not played, 0 = completed, 1 = completed with gold
			if (levelButter >= 0) {
				lastCompleted = index;
				button.State = levelButter + 2;
			} else if (lastCompleted == index - 1) {
				button.State = 1;
			} else {
				button.State = 0;
			}
			button.LevelName = level.Key;
			button.LevelScene = level.Value;
			index++;
		}

		if (totalButter >= _levels.Count) {
			var specialButton = GetNode<Button>("%SpecialLevelButton");
			specialButton.Visible = true;
			specialButton.Pressed += () => {
				GetTree().ChangeSceneToFile("res://scenes/levels/level_special.tscn");
			};
		}
		buttonGrid.GetChild<LevelButton>(0).GrabFocus();
		var backButton = GetNode<Button>("%ButtonBack");
		backButton.Pressed += OnBackButtonPressed;
	}

	public override void _Process(double delta) {
	}

	public override void _UnhandledKeyInput(InputEvent @event) {
		if (@event.IsActionPressed("pause")) {
			OnBackButtonPressed();
		}
	}

	private void OnLevelButtonPressed(int levelNumber) {
		GetTree().ChangeSceneToFile("res://scenes/levels/level_" + levelNumber + ".tscn");
	}
	
	private void OnBackButtonPressed() {
		GetTree().ChangeSceneToFile("res://scenes/menu.tscn");
	}
}