using Godot;
using Godot.Collections;

namespace ToasterGame.scripts;

// todo: figure out how to save completed levels and collected butter
public class SaveGame {
	public static void SaveLastLevel(string levelName, int totalButter) {
		using var saveGame = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Write);
		var saveData = new Dictionary<string, Variant> {
			{ "last_level", levelName },
			{ "butter", totalButter }
		};
		var jsonString = Json.Stringify(saveData);
		saveGame.StoreLine(jsonString);
	}
}