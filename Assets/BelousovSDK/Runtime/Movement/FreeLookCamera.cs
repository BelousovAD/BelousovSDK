using BelousovSDK.Inputs;
using UnityEngine;

namespace BelousovSDK.Movement
{
    internal class FreeLookCamera : MonoBehaviour
    {
        [SerializeField] private BaseInput _input;
        [SerializeField] private Transform _camera;
        [SerializeField, Min(0.001f)] private float _sphereRadius = 0.2f;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField, Min(0f)] private float _preferredDistance = 2f;
        [SerializeField, Range(-89f, 0f)] private float _minVerticalAngle = -89f;
        [SerializeField, Range(1f, 89f)] private float _maxVerticalAngle = 89f;
        [SerializeField] private Transform _target;
        
        private float _horizontalAngle;
        private float _verticalAngle;

        private void OnEnable() =>
            _input.RotateRequested += Rotate;

        private void OnDisable() =>
            _input.RotateRequested -= Rotate;

        private void LateUpdate()
        {
            transform.position = _target.position;
            _camera.localPosition = Physics.SphereCast(
                new Ray(transform.position, -transform.forward),
                _sphereRadius,
                out RaycastHit hit,
                _preferredDistance,
                _layerMask)
                ? new Vector3(0f, 0f, -hit.distance + _sphereRadius)
                : new Vector3(0f, 0f, -_preferredDistance);
        }

        private void Rotate(Vector2 delta)
        {
            _horizontalAngle += delta.x;
            _verticalAngle -= delta.y;
            _verticalAngle = Mathf.Clamp(_verticalAngle, _minVerticalAngle, _maxVerticalAngle);
            transform.rotation = Quaternion.Euler(_verticalAngle, _horizontalAngle, 0f);
        }
    }
}