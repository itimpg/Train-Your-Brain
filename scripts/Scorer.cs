using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Scorer
{
    public int Score { get; set; }
    private int _winCount { get; set; }
    private int _ruleIndex;
    public ScoreRule CurrentRule { get { return _scoreRules[_ruleIndex]; } }
    private List<ScoreRule> _scoreRules;
    public Scorer()
    {
        _scoreRules = new List<ScoreRule>
        {
            new ScoreRule
            {
                Class = DifficultyClasses.Sprout,
                MoleTime = 9,
                MatchCount = 1,
                MoleCount = 2,
                MoleSlot = 2,
                WinCountToNextLevel = 1,
                BaseScore = 2
            },
            new ScoreRule
            {
                Class = DifficultyClasses.Sprout,
                MoleTime = 9,
                MatchCount = 2,
                MoleCount = 2,
                MoleSlot = 2,
                WinCountToNextLevel = 1,
                BaseScore = 2
            },
            new ScoreRule
            {
                Class = DifficultyClasses.Beginner,
                MoleTime = 3,
                MatchCount = 2,
                MoleCount = 3,
                MoleSlot = 3,
                WinCountToNextLevel = 2,
                BaseScore = 3
            },
            new ScoreRule
            {
                Class = DifficultyClasses.Intermidiate,
                MoleTime = 2,
                MatchCount = 3,
                MoleCount = 3,
                MoleSlot = 3,
                WinCountToNextLevel = 3,
                BaseScore = 5
            },
            new ScoreRule
            {
                Class = DifficultyClasses.Advanced,
                MoleTime = 1,
                MatchCount = 3,
                MoleCount = 3,
                MoleSlot = 4,
                WinCountToNextLevel = 3,
                BaseScore = 7,
            },
            new ScoreRule
            {
                Class = DifficultyClasses.Elite,
                MoleTime = 1,
                MatchCount = 4,
                MoleCount = 3,
                MoleSlot = 4,
                WinCountToNextLevel = 99,
                BaseScore = 11
            },
        };
    }

    public void Reset(bool isHardMode)
    {
        _winCount = 0;
        Score = 0;
        _ruleIndex = isHardMode ? 3 : 0;
    }

    public void AddScore(bool isFinishCombo = false)
    {
        Score += CurrentRule.BaseScore;

        if (isFinishCombo)
        {
            _winCount++;
            if (_winCount >= CurrentRule.WinCountToNextLevel)
            {
                _ruleIndex++;
                _winCount = 0;
            }
        }
    }

    public void MinusScore()
    {
        Score -= (int)(CurrentRule.BaseScore * 1.3);
        _winCount = 0;
        if (Score < 0)
        {
            Score = 0;
        }
    }
}

public enum DifficultyClasses
{
    Sprout,
    Beginner,
    Intermidiate,
    Advanced,
    Elite,
    SuperElite
}

public class ScoreRule
{
    public DifficultyClasses Class { get; set; }
    public int MoleTime { get; set; }
    public int MatchCount { get; set; }
    public int MoleCount { get; set; }
    public int MoleSlot { get; set; }
    public int WinCountToNextLevel { get; set; }
    public int BaseScore { get; set; }
}