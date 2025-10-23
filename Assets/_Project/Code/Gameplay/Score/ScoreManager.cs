using UnityEngine;

using _Project.Code.Core.General;
using TMPro;
using _Project.Code.Gameplay.GameManagement;
using _Project.Code.Core.Events;

public class ScoreManager : Singleton<ScoreManager>
{
    [SerializeField] private TextMeshProUGUI textHUDScore;
    [SerializeField] private TextMeshProUGUI textHighscore;

    private const string HighscoreKey = "Highscore";

    private int _currentScore = 0;
    private int _highScore = 0;


    private void Start()
    {
        _highScore = PlayerPrefs.GetInt(HighscoreKey);

        UpdateHighScore();

        EventBus.Instance?.Subscribe<GameStateChangedEvent>(this, ResetScore);
    }

    public void ResetScore(GameStateChangedEvent evt)
    {
        if (evt.StateName == "Gameplay")
        {
            _currentScore = 0;
            textHUDScore.SetText(_currentScore.ToString());
        }
        else if (evt.StateName == "Menu")
        {
            UpdateHighScore();
        }
    }

    private void UpdateHighScore()
    {
        if (_currentScore > _highScore)
        {
            PlayerPrefs.SetInt(HighscoreKey, _currentScore);
            PlayerPrefs.Save();

            _highScore = _currentScore;
        }

        textHighscore.SetText(_highScore.ToString());
    }

    public void AddScore(int score)
    {
        _currentScore += score;
        textHUDScore.SetText(_currentScore.ToString());
    }
}
