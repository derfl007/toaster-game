using Godot;
using Godot.Collections;

namespace ToasterGame.scripts;

// todo: figure out how to save completed levels and collected butter
public static class SaveGame {
	
	/// <summary>
	/// Saves a level's butter count to the save file.
	/// Format: { "levelName": butterCount }[]
	/// </summary>
	/// <param name="levelName">The filename of the level (e.g. level_1.tscn)</param>
	/// <param name="butter">The number of butter collected (either 0 or 1)</param>
	public static void SaveLevel(string levelName, int butter) {
		
		var level = new Dictionary<string, Variant>() {
			{ levelName, butter }
		};

		string jsonString;
		if (FileAccess.FileExists("user://savegame.save")) {
			using var oldSaveGame = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Read);
			var saveDataString = oldSaveGame.GetLine();
			var saveData = Json.ParseString(saveDataString).As<Dictionary<string, Variant>>();
			var previous = saveData.TryGetValue(levelName, out var value) ? value.As<int>() : -1;
			if (previous >= butter) return; // don't save if the butter count is the same
			saveData.Merge(level, butter >= previous); // only overwrite if the new butter count is higher
			jsonString = Json.Stringify(saveData);
		}
		else {
			jsonString = Json.Stringify(level);
		}
		
		using var saveGame = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Write);
		saveGame.StoreLine(jsonString);
	}

	/// <summary>
	/// Returns a dictionary with the level names as keys and the butter count as values.
	/// </summary>
	/// <returns></returns>
	public static Dictionary<string, Variant> LoadSave() {
		if (FileAccess.FileExists("user://savegame.save")) {
			using var saveGame = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Read);
			return Json.ParseString(saveGame.GetLine()).As<Dictionary<string, Variant>>();
		}
		else {
			SaveLevel("level_0.tscn", -1);
			using var saveGame = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Read);
			return Json.ParseString(saveGame.GetLine()).As<Dictionary<string, Variant>>();
		}
	}
}