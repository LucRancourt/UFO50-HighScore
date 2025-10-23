using System;
using UnityEngine;
using UnityEngine.InputSystem;

using _Project.Code.Core.ServiceLocator;
using _Project.Code.Core.Events;


namespace _Project.Code.Gameplay.Input
{
    public class InputService : MonoBehaviourService
    {
        private PlayerInputActions _inputActions;

        [SerializeField] private InputProfile _profile;

        public InputProfile Profile => _profile;

        public override void Initialize()
        {
            _inputActions = new PlayerInputActions();

            _inputActions.Gameplay.Move.performed += HandleMovePerformed;
            _inputActions.Gameplay.Move.canceled += HandleMoveCanceled;
            //_inputActions.Gameplay.Look.performed += HandleLookPerformed;
            //_inputActions.Gameplay.Look.canceled += HandleLookCanceled;
            //_inputActions.Gameplay.Interact.performed += HandleInteractPerformed;
            _inputActions.Gameplay.Fire.performed += HandleFirePerformed;
            _inputActions.Gameplay.Fire.canceled += HandleFireCanceled;
            _inputActions.Gameplay.Fire2.performed += HandleFire2Performed;
            _inputActions.Gameplay.Fire2.canceled += HandleFire2Canceled;

            _inputActions.Gameplay.Enable();
            
        }

        private void HandleMovePerformed(InputAction.CallbackContext context)
        {
            EventBus.Instance.Publish(new MoveInputEvent { Input = context.ReadValue<Vector2>() });
        }

        private void HandleMoveCanceled(InputAction.CallbackContext context)
        {
            EventBus.Instance.Publish(new MoveInputEvent { Input = Vector2.zero });
        }

        private void HandleFirePerformed(InputAction.CallbackContext context)
        {
            EventBus.Instance.Publish(new FireInputEvent { isFiring = true });
        }
        private void HandleFireCanceled(InputAction.CallbackContext context)
        {
            EventBus.Instance.Publish(new FireInputEvent { isFiring = false });
        }

        private void HandleFire2Performed(InputAction.CallbackContext context)
        {
            EventBus.Instance.Publish(new Fire2InputEvent { });
        }
        private void HandleFire2Canceled(InputAction.CallbackContext context)
        {
            EventBus.Instance.Publish(new Fire2InputEvent { });
        }
        public void EnableGameplayActions()
        {
            _inputActions.Gameplay.Enable();
            _inputActions.UI.Disable();
        }

        public void EnableUIActions()
        {
            _inputActions.Gameplay.Disable();
            _inputActions.UI.Enable();
        }

        public void DisableAllActions()
        {
            _inputActions.Gameplay.Disable();
            _inputActions.UI.Disable();
        }

        public override void Dispose()
        {
            if (_inputActions != null)
            {
                _inputActions.Gameplay.Move.performed -= HandleMovePerformed;
                _inputActions.Gameplay.Move.canceled -= HandleMoveCanceled;
                //_inputActions.Gameplay.Look.performed -= HandleLookPerformed;
                //_inputActions.Gameplay.Look.canceled -= HandleLookCanceled;
                //_inputActions.Gameplay.Interact.performed -= HandleInteractPerformed;
                _inputActions.Gameplay.Fire.performed -= HandleFirePerformed;
                _inputActions.Gameplay.Fire.canceled -= HandleFireCanceled;
                _inputActions.Gameplay.Fire2.performed -= HandleFire2Performed;
                _inputActions.Gameplay.Fire2.canceled -= HandleFire2Canceled;

                _inputActions.Gameplay.Disable();
                _inputActions.Dispose();
            }
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}
