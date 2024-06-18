using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class MatchingItemsContainer : MarginContainer
{
	private HBoxContainer _container;
	private PackedScene _matchingItemScene;


	public override void _Ready()
	{
		_container = GetNode<HBoxContainer>("TextureRect/Container");
		_matchingItemScene = GD.Load<PackedScene>("res://scenes/matching_item.tscn");
	} 

	public IEnumerable<MatchingItem> GenerateMathingItems(int itemCount, LoadedTexture[] textures)
	{
		foreach (var child in _container.GetChildren())
		{
			child.QueueFree();
		}

		for (var i = 0; i < itemCount; i++)
		{
			MatchingItem item = _matchingItemScene.Instantiate<MatchingItem>();
			item.Init(textures[i]);
			_container.AddChild(item);

			yield return item;
		}
	}
}


