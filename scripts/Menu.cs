using Godot;

namespace ToasterGame.scripts;

public partial class Menu : Control {

	private Button _buttonStart;
	private Button _buttonSettings;
	private Button _buttonExit;
	public override void _Ready() {
		_buttonStart = GetNode<Button>("%ButtonStart");
		_buttonSettings = GetNode<Button>("%ButtonSettings");
		_buttonExit = GetNode<Button>("%ButtonExit");
		_buttonStart.GrabFocus();
		_buttonStart.Pressed += OnButtonStartPressed;
		_buttonSettings.Pressed += OnSettingsButtonPressed;
		_buttonExit.Pressed += OnButtonExitPressed;
	}

	public override void _Process(double delta) {
	}

	private void OnButtonStartPressed() {
		GetTree().ChangeSceneToFile("res://scenes/level_selection.tscn");
	}
	
	private void OnSettingsButtonPressed() {
		GetTree().ChangeSceneToFile("res://scenes/settings.tscn");
	}

	private void OnButtonExitPressed() {
		GetTree().Quit();
	}
}