using _Project.Code.Core.ServiceLocator;
using _Project.Code.Gameplay.GameManagement;
using UnityEngine;
using UnityEngine.UI;

public class StartGameButton : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(StartGame);
    }

    private void StartGame()
    {
        ServiceLocator.Get<GameManagementService>().TransitionToGameplay();
    }
}
