using Godot;

namespace ToasterGame.scripts;

/// <summary>
/// Kinda handles the whole level a bit. Mainly meant for UI though
/// </summary>
public partial class HudUi : Control {
	[Export] private float _deathPanelFadeDuration = 1;
	
	private Label _butterLabel;
	private Label _healthLabel;
	private Panel _deathPanel;
	private Panel _winPanel;
	private Player _player;

	private bool _showDeathPanel;
	private float _deathPanelTimer;

	public override void _Ready() {
		GetTree().Paused = false;
		_butterLabel = GetNode<Label>("%LabelButterCount");
		_healthLabel = GetNode<Label>("%LabelHealth");
		_deathPanel = GetNode<Panel>("%PanelDeath");
		_deathPanel.Modulate = new Color(1, 1, 1, 0);
		_winPanel = GetNode<Panel>("%PanelWin");
		_player = GetNode<Player>("%Player");
		_player.UpdateButterCount += OnUpdateButterCount;
		_player.UpdateHealth += OnUpdateHealth;
		_player.PlayerDied += OnPlayerDied;
		OnUpdateHealth(_player.MaxHealth); // in case _player is not ready yet
		var buttonRestart = GetNode<Button>("%ButtonRestart");
		var buttonBack = GetNode<Button>("%ButtonBack");
		var buttonBackWin = GetNode<Button>("%ButtonBackWin");
		var buttonNext = GetNode<Button>("%ButtonNext");
		buttonRestart.Pressed += OnButtonRestartPressed;
		buttonBack.Pressed += () => OnButtonBackPressed(false);
		buttonBackWin.Pressed += () => OnButtonBackPressed(true);
		GetNode<Area2D>("%WinJam/Area2D").BodyEntered += (body) => {
			_winPanel.Visible = true;
			GetTree().Paused = true;
			buttonNext.GrabFocus();
		};
	}

	private void OnButtonRestartPressed() {
		GetTree().Paused = false;
		GetTree().ReloadCurrentScene();
	}

	private void OnButtonBackPressed(bool won) {
		if (won) {
			SaveGame.SaveLastLevel(GetTree().CurrentScene.SceneFilePath, _player.Butter);
		}
		GetTree().Paused = false;
		GetTree().ChangeSceneToFile("res://scenes/level_selection.tscn");
	}

	public override void _Process(double delta) {
		if (!_showDeathPanel) return;
		_deathPanelTimer += (float) delta;
		_deathPanel.Modulate = new Color(1, 1, 1, _deathPanelTimer / _deathPanelFadeDuration);
		if (!(_deathPanelTimer >= _deathPanelFadeDuration)) return;
		_deathPanelTimer = 0;
		_showDeathPanel = false;
		GetTree().Paused = true;
	}

	private void OnUpdateButterCount(int count) {
		GD.Print("Updating butter count");
		_butterLabel.Text = count.ToString();
	}

	private void OnUpdateHealth(int health) {
		_healthLabel ??= GetNode<Label>("%LabelHealth");
		_healthLabel.Text = health.ToString();
	}

	private void OnPlayerDied(string reason) {
		_deathPanel.Visible = true;
		_showDeathPanel = true;
		_deathPanel.GetNode<RichTextLabel>("%LabelReason").Text = reason;
		_deathPanel.GetNode<Button>("%ButtonRestart").GrabFocus();
	}
}