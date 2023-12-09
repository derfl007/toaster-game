using Godot;

namespace ToasterGame.scripts.levels;

[Tool]
public partial class LevelData : Resource
{
	public PackedScene LevelScene;
	public string LevelName;
}