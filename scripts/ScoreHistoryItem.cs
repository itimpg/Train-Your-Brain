using Godot;
using System;

public partial class ScoreHistoryItem : MarginContainer
{
	private Label _scoreLabel;
	public int? Score { private get; set; }

	public override void _Ready()
	{
		_scoreLabel = GetNode<Label>("Panel/MarginContainer/ScoreLabel");
		_scoreLabel.Text = "";
		if (Score.HasValue)
		{
			_scoreLabel.Text = Score.Value.ToString();
		}
	}
}
