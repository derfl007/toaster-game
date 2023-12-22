using Godot;

namespace ToasterGame.scripts;

public partial class Enemy : CharacterBody2D {
	[Export] private float _maxDistance = 200.0f;
	[Export] private float _movementSpeed = 100.0f;
  [Export(PropertyHint.MultilineText)] private string _customDamageText = "You got stabbed [font_size=8]\n(and also, uhm, burned... the knife was hot okay)[/font_size]";  
	
	private float _movementDirection = -1.0f;
	private RayCast2D _rayCastDown;
	private RayCast2D _rayCastSide;
	private Sprite2D _sprite;
	private Area2D _damageArea;
	private Area2D _causeDamageArea;
	private CollisionShape2D _causeDamageAreaCollisionShape;
	private AudioStreamPlayer2D _audioAttack;
	private AudioStreamPlayer2D _audioDie;
	private HudUi _hudUi;
	private float _deathTimer;
	private float _deathDuration = 2f;
	private bool _dead;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		_rayCastDown = GetNode<RayCast2D>("RayCastDown");
		_rayCastSide = GetNode<RayCast2D>("RayCastSide");
		_sprite = GetNode<Sprite2D>("Sprite");
		_damageArea = GetNode<Area2D>("%DamageArea");
		_causeDamageArea = GetNode<Area2D>("%CauseDamageArea");
		_damageArea.BodyEntered += OnDamageAreaEntered;
		_causeDamageArea.BodyEntered += OnCauseDamageAreaBodyEntered;
		_causeDamageAreaCollisionShape = _causeDamageArea.GetChild<CollisionShape2D>(0);
		_audioAttack = GetNode<AudioStreamPlayer2D>("%AudioAttack");
		_audioDie = GetNode<AudioStreamPlayer2D>("%AudioDie");
		_hudUi = GetParent().GetParent().GetNode<HudUi>("%HudUi");
	}

	public override void _Process(double delta) {
		if (!_dead) return;
		if (_deathTimer <= _deathDuration) {
			_deathTimer += (float)delta;
		}
		else {
			QueueFree();
		}
	}

	public override void _PhysicsProcess(double delta) {
		var velocity = Velocity;
		if (_dead) {
			velocity = Vector2.Down * _movementSpeed;
			Velocity = velocity;
			MoveAndSlide();
			return;
		}

		if (!_rayCastDown.IsColliding() || _rayCastSide.IsColliding()) {
			_movementDirection *= -1.0f;
			_sprite.FlipH = !_sprite.FlipH;
			_rayCastDown.TargetPosition = _rayCastDown.TargetPosition.Reflect(Vector2.Down);
			_rayCastSide.TargetPosition = _rayCastSide.TargetPosition.Reflect(Vector2.Down);
			_causeDamageAreaCollisionShape.Rotation += Mathf.Pi;
		}

		velocity.X = _movementSpeed * _movementDirection;
		Velocity = velocity;
		MoveAndSlide();
	}

	private void OnDamageAreaEntered(Node2D body) {
		if (body is not Player player || _dead) return;
		GD.Print($"Tracking Knifes: {_hudUi.TrackKnifesInsteadOfButter}");
		if (_hudUi.TrackKnifesInsteadOfButter) {
			player.AddButter();
		}
		_audioDie.Play();
		_dead = true;
	}

	private void OnCauseDamageAreaBodyEntered(Node2D body) {
		if (body is not Player player || _dead) return;
		_audioAttack.Play();
		player.TakeDamage(_customDamageText);
		_dead = true;
	}
}