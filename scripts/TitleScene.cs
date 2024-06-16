using Godot;
using System;

public partial class TitleScene : CanvasLayer
{

	private SoundFx _soundFx;
	public override void _Ready()
	{
		_soundFx = GetNode<SoundFx>("/root/SoundFx");

		var demoMole = GetNode<Mole>("Panel/MarginContainer/VBoxContainer/Mole");
		demoMole.PlayDemo();

		var startButton = GetNode<TextureButton>("Panel/MarginContainer/VBoxContainer/StartButton");
		startButton.Pressed += GoToGameScene;

		var startHardModeButton = GetNode<TextureButton>("Panel/MarginContainer/VBoxContainer/StartHardModeButton");
		startHardModeButton.Pressed += GoToGameSceneHardMode;
	}

	private void GoToGameScene()
	{
		_soundFx.Play("click");
		var singleton = GetNode<WhackAMatchSingleton>("/root/WhackAMatchSingleton");
		singleton.IsHardMode = false;
		singleton.EmitSignal(nameof(singleton.GotoGameScene));
		Hide();
	}

	private void GoToGameSceneHardMode()
	{
		_soundFx.Play("click");
		var singleton = GetNode<WhackAMatchSingleton>("/root/WhackAMatchSingleton");
		singleton.IsHardMode = true;
		singleton.EmitSignal(nameof(singleton.GotoGameScene));
		Hide();
	}
}
