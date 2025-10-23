using UnityEngine;

using _Project.Code.Core.General;

public class ScoreManager : Singleton<ScoreManager>
{
    private int _currentScore = 0;

    public void ResetScore()
    {
        _currentScore = 0;
    }

    public void AddScore(int score)
    {
        _currentScore += score;
    }
}
