using Godot;

namespace ToasterGame.scripts;

public partial class Unicycle : CharacterBody2D {
	[Export] public float NormalSpeed = 500.0f;
	[Export] public float MaxCharge = 5.0f;

	private Player _player;
	private Node2D _attachPoint;
	private AnimatedSprite2D _sprite;
	private int _frames = 5;
	private bool _isAttached;
	private float _speed;
	private float _charge;


	private float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public override void _Ready() {
		_speed = NormalSpeed;
		_charge = MaxCharge;
		_attachPoint = GetNode<Node2D>("PlayerAttachmentPoint");
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	public override void _PhysicsProcess(double delta) {
		if (!_isAttached) return;


		if (_charge <= 0) {
			_player.IsOnUnicycle = false;
			_player.CanMove = true;
		}

		if (!_player.IsOnUnicycle) {
			_player = null;
			_isAttached = false;
		}
		else {
			var velocity = Velocity;

			if (!IsOnFloor())
				velocity.Y += _gravity * (float)delta;

			var direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
			if (direction != Vector2.Zero) {
				velocity.X = direction.X * _speed;
			}
			else {
				velocity.X = Mathf.MoveToward(Velocity.X, 0, _speed);
			}

			_player.GlobalPosition = _attachPoint.GlobalPosition;

			Velocity = velocity;
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