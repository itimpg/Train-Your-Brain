using Godot;
using System;

public partial class CountdownScene : Control
{

	[Signal]
	public delegate void OnTimeoutEventHandler();

	private Label _countdownLabel;
	private Timer _timer;

	public override void _Ready()
	{
		_countdownLabel = GetNode<Label>("Panel/CountdownLabel");
		_timer = GetNode<Timer>("Timer");
		_timer.Timeout += NotifyTimeout;

		Hide();
	}

	public void StartCountdown()
	{
		Show();
		_timer.Start();
	}

	public override void _Process(double delta)
	{
		var countdown = (int)_timer.TimeLeft + 1;
		_countdownLabel.Text = countdown.ToString();
	}

	private void NotifyTimeout()
	{
		EmitSignal(SignalName.OnTimeout);
		Hide();
	}
}
