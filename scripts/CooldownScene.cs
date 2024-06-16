using Godot;
using System;

public partial class CooldownScene : Control
{
	private Timer _timer;

	public override void _Ready()
	{
		_timer = GetNode<Timer>("Timer");
		_timer.Timeout += NotifyTimeout;
	}

	public void ShowScreen()
	{
		Show();
		_timer.Start();
	}

	private void NotifyTimeout()
	{
		Hide();
	}
}
