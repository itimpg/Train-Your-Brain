using Godot;
using System;
using System.Collections.Generic;
using Timer = Godot.Timer;

public partial class Game : CanvasLayer
{
	private MoleManager _moleManager;
	private GameTimer _timer;
	private Label _scoreLabel;
	private MarginContainer _playZone;

	private WhackAMatchSingleton _singleton;

	private CountdownScene _startGameTimer;

	public override void _Ready()
	{
		_singleton = GetNode<WhackAMatchSingleton>("/root/WhackAMatchSingleton");
		_playZone = GetNode<MarginContainer>("MarginContainer/VBoxContainer/HBoxContainer2/TextureRect/PlayZone");
		_timer = GetNode<GameTimer>("MarginContainer/VBoxContainer/HBoxContainer/Timer");
		_timer.OnTimeout += OnGameTimeout;

		_startGameTimer = GetNode<CountdownScene>("StartGameTimer");
		_startGameTimer.OnTimeout += StartGame;

		_scoreLabel = GetNode<Label>("MarginContainer/VBoxContainer/HBoxContainer/ScoreLabel");

		var matchingItemsContainer = GetNode<HBoxContainer>("MarginContainer/VBoxContainer/HBoxContainer2/TextureRect/PlayZone/VBoxContainer/HBoxContainer/TextureRect/MatchingItemsContainer");

		var mole1 = GetNode<Mole>("MarginContainer/VBoxContainer/HBoxContainer2/TextureRect/PlayZone/VBoxContainer/HBoxContainer2/Mole1");
		var mole2 = GetNode<Mole>("MarginContainer/VBoxContainer/HBoxContainer2/TextureRect/PlayZone/VBoxContainer/HBoxContainer3/Mole2");
		var mole3 = GetNode<Mole>("MarginContainer/VBoxContainer/HBoxContainer2/TextureRect/PlayZone/VBoxContainer/HBoxContainer3/Mole3");
		var mole4 = GetNode<Mole>("MarginContainer/VBoxContainer/HBoxContainer2/TextureRect/PlayZone/VBoxContainer/HBoxContainer4/Mole4");

		var moles = new List<Mole>
		{
			mole1, mole2, mole3, mole4
		};

		_moleManager = new MoleManager(
			_singleton,
			GetNode<SoundFx>("/root/SoundFx"),
			matchingItemsContainer,
			moles,
			GetNode<ResultScene>("ResultScene"),
			_playZone,
			GetNode<Timer>("MoleTimer"));

		var gobackButton = GetNode<TextureButton>("MarginContainer/VBoxContainer/GobackButton");
		gobackButton.Pressed += GoToTitleScene;
	}

	public void ShowGameScene()
	{
		_timer.Reset();
		_moleManager.Scorer.Reset();
		_playZone.Hide();
		_startGameTimer.StartCountdown();
	}

	private async void StartGame()
	{
		await _moleManager.ResetGame();
		_timer.StartCountdown();
	}

	public override void _Process(double delta)
	{
		_scoreLabel.Text = _moleManager.Scorer.Score.ToString();
	}

	private void OnGameTimeout()
	{
		_moleManager.Stop();
		_singleton.EmitSignal(nameof(_singleton.GoToShowScoreScene), _moleManager.Scorer.Score);
	}

	private void GoToTitleScene()
	{
		_singleton.EmitSignal(nameof(_singleton.GoToTitleScene));
	}
}
