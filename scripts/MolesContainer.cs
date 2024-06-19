using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class MolesContainer : Control
{
	[Signal]
	public delegate void OnTimeoutEventHandler();

	private WhackAMatchSingleton _singleton;
	private bool _isMolePressed;

	public List<Mole> Moles { get; private set; }
	private Timer _timer;

	public override void _Ready()
	{
		_singleton = GetNode<WhackAMatchSingleton>("/root/WhackAMatchSingleton");
		_singleton.OnMolePressed += OnMolePressed;

		Moles = new List<Mole>
		{
			GetNode<Mole>("VBoxContainer/HBoxContainer3/Mole2"),
			GetNode<Mole>("VBoxContainer/HBoxContainer3/Mole3"),
			GetNode<Mole>("VBoxContainer/HBoxContainer2/Mole1"),
			GetNode<Mole>("VBoxContainer/HBoxContainer4/Mole4")
		};

		_timer = GetNode<Timer>("Timer");
		_timer.Timeout += OnMolesTimeout;
	}

	public void StopMoleCountdown()
	{
		_timer.Stop();
	}

	public void Reset()
	{
		_isMolePressed = false;
		foreach (var mole in Moles)
		{
			mole.Reset();
		}
	}

	public void HideMolesButSelected(Mole selectedMole)
	{
		foreach (var mole in Moles.Where(x => !x.IsHiding && x != selectedMole))
		{
			mole.HideMole();
		}
	}

	public void RandomMole(ScoreRule scoreRule, IEnumerable<LoadedTexture> allTextures, IEnumerable<MatchingItem> matchingItems)
	{
		var itemToMatch = matchingItems
			.OrderBy(x => Random.Shared.Next())
			.First();

		var remainingPickableItems = allTextures
			.Where(x => !matchingItems.Select(x => x.Key).Contains(x.Name))
			.Take(scoreRule.MoleCount - 1)
			.Append(itemToMatch.LoadedTexture)
			.OrderBy(x => Random.Shared.Next())
			.ToArray();

		var randomMoles = Moles
			.Take(scoreRule.MoleSlot)
			.OrderBy(x => Random.Shared.Next())
			.Take(scoreRule.MoleCount)
			.ToArray();

		for (var i = 0; i < Moles.Count; i++)
		{
			Moles[i].Hide();

			if (i < scoreRule.MoleSlot)
			{
				Moles[i].Show();
			}
		}

		for (var i = 0; i < scoreRule.MoleCount; i++)
		{
			randomMoles[i].SetUp(remainingPickableItems[i]);
			randomMoles[i].ShowMole();
		}

		_timer.WaitTime = scoreRule.MoleTime;
		_timer.Start();

		_isMolePressed = false;
	}

	private async void OnMolesTimeout()
	{
		foreach (var mole in Moles.Where(x => !x.IsHiding && x.IsVisibleInTree()))
		{
			mole.HideMole();
		}

		await Task.Delay(200);
		if (_isMolePressed)
		{
			return;
		}

		EmitSignal(SignalName.OnTimeout);
	}

	private void OnMolePressed(Mole mole)
	{
		_isMolePressed = true;
	}
}
