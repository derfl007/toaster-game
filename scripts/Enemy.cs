using Godot;

namespace ToasterGame.scripts;

public partial class Enemy : CharacterBody2D {
	[Export] private float _maxDistance = 200.0f;
	[Export] private float _movementSpeed = 100.0f;

	private float _movementDirection = -1.0f;
	private RayCast2D _rayCastDown;
	private RayCast2D _rayCastSide;
	private Sprite2D _sprite;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		_rayCastDown = GetNode<RayCast2D>("RayCastDown");
		_rayCastSide = GetNode<RayCast2D>("RayCastSide");
		_sprite = GetNode<Sprite2D>("DefaultCollision/Sprite");
		GetNode<Area2D>("%DamageArea").BodyEntered += OnDamageAreaEntered;
		GetNode<Area2D>("%CauseDamageArea").BodyEntered += OnCauseDamageAreaBodyEntered;
	}

	public override void _PhysicsProcess(double delta) {
		if (!_rayCastDown.IsColliding() || _rayCastSide.IsColliding()) {
			_movementDirection *= -1.0f;
			_sprite.FlipH = !_sprite.FlipH;
			_rayCastDown.TargetPosition = _rayCastDown.TargetPosition.Reflect(Vector2.Down);
			_rayCastSide.TargetPosition = _rayCastSide.TargetPosition.Reflect(Vector2.Down);
		}

		var velocity = Velocity;
		velocity.X = _movementSpeed * _movementDirection;
		Velocity = velocity;
		MoveAndSlide();
	}

	private void OnDamageAreaEntered(Node2D body) {
		QueueFree();
	}

	private static void OnCauseDamageAreaBodyEntered(Node2D body){
		if (body is Player player) {
			player.TakeDamage("You got stabbed [font_size=8]\n(and also, uhm, burned... the knife was hot okay)[/font_size]");
		}
	}
}