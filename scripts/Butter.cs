using Godot;

namespace ToasterGame.scripts;

public partial class Butter : Node2D {
	public override void _Ready() {
	}

	public override void _Process(double delta) {
	}
	
	private void OnArea2DBodyEntered(Node2D body) {
		GD.Print("Butter collected");
		if (body is not Player player) return;
		player.AddButter();
		QueueFree();
	}
}