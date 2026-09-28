using Reflex.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BelousovSDK.Inputs
{
    internal class PlayerInput : BaseInput
    {
        private Input _input;

        [Inject]
        private void Initialize(Input input) =>
            _input = input;

        private void OnEnable()
        {
            _input.Enable();
            _input.Player.Move.performed += RequestMove;
            _input.Player.Move.canceled += RequestMove;
            _input.Player.Look.performed += RequestRotate;
            _input.Player.Look.canceled += RequestRotate;
            _input.Player.Jump.performed += RequestJump;
            _input.Player.Attack.performed += RequestAttack;
            _input.Player.Attack.canceled += RequestAttack;
        }

        private void OnDisable()
        {
            _input.Disable();
            _input.Player.Move.performed -= RequestMove;
            _input.Player.Move.canceled -= RequestMove;
            _input.Player.Look.performed -= RequestRotate;
            _input.Player.Look.canceled -= RequestRotate;
            _input.Player.Jump.performed -= RequestJump;
            _input.Player.Attack.performed -= RequestAttack;
            _input.Player.Attack.canceled -= RequestAttack;
        }

        private void RequestMove(InputAction.CallbackContext context) =>
            RequestMove(context.ReadValue<Vector2>());

        private void RequestRotate(InputAction.CallbackContext context) =>
            RequestRotate(context.ReadValue<Vector2>());

        private void RequestJump(InputAction.CallbackContext context) =>
            RequestJump();

        private void RequestAttack(InputAction.CallbackContext context) =>
            RequestAttack(context.ReadValueAsButton());
    }
}
