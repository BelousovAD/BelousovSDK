using BelousovSDK.Inputs;
using UnityEngine;

namespace BelousovSDK.Movement
{
    internal class Mover : MonoBehaviour
    {
        [SerializeField] private Transform _root;
        [SerializeField] private Transform _model;
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private BaseInput _input;
        [SerializeField] private Transform _cameraRig;
        [SerializeField][Min(0f)] private float _moveSpeed = 1f;
        [SerializeField][Min(0f)] private float _jumpHeight = 2f;
        [SerializeField][Min(0f)] private float _jumpBufferSeconds = 0.12f;
        [SerializeField] private float _gravity = -9.81f;
        
        private Vector3 _horizontalVelocity;
        private Vector3 _verticalVelocity;
        private float _jumpRequestedUntil;

        private void OnEnable()
        {
            _horizontalVelocity = Vector3.zero;
            _verticalVelocity = Vector3.zero;
            _input.MoveRequested += UpdateHorizontalVelocity;
            _input.JumpRequested += BufferJump;
            _jumpRequestedUntil = 0f;
        }

        private void OnDisable()
        {
            _input.MoveRequested -= UpdateHorizontalVelocity;
            _input.JumpRequested -= BufferJump;
            _jumpRequestedUntil = 0f;
        }

        private void Update()
        {
            if (_horizontalVelocity != Vector3.zero)
            {
                _root.forward = Vector3.ProjectOnPlane(_cameraRig.forward, Vector3.up).normalized;
                _model.forward = _root.rotation * _horizontalVelocity.normalized;
            }
        }

        private void FixedUpdate()
        {
            if (_characterController.isGrounded && _verticalVelocity.y < 0f)
            {
                _verticalVelocity.y = 0f;
            }

            ConsumeJump();
            _verticalVelocity.y += _gravity * Time.fixedDeltaTime;
            
            if (_horizontalVelocity + _verticalVelocity != Vector3.zero)
            {
                _characterController.Move(_root.rotation * (_horizontalVelocity + _verticalVelocity) *
                                          Time.fixedDeltaTime);
            }
        }

        private void UpdateHorizontalVelocity(Vector2 direction) =>
            _horizontalVelocity = new Vector3(direction.x, 0f, direction.y) * _moveSpeed;

        private void BufferJump() =>
            _jumpRequestedUntil = Time.time + _jumpBufferSeconds;

        private void ConsumeJump()
        {
            if (_characterController.isGrounded && _verticalVelocity.y <= 0f &&
                Time.time <= _jumpRequestedUntil)
            {
                _jumpRequestedUntil = 0f;
                _verticalVelocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
            }
        }
    }
}
