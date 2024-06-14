using Godot;
using System;

public partial class TitleScene : CanvasLayer
{
	public override void _Ready()
	{
		var demoMole = GetNode<Mole>("Panel/MarginContainer/VBoxContainer/Mole");
		demoMole.PlayDemo();

		var startButton = GetNode<TextureButton>("Panel/MarginContainer/VBoxContainer/StartButton");
		startButton.Pressed += GoToGameScene;
	}

	private void GoToGameScene()
	{
		var global = GetNode<WhackAMatchSingleton>("/root/WhackAMatchSingleton");
		global.GotoScene("res://scenes/game.tscn");
	}
}
