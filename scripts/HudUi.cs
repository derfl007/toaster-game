using System.Linq;
using Godot;

namespace ToasterGame.scripts;

/// <summary>
/// Kinda handles the whole level a bit. Mainly meant for UI though
/// </summary>
public partial class HudUi : Control {
	[Export] private float _panelFadeDuration = 1;

	private Label _butterLabel;
	private Label _healthLabel;
	private Panel _deathPanel;
	private Panel _winPanel;
	private Panel _pausePanel;
	private Player _player;
	private Button _buttonWinNext;
	private Button _buttonDeathBack;
	private Button _buttonDeathRestart;
	private Button _buttonWinBack;
	private Button _buttonPauseRestart;
	private Button _buttonPauseBack;
	private Button _buttonPauseResume;
	private Area2D _winArea;

	private bool _showDeathPanel;
	private bool _showWinPanel;
	private bool _showPausePanel;
	private float _panelTimer;

	public override void _Ready() {
		_butterLabel = GetNode<Label>("%LabelButterCount");
		_healthLabel = GetNode<Label>("%LabelHealth");
		_winPanel = GetNode<Panel>("%PanelWin");
		_deathPanel = GetNode<Panel>("%PanelDeath");
		_pausePanel = GetNode<Panel>("%PanelPause");
		_player = GetNode<Player>("%Player");
		_buttonDeathRestart = GetNode<Button>("%ButtonDeathRestart");
		_buttonDeathBack = GetNode<Button>("%ButtonDeathBack");
		_buttonWinBack = GetNode<Button>("%ButtonWinBack");
		_buttonWinNext = GetNode<Button>("%ButtonWinNext");
		_buttonPauseRestart = GetNode<Button>("%ButtonPauseRestart");
		_buttonPauseBack = GetNode<Button>("%ButtonPauseBack");
		_buttonPauseResume = GetNode<Button>("%ButtonPauseResume");
		_winArea = GetNode<Area2D>("%WinJam/Area2D");
		
		_deathPanel.Modulate = new Color(1, 1, 1, 0);
		_deathPanel.Visible = false;
		_winPanel.Modulate = new Color(1, 1, 1, 0);
		_winPanel.Visible = false;
		_pausePanel.Visible = false;

		_player.UpdateButterCount += OnUpdateButterCount;
		_player.UpdateHealth += OnUpdateHealth;
		_player.PlayerDied += OnPlayerDied;
		OnUpdateHealth(_player.MaxHealth); // in case _player is not ready yet

		_buttonDeathRestart.Pressed += OnButtonRestartPressed;
		_buttonDeathBack.Pressed += () => OnButtonBackPressed(false);
		_buttonWinBack.Pressed += () => OnButtonBackPressed(true);
		_buttonDeathBack.Pressed += () => OnButtonBackPressed(false);
		_buttonPauseBack.Pressed += () => OnButtonBackPressed(false);
		_buttonPauseRestart.Pressed += OnButtonRestartPressed;
		_buttonPauseResume.Pressed += ShowOrHidePausePanel;

		_winArea.BodyEntered += OnPlayerWin;
	}

	public override void _Process(double delta) {
		if (!_showWinPanel && !_showDeathPanel) return;
		_panelTimer += (float)delta;
		if (_showWinPanel) _winPanel.Modulate = new Color(1, 1, 1, _panelTimer / _panelFadeDuration);
		if (_showDeathPanel) _deathPanel.Modulate = new Color(1, 1, 1, _panelTimer / _panelFadeDuration);
		if (!(_panelTimer >= _panelFadeDuration)) return;
		_panelTimer = 0;
		_showDeathPanel = false;
		_showWinPanel = false;
		GetTree().Paused = true;
	}

	public override void _UnhandledKeyInput(InputEvent @event) {
		if (@event.IsActionPressed("pause")) {
			ShowOrHidePausePanel();
		}
	}

	private void ShowOrHidePausePanel() {
		_showPausePanel = !_showPausePanel;
		_pausePanel.Visible = _showPausePanel;
		GetTree().Paused = _showPausePanel;
		if (_showPausePanel) {
			_buttonPauseResume.GrabFocus();
		}
	}

	private void OnPlayerWin(Node2D body) {
		_showWinPanel = true;
		_winPanel.Visible = true;
		_buttonWinNext.GrabFocus();
	}

	private void OnButtonRestartPressed() {
		GetTree().Paused = false;
		GetTree().ReloadCurrentScene();
	}

	private void OnButtonBackPressed(bool won) {
		if (won) {
			SaveGame.SaveLevel(GetTree().CurrentScene.SceneFilePath.Split('/').Last(), _player.Butter);
		}

		GetTree().Paused = false;
		GetTree().ChangeSceneToFile("res://scenes/level_selection.tscn");
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
		_showDeathPanel = true;
		_deathPanel.Visible = true;
		_deathPanel.GetNode<RichTextLabel>("%LabelReason").Text = reason;
		_buttonDeathRestart.GrabFocus();
	}
}