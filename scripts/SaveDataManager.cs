using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Dictionary = Godot.Collections.Dictionary;

public partial class SaveDataManager : Node2D
{
	private string _path, _fileName;

	public override void _Ready()
	{
		_path = ProjectSettings.GlobalizePath("user://");
		_fileName = "SaveGame.json";
	}

	public void SaveData(GameSaveData saveData)
	{
		if (!Directory.Exists(_path))
		{
			Directory.CreateDirectory(_path);
		}

		var filePath = Path.Join(_path, _fileName);
		var json = JsonSerializer.Serialize(saveData);
		GD.Print(json);
		try
		{
			File.WriteAllText(filePath, json);
		}
		catch (Exception ex)
		{
			GD.PrintErr(ex);
		}
	}

	public GameSaveData LoadGame()
	{
		var filePath = Path.Join(_path, _fileName);
		
		if (!File.Exists(filePath))
		{
			return new GameSaveData();
		}

		try
		{
			var json = File.ReadAllText(filePath);
			GD.Print(json);
			return JsonSerializer.Deserialize<GameSaveData>(json);
		}
		catch (Exception ex)
		{
			GD.PrintErr(ex);
		}

		return new GameSaveData();
	}

	public void CheckUpdateSaveData(GameSaveData data, int newScore, bool isHardMode)
	{
		if (isHardMode)
		{
			data.HardScoreHistories.Add(newScore);
			data.HardScoreHistories = data.HardScoreHistories.OrderByDescending(x => x).Take(5).ToList();
		}
		else
		{
			data.EasyScoreHistories.Add(newScore);
			data.EasyScoreHistories = data.EasyScoreHistories.OrderByDescending(x => x).Take(5).ToList();
		}
		SaveData(data);
	}
}

public class GameSaveData
{
	public List<int> EasyScoreHistories { get; set; }
	public List<int> HardScoreHistories { get; set; }

	public GameSaveData()
	{
		EasyScoreHistories = new List<int>();
		HardScoreHistories = new List<int>();
	}
}