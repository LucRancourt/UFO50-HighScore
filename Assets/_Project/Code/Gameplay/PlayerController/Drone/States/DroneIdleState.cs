using UnityEngine;

using _Project.Code.Gameplay.Input;


namespace _Project.Code.Gameplay.PlayerController.Drone.States
{
    public class DroneIdleState : DroneBaseState
    {
        public DroneIdleState(DroneController controller) : base(controller) { }

        protected override void HandleMove(MoveInputEvent evt)
        {
            base.HandleMove(evt);

            if (_controller.IsMovingForward || _controller.IsMovingBackward || _controller.IsMovingRight || _controller.IsMovingLeft)
            {
                _stateMachine.TransitionTo<DroneMovingState>();
            }
        }
    }
}