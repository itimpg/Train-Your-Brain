using Godot;
using System;
using System.Collections.Generic;
using Timer = Godot.Timer;

public partial class GameScene : CanvasLayer
{
	private GameManager _gameManager;
	private GameTimer _timer;
	private Label _scoreLabel;
	private PlayZoneScene _playZone;

	private WhackAMatchSingleton _singleton;

	private CountdownScene _startGameTimer;

	public override void _Ready()
	{
		_singleton = GetNode<WhackAMatchSingleton>("/root/WhackAMatchSingleton");
		_playZone = GetNode<PlayZoneScene>("MarginContainer/VBoxContainer/HBoxContainer2/TextureRect/PlayZoneScene");
		_timer = GetNode<GameTimer>("MarginContainer/VBoxContainer/HBoxContainer/Timer");
		_timer.OnTimeout += OnGameTimeout;

		_gameManager = GetNode<GameManager>("GameManager");

		_startGameTimer = GetNode<CountdownScene>("StartGameTimer");
		_startGameTimer.OnTimeout += StartGame;

		_scoreLabel = GetNode<Label>("MarginContainer/VBoxContainer/HBoxContainer/ScoreLabel");
 
		var playZoneScene = GetNode<PlayZoneScene>("MarginContainer/VBoxContainer/HBoxContainer2/TextureRect/PlayZoneScene");

		_gameManager.Init(
			playZoneScene,
			GetNode<ResultScene>("ResultScene"));

		var gobackButton = GetNode<TextureButton>("MarginContainer/VBoxContainer/GobackButton");
		gobackButton.Pressed += GoToTitleScene;
	}

	public void ShowGameScene()
	{
		_timer.Reset();
		_gameManager.Scorer.Reset(_singleton.IsHardMode);
		_playZone.Hide();
		_startGameTimer.StartCountdown();
	}

	private async void StartGame()
	{
		await _gameManager.ResetGame();
		_timer.StartCountdown();
	}

	public override void _Process(double delta)
	{
		_scoreLabel.Text = _gameManager.Scorer.Score.ToString();
	}

	private void OnGameTimeout()
	{
		_gameManager.Stop();
		_singleton.EmitSignal(nameof(_singleton.GoToShowScoreScene), _gameManager.Scorer.Score);
	}

	private void GoToTitleScene()
	{
		_gameManager.Stop();
		_singleton.EmitSignal(nameof(_singleton.GoToTitleScene));
	}
}
