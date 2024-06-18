using Godot;
using System;

public partial class PlayZoneScene : MarginContainer
{
	public MatchingItemsContainer MatchingItemsContainer { get; private set; }
	public MolesContainer MolesContainer { get; private set; }

	public override void _Ready()
	{
		MatchingItemsContainer = GetNode<MatchingItemsContainer>("VBoxContainer/MatchingItemsContainer");
		MolesContainer = GetNode<MolesContainer>("VBoxContainer/MolesContainer");
	}
}
