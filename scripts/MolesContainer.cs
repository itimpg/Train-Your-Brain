using Godot;
using System;
using System.Collections.Generic;

public partial class MolesContainer : Control
{
	public List<Mole> Moles { get; private set; }

	public override void _Ready()
	{
		Moles = new List<Mole>
		{
			GetNode<Mole>("VBoxContainer/HBoxContainer2/Mole1"),
			GetNode<Mole>("VBoxContainer/HBoxContainer3/Mole2"),
			GetNode<Mole>("VBoxContainer/HBoxContainer3/Mole3"),
			GetNode<Mole>("VBoxContainer/HBoxContainer4/Mole4")
		};
	}

	public void Reset()
	{
		foreach (var mole in Moles)
		{
			mole.Reset();
		}
	}
}
