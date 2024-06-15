using Godot;
using System;

public partial class GameTimer : MarginContainer
{
	private Timer _timer;
	private Label _countdownLabel;

	[Signal]
	public delegate void OnTimeoutEventHandler();

	public override void _Ready()
	{
		_timer = GetNode<Timer>("Timer");
		_timer.Timeout += NotifyTimeout;

		_countdownLabel = GetNode<Label>("HBoxContainer/CountdownLabel");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var countdown = _timer.IsStopped() ? 60 : (int)_timer.TimeLeft + 1;
		_countdownLabel.Text = countdown.ToString().PadLeft(2);
	}

	public void Reset()
	{
		_timer.Stop();
	}

	public void StartCountdown()
	{
		_timer.Start();
	}

	private void NotifyTimeout()
	{
		EmitSignal(SignalName.OnTimeout);
	}

}
