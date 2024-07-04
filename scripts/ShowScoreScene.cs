using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ShowScoreScene : CanvasLayer
{
	private int _score;
	private CooldownScene _cooldownScene;
	private SaveDataManager _saveManager;

	private Label _scoreLabel;

	private WhackAMatchSingleton _singleton;
	private SoundFx _soundFx;

	private ScoreBoard _scoreBoard;

	public override void _Ready()
	{
		_singleton = GetNode<WhackAMatchSingleton>("/root/WhackAMatchSingleton");
		_soundFx = GetNode<SoundFx>("/root/SoundFx");

		_saveManager = GetNode<SaveDataManager>("/root/SaveDataManager");
		_scoreLabel = GetNode<Label>("Panel/MarginContainer/VBoxContainer/ScoreLabel");
		_scoreBoard = GetNode<ScoreBoard>("Panel/MarginContainer/VBoxContainer/ScoreBoard");
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

		var data = _saveManager.LoadGame();

		if (_singleton.IsHardMode)
		{
			_saveManager.CheckUpdateSaveData(data, _score, true);
			_scoreBoard.ShowScoreHistory(data.HardScoreHistories);
		}
		else
		{
			_saveManager.CheckUpdateSaveData(data, _score, false);
			_scoreBoard.ShowScoreHistory(data.EasyScoreHistories);
		}

		_scoreLabel.Text = _score.ToString();
	}

	private void RetryGame()
	{
		_soundFx.Play("click");
		_singleton.EmitSignal(nameof(_singleton.GotoGameScene));
	}

	private void DoneGame()
	{
		_soundFx.Play("click");
		_singleton.EmitSignal(nameof(_singleton.GoToTitleScene));
	}
}
