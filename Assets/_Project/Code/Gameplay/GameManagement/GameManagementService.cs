using UnityEngine;

using _Project.Code.Core.ServiceLocator;
using _Project.Code.Core.StateMachine;
using _Project.Code.Core.Events;
using _Project.Code.Gameplay.Input;


namespace _Project.Code.Gameplay.GameManagement
{
    public class GameManagementService : MonoBehaviourService
    {
        private FiniteStateMachine<IState> _stateMachine;

        public IState CurrentState => _stateMachine?.CurrentState;

        public override void Initialize()
        {
            var gameplayState = new MenuState(this);
            _stateMachine = new FiniteStateMachine<IState>(gameplayState);

            _stateMachine.AddState(new PausedState(this));
            _stateMachine.AddState(new GameplayState(this));

            EventBus.Instance.Subscribe<PauseInputEvent>(this, HandlePauseInput);
        }

        private void HandlePauseInput(PauseInputEvent evt)
        {
            if (_stateMachine.CurrentState is GameplayState)
            {
                TransitionToPaused();
            }
            else if (_stateMachine.CurrentState is PausedState)
            {
                TransitionToGameplay();
            }
        }

        public void TransitionToGameplay() => _stateMachine.TransitionTo<GameplayState>();
        public void TransitionToPaused() => _stateMachine.TransitionTo<PausedState>();
        public void TransitionToMenu() => _stateMachine.TransitionTo<MenuState>();

        private void Update()
        {
            _stateMachine?.Update();
        }

        public override void Dispose()
        {
            EventBus.Instance?.Unsubscribe<PauseInputEvent>(this);
        }



        public Color EColorToColor(EColor color)
        {
            switch (color)
            {
                case EColor.White:
                    return Color.HSVToRGB(0.0f, 0.0f, 1.0f);

                case EColor.Red:
                    return Color.HSVToRGB(0.0f, 1.0f, 1.0f);

                case EColor.Blue:
                    return Color.HSVToRGB(0.55f, 1.0f, 1.0f);

                case EColor.Yellow:
                    return Color.HSVToRGB(0.166f, 1.0f, 1.0f);

                case EColor.Random:
                    return EColorToColor((EColor)Random.Range(0, 4));

                default:
                    return Color.black;
            }
        }

        public Color EBossColorToColor(EBossColor color)
        {
            switch (color)
            {
                case EBossColor.Green:
                    return Color.HSVToRGB(0.36f, 1.0f, 1.0f);

                case EBossColor.Purple:
                    return Color.HSVToRGB(0.83f, 1.0f, 1.0f);

                case EBossColor.Orange:
                    return Color.HSVToRGB(0.097f, 1.0f, 1.0f);

                case EBossColor.Random:
                    return EBossColorToColor((EBossColor)Random.Range(0, 3));

                default:
                    return Color.black;
            }
        }
    }
}

public enum EColor
{
    White,
    Red,
    Blue,
    Yellow,
    Random
}

public enum EBossColor
{
    Green,
    Purple,
    Orange,
    Random
}