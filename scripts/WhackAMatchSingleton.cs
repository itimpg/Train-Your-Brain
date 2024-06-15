using Godot;
using System;

public partial class WhackAMatchSingleton : Node
{
    [Signal]
    public delegate void OnMolePressedEventHandler(Mole mole);


    [Signal]
    public delegate void GoToTitleSceneEventHandler();


    [Signal]
    public delegate void GotoGameSceneEventHandler();


    [Signal]
    public delegate void GoToShowScoreSceneEventHandler(int score);

    public Node CurrentScene { get; set; }

    public override void _Ready()
    {
        Viewport root = GetTree().Root;
        CurrentScene = root.GetChild(root.GetChildCount() - 1);
    }

    public void GotoScene(string path, string data = null)
    {
        // This function will usually be called from a signal callback,
        // or some other function from the current scene.
        // Deleting the current scene at this point is
        // a bad idea, because it may still be executing code.
        // This will result in a crash or unexpected behavior.

        // The solution is to defer the load to a later time, when
        // we can be sure that no code from the current scene is running:

        CallDeferred(MethodName.DeferredGotoScene, path, data);
    }

    public void DeferredGotoScene(string path, string data)
    {
        // It is now safe to remove the current scene.
        CurrentScene.Free();

        // Load a new scene.
        var nextScene = GD.Load<PackedScene>(path);

        // Instance the new scene.
        CurrentScene = nextScene.Instantiate();
        if (!string.IsNullOrEmpty(data) && CurrentScene.HasMethod("Initialize"))
        {
            CurrentScene.Call("Initialize", data);
        }

        // Add it to the active scene, as child of root.
        GetTree().Root.AddChild(CurrentScene);

        // Optionally, to make it compatible with the SceneTree.change_scene_to_file() API.
        GetTree().CurrentScene = CurrentScene;
    }

    public void ClearNodesByGroupName(string groupName)
    {
        foreach (var item in GetTree().GetNodesInGroup(groupName))
        {
            item.QueueFree();
        }
    }
}