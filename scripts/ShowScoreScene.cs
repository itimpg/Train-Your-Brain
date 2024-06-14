using Godot;
using System;

public partial class ShowScoreScene : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var highScoreLabel = GetNode<Label>("Panel/MarginContainer/VBoxContainer/HBoxContainer/HighScoreLabel");
		var scoreLabel = GetNode<Label>("Panel/MarginContainer/VBoxContainer/ScoreLabel");

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
}
