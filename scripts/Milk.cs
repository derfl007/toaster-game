using Godot;

namespace ToasterGame.scripts;

public partial class Milk : Node2D {
	public override void _Ready() {
		GetNode<Area2D>("Area2D").BodyEntered += OnArea2DBodyEntered;
	}

	private void OnArea2DBodyEntered(Node2D body) {
		GD.Print("Milk collected");
		if (body is not Player player) return;
		player.Heal();
		QueueFree();
	}
}