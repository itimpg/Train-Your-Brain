public partial class Scorer
{
    public int Score { get; set; }

    public void Reset()
    {
        Score = 0;
    }

    public void AddScore()
    {
        Score += 3;
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
