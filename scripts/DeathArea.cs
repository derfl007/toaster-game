using Godot;

namespace ToasterGame.scripts;

public partial class DeathArea : Node2D {

	private AudioStreamPlayer2D _audioWater;
	
	public override void _Ready() {
		GetNode<Area2D>("Area2D").BodyEntered += OnArea2DBodyEntered;
		_audioWater = GetNode<AudioStreamPlayer2D>("%AudioWater");
	}

	private void OnArea2DBodyEntered(Node2D body) {
		if (body is not Player player) return;
		_audioWater.Play();
		for (var i = 0; i < player.MaxHealth; i++) {
			player.TakeDamage("Nobody likes soggy toast...");
		}
	}
}