using System.Linq;
using Godot;

namespace ToasterGame.scripts;

/// <summary>
/// Kinda handles the whole level a bit. Mainly meant for UI though
/// </summary>
public partial class HudUi : Control {
	[Export] private float _panelFadeDuration = 1;
	[Export] private PackedScene _nextLevel;
	[Export] public bool TrackKnifesInsteadOfButter;

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
	private AudioStreamPlayer _audioWin;
	private AudioStreamPlayer _audioLose;
	private RichTextLabel _knivesText;

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
		_buttonDeathRestart = GetNode<Button>("%ButtonDeathRestart");
		_buttonDeathBack = GetNode<Button>("%ButtonDeathBack");
		_buttonWinBack = GetNode<Button>("%ButtonWinBack");
		_buttonWinNext = GetNode<Button>("%ButtonWinNext");
		_buttonPauseRestart = GetNode<Button>("%ButtonPauseRestart");
		_buttonPauseBack = GetNode<Button>("%ButtonPauseBack");
		_buttonPauseResume = GetNode<Button>("%ButtonPauseResume");
		_player = GetNode<Player>("%Player");
		_winArea = GetNode<Area2D>("%WinJam/Area2D");
		_audioWin = GetNode<AudioStreamPlayer>("%AudioWin");
		_audioLose = GetNode<AudioStreamPlayer>("%AudioLose");

		_deathPanel.Modulate = new Color(1, 1, 1, 0);
		_deathPanel.Visible = false;
		_winPanel.Modulate = new Color(1, 1, 1, 0);
		_winPanel.Visible = false;
		_pausePanel.Visible = false;

		_player.UpdateButterCount += OnUpdateButterCount;
		_player.UpdateHealth += OnUpdateHealth;
		_player.PlayerDied += OnPlayerDied;
		OnUpdateHealth(_player.MaxHealth); // in case _player is not ready yet

		GetTree().Root.SizeChanged += _player.CalculateCameraLimits;

		_buttonDeathRestart.Pressed += OnButtonRestartPressed;
		_buttonDeathBack.Pressed += () => OnButtonBackPressed(false);
		_buttonWinBack.Pressed += () => OnButtonBackPressed(true);
		_buttonWinNext.Pressed += OnButtonWinNextPressed;
		_buttonDeathBack.Pressed += () => OnButtonBackPressed(false);
		_buttonPauseBack.Pressed += () => OnButtonBackPressed(false);
		_buttonPauseRestart.Pressed += OnButtonRestartPressed;
		_buttonPauseResume.Pressed += ShowOrHidePausePanel;

		_winArea.BodyEntered += OnPlayerWin;
	}

	private void OnButtonWinNextPressed() {
		GetTree().Paused = false;
		GetTree().ChangeSceneToPacked(_nextLevel);
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

	public override void _UnhandledInput(InputEvent @event) {
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
		_audioWin.Play();
		_showWinPanel = true;
		_winPanel.Visible = true;
		_buttonWinNext.GrabFocus();
		SaveGame.SaveLevel(GetTree().CurrentScene.SceneFilePath.Split('/').Last(), _player.Butter);
	}

	private void OnButtonRestartPressed() {
		GetTree().Paused = false;
		GetTree().ReloadCurrentScene();
	}

	private void OnButtonBackPressed(bool won) {
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
		_audioLose.Play();
		_showDeathPanel = true;
		_deathPanel.Visible = true;
		_deathPanel.GetNode<RichTextLabel>("%LabelReason").Text = reason;
		if (TrackKnifesInsteadOfButter) {
			var highScore = SaveGame.LoadSave().TryGetValue("level_special.tscn", out var value) ? (int)value : -1;
			if (highScore < _player.Butter) {
				SaveGame.SaveLevel("level_special.tscn", _player.Butter);
				highScore = _player.Butter;
			}
			_knivesText = _deathPanel.GetNode<RichTextLabel>("%LabelKnives");
			_knivesText.Visible = true;
			_knivesText.Text = $"[center]You collected {_player.Butter} knives\nHighscore: {highScore}[/center]";
			
		}
		_buttonDeathRestart.GrabFocus();
	}
}