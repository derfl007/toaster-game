using Godot;

namespace ToasterGame.scripts;

public partial class DeathArea : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<Area2D>("Area2D").BodyEntered += OnArea2DBodyEntered;
	}

	private static void OnArea2DBodyEntered(Node2D body) {
		if (body is not Player player) return;
		for (var i = 0; i < player.MaxHealth; i++) {
			player.TakeDamage("Nobody likes soggy toast...");
		}
	}
}