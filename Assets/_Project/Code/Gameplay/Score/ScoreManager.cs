using UnityEngine;

using _Project.Code.Core.General;
using TMPro;
using _Project.Code.Gameplay.GameManagement;
using _Project.Code.Core.Events;

public class ScoreManager : Singleton<ScoreManager>
{
    [SerializeField] private TextMeshProUGUI text;
    private int _currentScore = 0;


    private void Start()
    {
        EventBus.Instance?.Subscribe<GameStateChangedEvent>(this, ResetScore);
    }

    public void ResetScore(GameStateChangedEvent evt)
    {
        if (evt.StateName != "Gameplay") return;

        _currentScore = 0;
        text.SetText(_currentScore.ToString());
    }

    public void AddScore(int score)
    {
        _currentScore += score;
        text.SetText(_currentScore.ToString());
    }
}
