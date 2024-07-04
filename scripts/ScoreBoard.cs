using Godot;
using System;
using System.Collections.Generic;

public partial class ScoreBoard : MarginContainer
{
	private PackedScene _scoreHistoryItemScene;
	private VBoxContainer _scoreHistoryContainer;

	public override void _Ready()
	{
		_scoreHistoryItemScene = GD.Load<PackedScene>("res://scenes/score_history_item.tscn");
		_scoreHistoryContainer = GetNode<VBoxContainer>("VBoxContainer/ScoreHistoryContainer");
	}

	public void ShowScoreHistory(List<int> scoreHistory)
	{
		foreach (Node child in _scoreHistoryContainer.GetChildren())
		{
			child.QueueFree();
		}

		for (var i = 0; i < 5; i++)
		{
			ScoreHistoryItem item = _scoreHistoryItemScene.Instantiate<ScoreHistoryItem>();
			if (i < scoreHistory.Count)
			{
				item.Score = scoreHistory[i];
			}

			_scoreHistoryContainer.AddChild(item);
		}
	}
}
