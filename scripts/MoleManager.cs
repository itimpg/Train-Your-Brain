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
    private SoundFx _soundFx;
    private HBoxContainer _matchingItemsContainer;
    private List<Mole> _moles;
    private ResultScene _resultScene;
    private MarginContainer _playZone;

    private PackedScene _matchingItemScene;
    private List<MatchingItem> _targetMatchingItems;
    private IEnumerable<TextureGroup> _textureGroups;

    private LoadedTexture[] _randomPickableItems;

    public Scorer Scorer { get; private set; }

    public MoleManager(
        WhackAMatchSingleton singleton,
        SoundFx soundFx,
        HBoxContainer matchingItemsContainer,
        List<Mole> moles,
        ResultScene resultScene,
        MarginContainer playZone)
    {
        _singleton = singleton;
        _soundFx = soundFx;
        _matchingItemsContainer = matchingItemsContainer;
        _moles = moles;
        _resultScene = resultScene;
        _playZone = playZone;
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

    public Task ResetGame()
    {
        Scorer.Reset();
        foreach(var mole in _moles){
            mole.Reset();
        }
        return RefreshMatching(true);
    }

    public async Task RefreshMatching(bool isReset = false)
    {
        _playZone.Hide();

        if (!isReset)
            await Task.Delay(400);

        var pickableItemCount = 4;
        var moleCount = 3;

        _singleton.ClearNodesByGroupName("MatchTarget");

        var group = _textureGroups.OrderBy(x => Random.Shared.Next()).First();
        _randomPickableItems = group.Textures.OrderBy(x => Random.Shared.Next()).ToArray();

        RandomTarget(pickableItemCount);

        SetupMoles(moleCount);

        _playZone.Show();
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
            _targetMatchingItems.Remove(matchingItem);
            Scorer.AddScore();

            if (_targetMatchingItems.Count == 0)
            {
                _soundFx.Play("correct_all_answer");
            }
            else
            {
                _soundFx.Play("correct_answer");
            }

            pressedMole.WhackCorrect();
            if (_targetMatchingItems.Count == 0)
            {
                await Task.WhenAll(_resultScene.ShowCorrect(), matchingItem.Blink());
                pressedMole.HideMole();
                await RefreshMatching();
            }
            else
            {
                await matchingItem.Blink();
                pressedMole.HideMole();
                matchingItem.HideImage();

                var moleCount = 3;
                SetupMoles(moleCount);
            }
        }
        else
        {
            _soundFx.Play("wrong_answer");
            pressedMole.WhackIncorrect();
            await _resultScene.ShowIncorrect();
            Scorer.MinusScore();
            pressedMole.HideMole();
            await RefreshMatching();
        }
    }
}