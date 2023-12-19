using Godot;

namespace ToasterGame.scripts;

public partial class Butter : Node2D {
	public override void _Ready() {
		GetNode<Area2D>("Area2D").BodyEntered += OnArea2DBodyEntered;
	}

	private void OnArea2DBodyEntered(Node2D body) {
		GD.Print("Butter collected");
		if (body is not Player player) return;
		player.AddButter();
		QueueFree();
	}
}