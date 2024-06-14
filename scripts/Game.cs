using Godot;
using System;
using System.Collections.Generic;

public partial class Game : CanvasLayer
{
	private MoleManager _moleManager;
	private GameTimer _timer;
	private Label _scoreLabel;
	private MarginContainer _playZone;

	private WhackAMatchSingleton _singleton;

	public override void _Ready()
	{
		_singleton = GetNode<WhackAMatchSingleton>("/root/WhackAMatchSingleton");
		_playZone = GetNode<MarginContainer>("MarginContainer/VBoxContainer/HBoxContainer2/TextureRect/PlayZone");
		_timer = GetNode<GameTimer>("MarginContainer/VBoxContainer/HBoxContainer/Timer");
		_timer.OnTimeout += OnGameTimeout;

		var startGameTimer = GetNode<CountdownScene>("StartGameTimer");
		startGameTimer.OnTimeout += StartGame;

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
			matchingItemsContainer,
			moles,
			GetNode<ResultScene>("ResultScene"),
			_playZone);

		_playZone.Hide();
		startGameTimer.StartCountdown();

		var gobackButton = GetNode<TextureButton>("MarginContainer/VBoxContainer/GobackButton");
		gobackButton.Pressed += GoToTitleScene;
	}

	private async void StartGame()
	{
		await _moleManager.ResetGame();
		_timer.StartCountdown();
	}

	private void OnGameTimeout()
	{
		GD.Print("timeout");
		//TODO: save score
		_singleton.GotoScene("res://scenes/show_score_scene.tscn", _moleManager.Scorer.Score.ToString());
	}

	public override void _Process(double delta)
	{
		_scoreLabel.Text = _moleManager.Scorer.Score.ToString();
	}


	private void GoToTitleScene()
	{
		_singleton.GotoScene("res://scenes/title_scene.tscn");
	}
}
