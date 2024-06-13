using Godot;
using System;
using System.Collections.Generic;

public partial class Game : CanvasLayer
{
	private MoleManager _moleManager;
	private GameTimer _timer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{ 
		_timer = GetNode<GameTimer>("MarginContainer/VBoxContainer/Timer");
		_timer.OnTimeout += OnTimeout;

		var matchingItemsContainer = GetNode<HBoxContainer>("MarginContainer/VBoxContainer/HBoxContainer2/TextureRect/MarginContainer/VBoxContainer/HBoxContainer/TextureRect/MatchingItemsContainer");

		var mole1 = GetNode<Mole>("MarginContainer/VBoxContainer/HBoxContainer2/TextureRect/MarginContainer/VBoxContainer/HBoxContainer2/Mole1");
		var mole2 = GetNode<Mole>("MarginContainer/VBoxContainer/HBoxContainer2/TextureRect/MarginContainer/VBoxContainer/HBoxContainer3/Mole2");
		var mole3 = GetNode<Mole>("MarginContainer/VBoxContainer/HBoxContainer2/TextureRect/MarginContainer/VBoxContainer/HBoxContainer3/Mole3");
		var mole4 = GetNode<Mole>("MarginContainer/VBoxContainer/HBoxContainer2/TextureRect/MarginContainer/VBoxContainer/HBoxContainer4/Mole4");

		var moles = new List<Mole>
		{
			mole1, mole2, mole3, mole4
		};

		_moleManager = new MoleManager(
			GetNode<WhackAMatchSingleton>("/root/WhackAMatchSingleton"),
			matchingItemsContainer,
			moles);

		_moleManager.SetNewGame();
		_timer.StartCountdown();
	}

    private void OnTimeout()
    {
		GD.Print("timeout");
    }

    public override void _Process(double delta)
	{
	}

}
