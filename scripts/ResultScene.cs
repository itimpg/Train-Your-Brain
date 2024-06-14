using Godot;
using System;
using System.Threading.Tasks;

public partial class ResultScene : Control
{
	private AnimationPlayer _animationPlayer;

	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		Hide();
	}

	public async Task ShowCorrect()
	{
		Show();
		_animationPlayer.Play("correct");
		await Task.Delay(600);
		Hide();
	}

	public async Task ShowIncorrect()
	{
		Show();
		_animationPlayer.Play("incorrect");
		await Task.Delay(600);
		Hide();
	}
}
