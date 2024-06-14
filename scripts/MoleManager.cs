using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

public partial class MoleManager : Node
{ 
    private WhackAMatchSingleton _singleton;
    private HBoxContainer _matchingItemsContainer;
    private List<Mole> _moles;
 
    private PackedScene _matchingItemScene; 
    private List<MatchingItem> _targetMatchingItems;
    private IEnumerable<TextureGroup> _textureGroups;

    private LoadedTexture[] _randomPickableItems;
 
    public Scorer Scorer { get; private set; }

    public MoleManager(
        WhackAMatchSingleton singleton,
        HBoxContainer matchingItemsContainer,
        List<Mole> moles)
    {
        _singleton = singleton;
        _matchingItemsContainer = matchingItemsContainer;
        _moles = moles;
        _singleton.OnMolePressed += OnMolePressed;

        _matchingItemScene = GD.Load<PackedScene>("res://scenes/matching_item.tscn");
        _targetMatchingItems = new List<MatchingItem>();

        var path = "res://assets/whack_a_match/pickable_items/";
        _textureGroups = ImageLoader.LoadFromPath(path);
        
        Scorer = new Scorer();
    }

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

    public void ResetGame()
    {
        Scorer.Reset();
        RefreshMatching();
    }

    public void RefreshMatching()
    {
        var pickableItemCount = 4;
        var moleCount = 3;

        _singleton.ClearNodesByGroupName("MatchTarget");

        var group = _textureGroups.OrderBy(x => Random.Shared.Next()).First();
        _randomPickableItems = group.Textures.OrderBy(x => Random.Shared.Next()).ToArray();

        RandomTarget(pickableItemCount);

        SetupMoles(moleCount);
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
            pressedMole.WhackCorrect();
            await matchingItem.Blink();
            pressedMole.HideMole();
            Scorer.AddScore();
            _targetMatchingItems.Remove(matchingItem);
            if (_targetMatchingItems.Count == 0)
            {
                RefreshMatching();
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
            pressedMole.WhackIncorrect();
            Scorer.MinusScore();
            await Task.Delay(600);
            pressedMole.HideMole();
            RefreshMatching();
        }
    }
}