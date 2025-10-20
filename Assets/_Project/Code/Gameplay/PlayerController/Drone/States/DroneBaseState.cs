using UnityEngine;

using _Project.Code.Core.Events;
using _Project.Code.Core.StateMachine;
using _Project.Code.Gameplay.Input;


namespace _Project.Code.Gameplay.PlayerController.Drone.States
{
    public class DroneBaseState : BaseState
    {
        protected readonly DroneController _controller;
        protected FiniteStateMachine<IState> _stateMachine => _controller.StateMachine;

        private static bool _isFiring;

        //===============================================================================================================================================

        protected DroneBaseState(DroneController controller) { _controller = controller; }

        public override void Enter()
        {
            EventBus.Instance?.Subscribe<MoveInputEvent>(this, HandleMove);
            EventBus.Instance?.Subscribe<FireInputEvent>(this, HandleFire);
        }

        protected virtual void HandleMove(MoveInputEvent evt)
        {
            _controller.MoveInput = evt.Input;
        }

        private void HandleFire(FireInputEvent evt)
        {
            _isFiring = evt.isFiring;
        }

        public override void Update()
        {
            if (_isFiring)
                _controller.FireProjectile();
        }

        public override void Exit()
        {
            EventBus.Instance?.Unsubscribe<MoveInputEvent>(this);
        }
    }
}