using Godot;

namespace ToasterGame.scripts;

public partial class Settings : PanelContainer {

	private Button _buttonExit;
	private HSlider _masterSlider;
	private HSlider _musicSlider;
	private HSlider _sfxSlider;
	
	private float _maxVolumeDb = 6;
	private float _minVolumeDb = -72;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		_buttonExit = GetNode<Button>("%ButtonExit");
		_masterSlider = GetNode<HSlider>("%MasterSlider");
		_musicSlider = GetNode<HSlider>("%MusicSlider");
		_sfxSlider = GetNode<HSlider>("%SfxSlider");
		
		_masterSlider.Value = Mathf.InverseLerp(_minVolumeDb, _maxVolumeDb, AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex("Master"))) * 100;
		_musicSlider.Value = Mathf.InverseLerp(_minVolumeDb, _maxVolumeDb, AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex("Music"))) * 100;
		_sfxSlider.Value = Mathf.InverseLerp(_minVolumeDb, _maxVolumeDb, AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex("SFX"))) * 100;
		
		_buttonExit.Pressed += () => GetTree().ChangeSceneToFile("res://scenes/menu.tscn");
		_masterSlider.ValueChanged += OnMasterSliderChanged;
		_musicSlider.ValueChanged += OnMusicSliderChanged;
		_sfxSlider.ValueChanged += OnSfxSliderChanged;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}
	
	private void OnMasterSliderChanged(double value) {
		var volumeDb = Mathf.Lerp(_minVolumeDb, _maxVolumeDb, value / 100);
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), (float)volumeDb);
	}
	
	private void OnMusicSliderChanged(double value) {
		var volumeDb = Mathf.Lerp(_minVolumeDb, _maxVolumeDb, value / 100);
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Music"), (float)volumeDb);
	}
	
	private void OnSfxSliderChanged(double value) {
		var volumeDb = Mathf.Lerp(_minVolumeDb, _maxVolumeDb, value / 100);
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("SFX"), (float)volumeDb);
	}
}