using Godot;

namespace ToasterGame.scripts;

public partial class LevelButton : HBoxContainer {
	public string LevelName {
		get =>  _button.Text;
		set => _button.Text = value;
	}

	public PackedScene LevelScene {
		set => _button.Pressed += () => {
			GetTree().ChangeSceneToPacked(value);
		};
	}

	/// 0 = locked, 1 = unlocked, 2 = completed, 3 = completed with gold
	public int State {
		set {
			switch (value) {
				case 0:
					_button.Disabled = true;
					break;
				case 1:
					_button.Disabled = false;
					break;
				case 2:
					_trophy.Visible = true;
					break;
				case 3:
					_trophyGold.Visible = true;
					break;
			}
		}
	}

	private TextureRect _trophy;
	private TextureRect _trophyGold;
	private Button _button;
	
	public override void _Ready() {
		_trophy = GetNode<TextureRect>("%TrophyNormal");
		_trophyGold = GetNode<TextureRect>("%TrophyGold");
		_button = GetNode<Button>("%Button");
	}

	public new void GrabFocus() {
		_button.GrabFocus();
	}
}