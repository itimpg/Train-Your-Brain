using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

public partial class MoleManager : Node
{
    PackedScene _matchingItemScene;

    List<TextureGroup> _textureGroups;
    List<MatchingItem> _targetMatchingItems;

    private WhackAMatchSingleton _singleton;
    private List<Mole> _moles;
    private HBoxContainer _matchingItemsContainer;

    public MoleManager(
        WhackAMatchSingleton singleton,
        HBoxContainer matchingItemsContainer,
        List<Mole> moles)
    {
        _targetMatchingItems = new List<MatchingItem>();

        _matchingItemScene = GD.Load<PackedScene>("res://scenes/matching_item.tscn");

        _singleton = singleton;
        _singleton.OnMolePressed += OnMolePressed;

        _matchingItemsContainer = matchingItemsContainer;
        _moles = moles;

        var path = "res://assets/whack_a_match/pickable_items/";
        var rootDir = DirAccess.Open(path);

        if (rootDir == null)
        {
            throw new Exception($"directory {path} not found");
        }

        _textureGroups = new List<TextureGroup>();

        var directories = rootDir.GetDirectories();
        foreach (var dir in directories)
        {
            var dirPath = path + dir;
            var loadedTextures = DirAccess.Open(dirPath).GetFiles()
                .Where(x => x.Contains(".import"))
                .Select(fileName => new LoadedTexture
                {
                    Name = fileName,
                    Texture2D = ResourceLoader.Load<Texture2D>($"{dirPath}/{fileName}".Replace(".import", ""))
                });
            _textureGroups.Add(new TextureGroup
            {
                GroupName = dir,
                Textures = loadedTextures
            });
        }
    }

    private LoadedTexture[] _randomPickableItems;

    private void RandomTarget(int pickableItemCount)
    {
        _targetMatchingItems.Clear();
        for (var i = 0; i < pickableItemCount; i++)
        {
            MatchingItem item = _matchingItemScene.Instantiate<MatchingItem>();
            item.Init(_randomPickableItems[i]);
            _matchingItemsContainer.AddChild(item);

            _targetMatchingItems.Add(item);
        }
    }

    private void SetupMoles(int moleCount)
    {
        var itemToMatch = _targetMatchingItems
            .OrderBy(x => Random.Shared.Next())
            .First();

        var remainingPickableItems = _randomPickableItems
            .Where(x => !_targetMatchingItems.Select(x => x.Key).Contains(x.Name))
            .Take(moleCount - 1)
            .Append(itemToMatch.LoadedTexture)
            .OrderBy(x => Random.Shared.Next())
            .ToArray();

        var randomMoles = _moles.OrderBy(x => Random.Shared.Next()).Take(moleCount).ToArray();

        for (var i = 0; i < moleCount; i++)
        {
            randomMoles[i].SetUp(remainingPickableItems[i]);
            randomMoles[i].ShowMole();
        }
    }

    public void SetNewGame()
    {
        var pickableItemCount = 4;
        var moleCount = 3;

        _singleton.ClearNodesByGroupName("MatchTarget");

        var group = _textureGroups.OrderBy(x => Random.Shared.Next()).First();
        _randomPickableItems = group.Textures.OrderBy(x => Random.Shared.Next()).ToArray();

        RandomTarget(pickableItemCount);

        SetupMoles(moleCount);
    }

    public void EndTurn()
    {
        foreach (var mole in _moles)
        {
            mole.HideMole();
        }
    }

    public async void OnMolePressed(Mole pressedMole)
    {
        var matchingItem = _targetMatchingItems.FirstOrDefault(x => x.Key == pressedMole.Key);
        var isCorrect = matchingItem != null;
        foreach (var mole in _moles.Where(x => !x.IsHiding && x != pressedMole))
        {
            mole.HideMole();
        }

        if (isCorrect)
        {
            await matchingItem.Blink();
            pressedMole.HideMole();
            // TODO: add new score
            _targetMatchingItems.Remove(matchingItem);
            if (_targetMatchingItems.Count == 0)
            {
                SetNewGame();
            }
            else
            {
                matchingItem.HideImage();

                var moleCount = 3;
                SetupMoles(moleCount);
            }
        }
        else
        {
            // TODO: minus score
            await Task.Delay(600);
            pressedMole.HideMole();
            SetNewGame();
        }
    }
}

public class TextureGroup
{
    public string GroupName { get; set; }
    public IEnumerable<LoadedTexture> Textures { get; set; }
}

public class LoadedTexture
{
    public string Name { get; set; }
    public Texture2D Texture2D { get; set; }
}