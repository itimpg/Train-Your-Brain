using Godot;
using System;

public partial class ShowScoreScene : CanvasLayer
{
	private string _score;

	public override void _Ready()
	{
		var saveManager = GetNode<SaveDataManager>("/root/SaveDataManager");
		var data = saveManager.LoadData();

		var newScore = Convert.ToInt32(_score);
		if (newScore > data.HighScore)
		{
			data.HighScore = newScore;
			saveManager.SaveData(data);
		}

		var highScoreLabel = GetNode<Label>("Panel/MarginContainer/VBoxContainer/HBoxContainer/HighScoreLabel");
		highScoreLabel.Text = data.HighScore.ToString();

		var scoreLabel = GetNode<Label>("Panel/MarginContainer/VBoxContainer/ScoreLabel");
		scoreLabel.Text = _score;

		var retryButton = GetNode<TextureButton>("Panel/MarginContainer/VBoxContainer/RetryButton");
		retryButton.Pressed += RetryGame;

		var doneButton = GetNode<TextureButton>("Panel/MarginContainer/VBoxContainer/DoneButton");
		doneButton.Pressed += DoneGame;
	}

	private void RetryGame()
	{
		var global = GetNode<WhackAMatchSingleton>("/root/WhackAMatchSingleton");
		global.GotoScene("res://scenes/game.tscn");
	}

	private void DoneGame()
	{
		var global = GetNode<WhackAMatchSingleton>("/root/WhackAMatchSingleton");
		global.GotoScene("res://scenes/title_scene.tscn");
	}

	public void Initialize(string data)
	{
		_score = data;
	}
}
