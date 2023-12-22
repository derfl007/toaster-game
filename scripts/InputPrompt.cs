using Godot;
using NathanHoad;

namespace ToasterGame.scripts;

public partial class InputPrompt : Control {

	[Export] private Texture2D _kbmTexture;
	[Export] private Texture2D _xboxTexture;
	[Export] private Texture2D _psTexture;
	[Export] private Texture2D _touchTexture;

	private TextureRect _kbmTextureRect;
	private TextureRect _xboxTextureRect;
	private TextureRect _psTextureRect;
	private TouchScreenButton _touchScreenButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		_kbmTextureRect = GetNode<TextureRect>("%KBM");
		_xboxTextureRect = GetNode<TextureRect>("%Xbox");
		_psTextureRect = GetNode<TextureRect>("%PS");
		_touchScreenButton = GetNode<TouchScreenButton>("%Touch");

		_kbmTextureRect.Texture = _kbmTexture;
		_xboxTextureRect.Texture = _xboxTexture;
		_psTextureRect.Texture = _psTexture;
		_touchScreenButton.TextureNormal = _touchTexture;

		ChangeTextures(InputHelper.GuessDeviceName());
		InputHelper.Instance.Connect("device_changed", Callable.From((string device, int id) => ChangeTextures(device, id)));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}

	private void ChangeTextures(string device, int id = 0) {
		GD.Print($"{GetParent().Name} - New device: {device}");
		_kbmTextureRect.Visible = device == InputHelper.DEVICE_KEYBOARD;
		_xboxTextureRect.Visible = device == InputHelper.DEVICE_XBOX_CONTROLLER;
		_psTextureRect.Visible = device == InputHelper.DEVICE_PLAYSTATION_CONTROLLER;
	}
}