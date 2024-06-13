using Godot;
using System;

public partial class WhackAMatchSingleton : Node
{
    [Signal]
    public delegate void OnMolePressedEventHandler(Mole mole);

    public Node CurrentScene { get; set; }

    public override void _Ready()
    {
        Viewport root = GetTree().Root;
        CurrentScene = root.GetChild(root.GetChildCount() - 1);
    }

    public void ClearNodesByGroupName(string groupName)
    {
        foreach (var item in GetTree().GetNodesInGroup(groupName))
        {
            item.QueueFree();
        }
    }
}