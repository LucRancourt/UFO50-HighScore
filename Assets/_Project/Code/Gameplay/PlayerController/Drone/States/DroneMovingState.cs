using UnityEngine;

using _Project.Code.Gameplay.Input;


namespace _Project.Code.Gameplay.PlayerController.Drone.States
{
    public class DroneMovingState : DroneBaseState
    {
        public DroneMovingState(DroneController controller) : base(controller) { }

        public override void FixedUpdate()
        {
            ExecuteMovement();
        }


        private void ExecuteMovement()
        {
            var targetSpeed = _controller.MovementProfile.WalkSpeed;
            var moveDirection = _controller.MoveInput;

            _controller.Motor.Move(moveDirection, targetSpeed);
        }

        protected override void HandleMove(MoveInputEvent evt)
        {
            base.HandleMove(evt);

            if (!_controller.IsMovingForward && !_controller.IsMovingBackward && !_controller.IsMovingRight && !_controller.IsMovingLeft)
            {
                _controller.Motor.Move(Vector2.zero, 0.0f);
                _stateMachine.TransitionTo<DroneIdleState>();
            }
        }
    }
}