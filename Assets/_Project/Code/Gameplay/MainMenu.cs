
using _Project.Code.Core.Events;
using _Project.Code.Gameplay.GameManagement;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button StartButton;
    [SerializeField] private GameObject MainMenuPanel;
    [SerializeField] private GameObject ControlsPage;

    private void Start()
    {
        StartButton.onClick.AddListener(StartGame);
        EventBus.Instance.Subscribe<GameStateChangedEvent>(this, OpenMenu);
    }

    private void OpenMenu(GameStateChangedEvent evt)
    {
        if (evt.StateName == "Menu")
            MainMenuPanel.SetActive(true);
    }

    private void StartGame()
    {
        ControlsPage.SetActive(true);
        MainMenuPanel.SetActive(false);
    }
}
