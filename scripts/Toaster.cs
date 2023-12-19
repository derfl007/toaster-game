using Godot;

namespace ToasterGame.scripts;

public partial class Toaster : Node {
	private AnimatedSprite2D _sprite;
	private CollisionPolygon2D _blockerShape;
	private Player _toast;
	private Control _timingUi;
	private Area2D _goodTimingAreaRight;
	private Area2D _perfectTimingArea;
	private Area2D _goodTimingAreaLeft;
	private Area2D _leftEdgeArea;
	private Area2D _rightEdgeArea;
	private CharacterBody2D _timingBar;
	private Marker2D _timingBarSpawnPoint;

	private bool _isToasting = false;
	private float _timingBarDirection = -1f;
	private bool _canToast = true;
	private float _toastCooldownTime = 0.0f; // Time since last toasting.
	private bool _inGoodTimingArea = false;
	private bool _inPerfectTimingArea = false;
	
	[Export] private float _timingBarSpeed = 500f; // How long toasting lasts (in s).
	[Export] private float _toastCooldown = 0.1f; // How long until toasting can be used again (in s).
	[Export] private float _jumpBoost = -500.0f; // How much to boost the toast's jump velocity (in px/s).


	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		_sprite = GetNode<AnimatedSprite2D>("StaticBody2D/DefaultCollisionShape/MainSprite");
		_blockerShape = GetNode<CollisionPolygon2D>("StaticBody2D/BlockerCollisionShape");
		_timingUi = GetNode<Control>("%TimingUi");
		_timingUi.Visible = false;
		_goodTimingAreaLeft = GetNode<Area2D>("%GoodTimingAreaLeft");
		_perfectTimingArea = GetNode<Area2D>("%PerfectTimingArea");
		_goodTimingAreaRight = GetNode<Area2D>("%GoodTimingAreaRight");
		_leftEdgeArea = GetNode<Area2D>("%LeftEdgeArea");
		_rightEdgeArea = GetNode<Area2D>("%RightEdgeArea");
		_timingBar = GetNode<CharacterBody2D>("%TimingBar");
		_timingBarSpawnPoint = GetNode<Marker2D>("%TimingBarSpawnPoint");
		
		_goodTimingAreaLeft.BodyEntered += OnGoodAreaEntered;
		_goodTimingAreaLeft.BodyExited += OnGoodAreaExited;
		_perfectTimingArea.BodyEntered += OnPerfectAreaEntered;
		_perfectTimingArea.BodyExited += OnPerfectAreaExited;
		_goodTimingAreaRight.BodyEntered += OnGoodAreaEntered;
		_goodTimingAreaRight.BodyExited += OnGoodAreaExited;
		_leftEdgeArea.BodyEntered += FlipTimingBar;
		_rightEdgeArea.BodyEntered += FlipTimingBar;
		GD.Print($"Sprite {_sprite}");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		if (_sprite == null) return;
		_blockerShape.Disabled = _canToast;

		if (_isToasting) {
			_timingBar.Velocity = new Vector2( _timingBarSpeed * _timingBarDirection, 0);
			_timingBar.MoveAndSlide();
		}
		else {
			_sprite.Frame = 0;
			if (_toastCooldownTime >= _toastCooldown) {
				_toastCooldownTime = 0.0f;
				_canToast = true;
			}
			else {
				_toastCooldownTime += (float)delta;
			}	
		}
	}

	public override void _Input(InputEvent @event) {
		base._Input(@event);
		if (!@event.IsActionPressed("interact") || !_isToasting) return;
		if (_inPerfectTimingArea) {
			JumpOut(0);
		} else if (_inGoodTimingArea) {
			JumpOut(1);
		} else {
			JumpOut(2);
		}
	}

	private void JumpOut(int healthLost) {
		_isToasting = false;
		_canToast = false;
		_timingUi.Visible = false;
		_timingBar.Velocity = new Vector2(0, 0);
		_timingBar.GlobalPosition = _timingBarSpawnPoint.GlobalPosition;
		_inGoodTimingArea = false;
		_inPerfectTimingArea = false;
		_timingBarDirection = -1f;
		_toast.CanMove = true;
		_toast.Velocity = _toast.Velocity with { Y = _jumpBoost };
		_toast.IncreaseToasts(healthLost);
	}

	private void OnToasterBodyEntered(Node2D body) {
		if (body is not Player characterBody2D || !_canToast) return;
		_toast = characterBody2D;
		_isToasting = true;
		_timingUi.Visible = true;
		_sprite.Frame = 1;
		_toast.CanMove = false;
	}
	
	private void OnGoodAreaEntered(Node2D body) {
		if (body is not CharacterBody2D || !_isToasting) return;
		_inGoodTimingArea = true;
	}
	
	private void OnGoodAreaExited(Node2D body) {
		if (body is not CharacterBody2D || !_isToasting) return;
		_inGoodTimingArea = false;
	}
	
	private void OnPerfectAreaEntered(Node2D body) {
		if (body is not CharacterBody2D || !_isToasting) return;
		_inPerfectTimingArea = true;
	}
	
	private void OnPerfectAreaExited(Node2D body) {
		if (body is not CharacterBody2D || !_isToasting) return;
		_inPerfectTimingArea = false;
	}
	
	private void FlipTimingBar(Node2D body) {
		_timingBarDirection *= -1;
	}
}
