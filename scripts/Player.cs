using System;
using Godot;

namespace ToasterGame.scripts;

public partial class Player : CharacterBody2D {

	[Export] public float NormalSpeed = 300.0f;
	[Export] public float NormalJumpVelocity = -400.0f;
	[Export] public int MaxHealth = 3;

	[Signal]
	public delegate void UpdateButterCountEventHandler(int butter);

	[Signal]
	public delegate void UpdateHealthEventHandler(int health);

	[Signal]
	public delegate void PlayerDiedEventHandler(string reason);

	public bool IsOnUnicycle;
	public bool CanMove = true;
	public int Butter;

	private int _health;
	private float _speed;
	private float _jumpVelocity;
	private bool _dead;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	private float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	private AnimatedSprite2D _sprite;
	private HudUi _hudUi;
	private Camera2D _camera;

	public override void _Ready() {
		_health = MaxHealth;
		_speed = NormalSpeed;
		_jumpVelocity = NormalJumpVelocity;
		_sprite = GetNode<AnimatedSprite2D>("DefaultCollision/Sprite");
		_camera = GetNode<Camera2D>("Camera2D");
		_hudUi = GetNode<HudUi>("%HudUi");
		EmitSignal(SignalName.UpdateHealth, _health);
		CalculateCameraLimits();
	}

	public override void _PhysicsProcess(double delta) {
		var velocity = Velocity;

		// add gravity
		if (!IsOnFloor())
			velocity.Y += _gravity * (float)delta;

		if (Input.IsActionPressed("jump") && IsOnUnicycle) {
			IsOnUnicycle = false;
			CanMove = true;
			velocity.Y = _jumpVelocity;
		}

		if (!_dead && CanMove) {
			// Handle Jump.
			if (Input.IsActionJustPressed("jump") && IsOnFloor()) {
				GD.Print("jump");
				velocity.Y = _jumpVelocity;
			}

			// Get the input direction and handle the movement/deceleration.
			var direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
			if (direction != Vector2.Zero) {
				velocity.X = direction.X * _speed;
			}
			else {
				velocity.X = Mathf.MoveToward(Velocity.X, 0, _speed);
			}
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	public void CalculateCameraLimits() {
		GD.Print("Calculating camera limits");
		var tileMap = GetNode<TileMap>("%Level/TileMap");
		var mapLimits = tileMap.GetUsedRect();
		var cellSize = tileMap.TileSet.TileSize;
		_camera.LimitLeft = mapLimits.Position.X * cellSize.X;
		_camera.LimitRight = (mapLimits.Position.X + mapLimits.Size.X) * cellSize.X;
		_camera.LimitTop = (mapLimits.Position.Y - 2) * cellSize.Y;
		_camera.LimitBottom = (mapLimits.Position.Y + mapLimits.Size.Y - 2) * cellSize.Y;
		var screenWidth = GetViewportRect().Size.X;
		// at resolution 1052 width, zoom should be 3x
		_camera.Zoom = new Vector2(screenWidth / 350, screenWidth / 350);
		GD.Print($"Set limits: {_camera.LimitLeft}, {_camera.LimitRight}, {_camera.LimitTop}, {_camera.LimitBottom}");
	}

	public void IncreaseToasts(int amount) {
		for (var i = 0; i < amount; i++) {
			TakeDamage("You burned to death :(");
		}
	}

	public void AddButter() {
		Butter++;
		EmitSignal(SignalName.UpdateButterCount, Butter);
	}

	/// <summary>
	/// Lowers health and updates the health counter and sprite.
	/// </summary>
	/// <param name="reason">The damage reason, used for the death screen</param>
	public void TakeDamage(string reason) {
		if (_dead) return;
		_health--;
		_sprite.Frame = MaxHealth - _health;
		EmitSignal(SignalName.UpdateHealth, _health);
		if (_health > 0) return;
		_dead = true;
		EmitSignal(SignalName.PlayerDied, $"[center]{reason}[/center]");
	}

	public void Heal() {
		if (_health >= MaxHealth) return;
		_health++;
		_sprite.Frame = MaxHealth - _health;
		EmitSignal(SignalName.UpdateHealth, _health);
	}
}