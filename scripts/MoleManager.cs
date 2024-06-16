using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Timer = Godot.Timer;

public partial class MoleManager : Node
{
    private WhackAMatchSingleton _singleton;
    private SoundFx _soundFx;
    private HBoxContainer _matchingItemsContainer;
    private List<Mole> _moles;
    private ResultScene _resultScene;
    private MarginContainer _playZone;
    private Timer _moleTimer;

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
        MarginContainer playZone,
        Timer moleTimer)
    {
        _singleton = singleton;
        _soundFx = soundFx;
        _matchingItemsContainer = matchingItemsContainer;
        _moles = moles;
        _resultScene = resultScene;
        _playZone = playZone;
        _singleton.OnMolePressed += OnMolePressed;
        _moleTimer = moleTimer;
        _moleTimer.Timeout += OnStartHideMoles;

        _matchingItemScene = GD.Load<PackedScene>("res://scenes/matching_item.tscn");
        _targetMatchingItems = new List<MatchingItem>();

        var path = "res://assets/whack_a_match/pickable_items/";
        _textureGroups = ImageLoader.LoadFromPath(path);

        Scorer = new Scorer();
    }

    private void RandomTarget()
    {
        _targetMatchingItems.Clear();
        for (var i = 0; i < Scorer.CurrentRule.MatchCount; i++)
        {
            MatchingItem item = _matchingItemScene.Instantiate<MatchingItem>();
            item.Init(_randomPickableItems[i]);
            _matchingItemsContainer.AddChild(item);

            _targetMatchingItems.Add(item);
        }
    }

    private void SetupMoles()
    {
        var moleCount = Scorer.CurrentRule.MoleCount;

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

        _moleTimer.WaitTime = Scorer.CurrentRule.MoleTime;
        _moleTimer.Start();
    }

    public Task ResetGame()
    {
        Scorer.Reset(_singleton.IsHardMode);
        foreach (var mole in _moles)
        {
            mole.Reset();
        }
        return RefreshMatching(true);
    }

    public void Stop()
    {
        _moleTimer.Stop();
        _playZone.Hide();
    }

    public async Task RefreshMatching(bool isReset = false)
    {
        _playZone.Hide();

        if (!isReset)
            await Task.Delay(400);
 
        _singleton.ClearNodesByGroupName("MatchTarget");

        var group = _textureGroups.OrderBy(x => Random.Shared.Next()).First();
        _randomPickableItems = group.Textures.OrderBy(x => Random.Shared.Next()).ToArray();

        RandomTarget();

        SetupMoles();

        _playZone.Show();
    }

    public async void OnMolePressed(Mole pressedMole)
    {
        _moleTimer.Stop();

        var matchingItem = _targetMatchingItems.FirstOrDefault(x => x.Key == pressedMole.Key);
        var isCorrect = matchingItem != null;
        foreach (var mole in _moles.Where(x => !x.IsHiding && x != pressedMole))
        {
            mole.HideMole();
        }

        if (isCorrect)
        {
            _targetMatchingItems.Remove(matchingItem);
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
                Scorer.AddScore(true);
                await Task.WhenAll(_resultScene.ShowCorrect(), matchingItem.Blink());
                pressedMole.HideMole();
                await RefreshMatching();
            }
            else
            {
                Scorer.AddScore();
                await matchingItem.Blink();
                pressedMole.HideMole();
                matchingItem.HideImage();
 
                SetupMoles();
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

    public async void OnStartHideMoles()
    {
        foreach (var mole in _moles.Where(x => !x.IsHiding))
        {
            mole.HideMole();
        }

        await Task.Delay(200);
        SetupMoles();
    }
}