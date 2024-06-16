public partial class Scorer
{
    public int Score { get; set; }
    private int _winCount { get; set; }
    public int MoleTime { get; private set; }

    public void Reset()
    {
        _winCount = 0;
        Score = 0;
        MoleTime = 8;
    }

    public void AddScore(int addedWinCount = 0)
    {
        Score += 3;
        // _winCount += addedWinCount;
        // if (_winCount % 3 == 0 && MoleTime > 1)
        // {
        //     MoleTime -= 1;
        // }
    }

    public void MinusScore()
    {
        Score -= 4;
        if (Score < 0)
        {
            Score = 0;
        }
    }
}
