using System.Collections.Generic;
using BelousovSDK.FiniteStateMachine;
using UnityEngine;
using StateMachine = BelousovSDK.FiniteStateMachine.StateMachine;

namespace BelousovSDK.Animation
{
    internal class AnimationCaller : MonoBehaviour
    {
        private readonly Dictionary<StateType, AnimationKey> _keys = new ()
        {
            [StateType.Idle] = AnimationKey.Idle,
            [StateType.Attack] = AnimationKey.Attack,
            [StateType.Jump] = AnimationKey.Jump,
            [StateType.Run] = AnimationKey.Run,
        };

        [SerializeField] private StateMachineProvider _stateMachineProvider;
        [SerializeField] private CustomAnimator _animator;

        private StateMachine _stateMachine;

        private void Awake() =>
            _stateMachine = _stateMachineProvider.StateMachine;

        private void OnEnable()
        {
            _stateMachine.StateChanged += CallAnimation;
            CallAnimation();
        }

        private void OnDisable() =>
            _stateMachine.StateChanged -= CallAnimation;

        private void CallAnimation()
        {
            if (_stateMachine.Current is not null &&
                _keys.TryGetValue(_stateMachine.Current.Type, out AnimationKey key))
            {
                _animator.Play(key);
            }
        }
    }
}