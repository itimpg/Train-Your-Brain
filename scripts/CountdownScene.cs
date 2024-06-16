using Godot;
using System;

public partial class CountdownScene : Control
{

	[Signal]
	public delegate void OnTimeoutEventHandler();

	private Label _countdownLabel;
	private Timer _timer;
	private const int MAX_PLAYS = 3;
	private int _playCount = 0;

	private SoundFx _soundFx;

	public override void _Ready()
	{
		_soundFx = GetNode<SoundFx>("/root/SoundFx");

		_countdownLabel = GetNode<Label>("Panel/CountdownLabel");
		_timer = GetNode<Timer>("Timer");
		_timer.Timeout += NotifyTimeout;

		Hide();
	}

	public void StartCountdown()
	{
		_playCount = 0;

		Show();
		_timer.Start();
		_countdownLabel.Text = MAX_PLAYS.ToString();
	}

	private void NotifyTimeout()
	{
		if (_playCount < MAX_PLAYS - 1)
		{
			_soundFx.Play("countdown");
			_playCount++;
			_countdownLabel.Text = (MAX_PLAYS - _playCount).ToString();
		}
		else
		{
			_timer.Stop();
			EmitSignal(SignalName.OnTimeout);
			Hide();
		}
	}
}
