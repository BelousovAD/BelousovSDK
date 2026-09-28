using System;
using UnityEngine;

namespace BelousovSDK.Inputs
{
    public abstract class BaseInput : MonoBehaviour
    {
        public event Action<Vector2> MoveRequested;

        public event Action<Vector2> RotateRequested;

        public event Action JumpRequested;

        public event Action<bool> AttackRequested;

        protected void RequestMove(Vector2 value) =>
            MoveRequested?.Invoke(value);

        protected void RequestRotate(Vector2 value) =>
            RotateRequested?.Invoke(value);

        protected void RequestJump() =>
            JumpRequested?.Invoke();

        protected void RequestAttack(bool value) =>
            AttackRequested?.Invoke(value);
    }
}
