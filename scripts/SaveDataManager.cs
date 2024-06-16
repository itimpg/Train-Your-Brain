using Godot;
using System;
using System.IO;
using Dictionary = Godot.Collections.Dictionary;

public partial class SaveDataManager : Node2D
{
	private string _path, _fileName;

	public override void _Ready()
	{
		_path = ProjectSettings.GlobalizePath("user://");
		_fileName = "SaveGame.json";
	}

	public void SaveData(SaveFile saveFile)
	{
		if (!Directory.Exists(_path))
		{
			Directory.CreateDirectory(_path);
		}

		var filePath = Path.Join(_path, _fileName);

		try
		{
			File.WriteAllText(filePath, saveFile.ToString());
		}
		catch (Exception ex)
		{
			GD.PrintErr(ex);
		}
	}

	public SaveFile LoadData()
	{
		var filePath = Path.Join(_path, _fileName);
		if (!File.Exists(filePath))
		{
			return new SaveFile();
		}

		try
		{
			var loadedData = File.ReadAllText(filePath);
			Json jsonLoader = new Json();
			Error error = jsonLoader.Parse(loadedData);
			if (error != Error.Ok)
			{
				GD.PrintErr(error);
				return new SaveFile();
			}

			return new SaveFile((Dictionary)jsonLoader.Data);
		}
		catch (Exception ex)
		{
			GD.PrintErr(ex);
		}

		return new SaveFile();
	}
}

public class SaveFile
{
	private Dictionary _dictionary;

	public SaveFile()
	{
		_dictionary = new Dictionary();
	}

	public SaveFile(Dictionary dictionary)
	{
		_dictionary = dictionary;
	}

	public int HighScore
	{
		get
		{
			return _dictionary.ContainsKey(nameof(HighScore)) ? (int)_dictionary[nameof(HighScore)] : 0;
		}
		set
		{
			if (_dictionary.ContainsKey(nameof(HighScore)))
			{
				_dictionary[nameof(HighScore)] = value;
			}
			else
			{
				_dictionary.Add(nameof(HighScore), value);
			}
		}
	}

	public int HighScoreHardMode
	{
		get
		{
			return _dictionary.ContainsKey(nameof(HighScoreHardMode)) ? (int)_dictionary[nameof(HighScoreHardMode)] : 0;
		}
		set
		{
			if (_dictionary.ContainsKey(nameof(HighScoreHardMode)))
			{
				_dictionary[nameof(HighScoreHardMode)] = value;
			}
			else
			{
				_dictionary.Add(nameof(HighScoreHardMode), value);
			}
		}
	}

	public override string ToString()
	{
		return Json.Stringify(_dictionary);
	}
}