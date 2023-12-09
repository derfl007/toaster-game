using Godot;

namespace ToasterGame.scripts;

public partial class Toaster : Node {
	private AnimatedSprite2D _sprite;
	private CollisionPolygon2D _blockerShape;
	private Player _toast;

	private bool _isToasting = false;
	private bool _canToast = true;
	private float _toastTime = 0.0f; // Time since toasting started.
	private float _toastCooldownTime = 0.0f; // Time since last toasting.
	
	[Export] private float _toastDuration = 3.0f; // How long toasting lasts (in s).
	[Export] private float _toastCooldown = 5.0f; // How long until toasting can be used again (in s).
	[Export] private float _jumpBoost = -500.0f; // How much to boost the toast's jump velocity (in px/s).


	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		_sprite = GetNode<AnimatedSprite2D>("StaticBody2D/DefaultCollisionShape/MainSprite");
		_blockerShape = GetNode<CollisionPolygon2D>("StaticBody2D/BlockerCollisionShape");
		GD.Print($"Sprite {_sprite}");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		if (_sprite == null) return;
		_blockerShape.Disabled = _canToast;
		
		if (_isToasting) {
			_toast.CanMove = false;
			_sprite.Frame = 1;
			_toastTime += (float)delta;
			if (!(_toastTime >= _toastDuration)) return;
			_isToasting = false;
			_canToast = false;
			_toastTime = 0.0f;
			// throw _toast up and right
			_toast.CanMove = true;
			_toast.Velocity = new Vector2(0, _jumpBoost);
			_toast.IncreaseToasts(1);
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

	private void OnToasterBodyEntered(Node2D body) {
		if (body is not Player characterBody2D || !_canToast) return;
		_toast = characterBody2D;
		_isToasting = true;
	}
}
