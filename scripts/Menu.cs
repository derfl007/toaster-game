using Godot;

namespace ToasterGame.scripts;

public partial class Menu : Control {
	public override void _Ready() {
		GetNode<Button>("HBoxContainer/ButtonStart").GrabFocus();
	}

	public override void _Process(double delta) {
	}

	private void OnButtonStartPressed() {
		GetTree().ChangeSceneToFile("res://scenes/level_selection.tscn");
	}

	private void OnButtonQuitPressed() {
		GetTree().Quit();
	}
}