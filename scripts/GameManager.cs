using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Timer = Godot.Timer;

public partial class GameManager : Node
{
    private WhackAMatchSingleton _singleton;
    private SoundFx _soundFx;
    private ResultScene _resultScene;
    private PlayZoneScene _playZoneScene;

    private List<MatchingItem> _targetMatchingItems;
    private IEnumerable<TextureGroup> _textureGroups;

    private LoadedTexture[] _randomPickableItems;

    public Scorer Scorer { get; private set; }

    public override void _Ready()
    {
        _singleton = GetNode<WhackAMatchSingleton>("/root/WhackAMatchSingleton");
        _singleton.OnMolePressed += OnMolePressed;

        _soundFx = GetNode<SoundFx>("/root/SoundFx");

        _targetMatchingItems = new List<MatchingItem>();

        Scorer = new Scorer();
    }

    public void Init(
        PlayZoneScene playZoneScene,
        ResultScene resultScene)
    {
        _playZoneScene = playZoneScene;
        _playZoneScene.MolesContainer.OnTimeout += SetupMoles;

        _resultScene = resultScene;

        var path = "res://assets/whack_a_match/pickable_items/";
        _textureGroups = ImageLoader.LoadFromPath(path);
    }

    private void SetupMoles()
    {
        _playZoneScene.MolesContainer.RandomMole(Scorer.CurrentRule, _randomPickableItems, _targetMatchingItems);
    }

    public Task ResetGame()
    {
        Scorer.Reset(_singleton.IsHardMode);
        _playZoneScene.MolesContainer.Reset();
        return RefreshMatching(true);
    }

    public void Stop()
    {
        _playZoneScene.MolesContainer.StopMoleCountdown();
        _playZoneScene.Hide();
    }

    public async Task RefreshMatching(bool isReset = false)
    {
        _playZoneScene.Hide();

        if (!isReset)
            await Task.Delay(400);

        var group = _textureGroups.OrderBy(x => Random.Shared.Next()).First();
        _randomPickableItems = group.Textures.OrderBy(x => Random.Shared.Next()).ToArray();

        _targetMatchingItems = _playZoneScene.MatchingItemsContainer.GenerateMathingItems(Scorer.CurrentRule.MatchCount, _randomPickableItems).ToList();

        SetupMoles();

        _playZoneScene.Show();
    }

    public async void OnMolePressed(Mole pressedMole)
    {
        _playZoneScene.MolesContainer.StopMoleCountdown();
        _playZoneScene.MolesContainer.HideMolesButSelected(pressedMole);

        var matchingItem = _targetMatchingItems.FirstOrDefault(x => x.Key == pressedMole.Key);
        var isCorrect = matchingItem != null;
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
}