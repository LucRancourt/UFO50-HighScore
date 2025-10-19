using UnityEngine;

using _Project.Code.Core.Events;
using _Project.Code.Core.ServiceLocator;
using _Project.Code.Core.StateMachine;
using _Project.Code.Gameplay.Input;


namespace _Project.Code.Gameplay.PlayerController._Base
{
    public abstract class BasePlayerController : MonoBehaviour
    {
        public FiniteStateMachine<IState> StateMachine { get; protected set; }

        protected virtual void Start()
        {
            Initialize();
        }

        protected virtual void Update()
        {
            StateMachine?.Update();
        }

        protected virtual void FixedUpdate()
        {
            StateMachine?.FixedUpdate();
        }

        public abstract void Initialize();

        protected virtual void OnDestroy()
        {
        }

        protected void PublishEvent<T>(T eventData) where T : IEvent
        {
            EventBus.Instance.Publish(eventData);
        }
    }
}
