using Godot;
using Godot.Collections;

namespace ToasterGame.scripts;

// todo: figure out how to save completed levels and collected butter
public class SaveGame {
	public static void SaveLevel(string levelName, int butter) {
		using var oldSaveGame = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Read);
		var saveDataString = oldSaveGame.GetLine();
		var saveData = Json.ParseString(saveDataString).As<Dictionary<string, Variant>>();
		var level = new Dictionary<string, Variant>() {
			{ levelName, butter }
		};
		saveData.Merge(level);
		var jsonString = Json.Stringify(saveData);
		using var saveGame = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Write);
		saveGame.StoreLine(jsonString);
	}

	public static Dictionary<string, Variant> LoadSave() {
		using var saveGame = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Read);
		return Json.ParseString(saveGame.GetLine()).As<Dictionary<string, Variant>>();
	}
}