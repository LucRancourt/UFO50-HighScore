using UnityEngine;
using _Project.Code.Core.ServiceLocator;
using _Project.Code.Gameplay.GameManagement;
using UnityEngine.UI;

public class ControlsPanelStartButton : MonoBehaviour
{
    [SerializeField] private GameObject ControlsPage;
    [SerializeField] private Button StartGameButton;

    private void Start()
    {
        ControlsPage.SetActive(false);
        StartGameButton.onClick.AddListener(StartGame);
    }

    private void StartGame()
    {
        ServiceLocator.Get<GameManagementService>().TransitionToGameplay();
        ControlsPage.SetActive(false);
    }
}
