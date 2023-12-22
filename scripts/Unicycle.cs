using System;
using Godot;

namespace ToasterGame.scripts;

public partial class Unicycle : CharacterBody2D {
	[Export] public float NormalSpeed = 500.0f;
	[Export] public float Acceleration = 250.0f;
	[Export] public float MaxCharge = 5.0f;

	private Player _player;
	private Node2D _attachPoint;
	private AnimatedSprite2D _sprite;
	private AudioStreamPlayer2D _audioDriving;
	private int _frames = 5;
	private bool _isAttached;
	private float _speed;
	private float _charge;
	private float _maxPitch = 2;
	private float _minPitch = 0.5f;
	private float _pitchMultiplier = 2f;


	private float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public override void _Ready() {
		_speed = NormalSpeed;
		_charge = MaxCharge;
		_attachPoint = GetNode<Node2D>("PlayerAttachmentPoint");
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_audioDriving = GetNode<AudioStreamPlayer2D>("%AudioDriving");
	}

	public override void _PhysicsProcess(double delta) {
		if (!_isAttached) {
			if (_audioDriving.Playing) _audioDriving.Stop();
			return;
		}

		if (!_audioDriving.Playing) _audioDriving.Play();

		if (_charge <= 0) {
			_player.IsOnUnicycle = false;
			_player.CanMove = true;
			_audioDriving.Stop();
		}

		if (!_player.IsOnUnicycle) {
			_player = null;
			_isAttached = false;
			_audioDriving.Stop();
		}
		else {
			var velocity = Velocity;

			if (!IsOnFloor())
				velocity.Y += _gravity * (float)delta;

			var direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
			//set speed based on charge
			var speed = Mathf.Lerp(0, _speed, _charge / MaxCharge);
			GD.Print($"vel: {velocity.X}, speed: {speed}");
			velocity.X = (float)(direction != Vector2.Zero && Mathf.Abs(velocity.X) < speed
				? Math.Min(Mathf.MoveToward(velocity.X, speed, Acceleration * delta * direction.X), speed)
				: Math.Max(Mathf.MoveToward(velocity.X, 0, Acceleration * delta  * direction.X), 0));

			_player.GlobalPosition = _attachPoint.GlobalPosition;

			Velocity = velocity;

			//set pitch based on velocity
			_audioDriving.PitchScale = Mathf.Lerp(_minPitch, _maxPitch, Mathf.Abs(velocity.X) / speed);
			GD.Print($"vel: {velocity.X}, speed: {speed} pitch: {_audioDriving.PitchScale}");
			MoveAndSlide();

			if (velocity.X != 0.0f) {
				_charge -= (float)delta;
			}

			_sprite.Frame = _frames - Mathf.RoundToInt(_frames * (_charge / MaxCharge));
		}
	}

	private void OnEquipAreaBodyEntered(Node2D body) {
		GD.Print("Player entered equip area.");
		if (body is not Player player) return;
		_player = player;
		_player.GlobalPosition = _attachPoint.GlobalPosition;
		_player.IsOnUnicycle = true;
		_player.CanMove = false;
		_isAttached = true;
	}
}