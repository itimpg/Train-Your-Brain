using Godot;
using System;

public partial class ShowScoreScene : CanvasLayer
{
	private int _score;
	private CooldownScene _cooldownScene;
	private SaveDataManager _saveManager;

	private Label _highScoreLabel, _scoreLabel;

	private WhackAMatchSingleton _singleton;

	public override void _Ready()
	{
		_singleton = GetNode<WhackAMatchSingleton>("/root/WhackAMatchSingleton");
		_saveManager = GetNode<SaveDataManager>("/root/SaveDataManager");
		_highScoreLabel = GetNode<Label>("Panel/MarginContainer/VBoxContainer/HBoxContainer/HighScoreLabel");
		_scoreLabel = GetNode<Label>("Panel/MarginContainer/VBoxContainer/ScoreLabel");

		var retryButton = GetNode<TextureButton>("Panel/MarginContainer/VBoxContainer/RetryButton");
		retryButton.Pressed += RetryGame;

		var doneButton = GetNode<TextureButton>("Panel/MarginContainer/VBoxContainer/DoneButton");
		doneButton.Pressed += DoneGame;

		_cooldownScene = GetNode<CooldownScene>("CooldownScene");
	}

	public void ShowScore(int score)
	{ 
		_score = score;
		_cooldownScene.ShowScreen();

		var data = _saveManager.LoadData();

		if (_score > data.HighScore)
		{
			data.HighScore = _score;
			_saveManager.SaveData(data);
		}

		_highScoreLabel.Text = data.HighScore.ToString();
		_scoreLabel.Text = _score.ToString();
	}

	private void RetryGame()
	{
		_singleton.EmitSignal(nameof(_singleton.GotoGameScene));
	}

	private void DoneGame()
	{
		_singleton.EmitSignal(nameof(_singleton.GoToTitleScene));
	}
}
