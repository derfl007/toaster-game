using Godot;

namespace ToasterGame.scripts;

public partial class Camera : Camera2D {
	private Node2D _target;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		_target = GetNode<Node2D>("%Player/CameraTarget");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		if (_target == null) return;
		GlobalPosition = _target.GlobalPosition;
	}
}