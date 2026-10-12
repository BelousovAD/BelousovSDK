using System;
using System.Collections.Generic;
using BelousovSDK.Animation.States;
using BelousovSDK.FiniteStateMachine;
using UnityEngine;
using StateMachine = BelousovSDK.FiniteStateMachine.StateMachine;

namespace BelousovSDK.Animation
{
    internal class AnimationCaller : MonoBehaviour
    {
        private readonly Dictionary<Type, AnimationKey> _keys = new ()
        {
            [typeof(IIdleState)] = AnimationKey.Idle,
            [typeof(IAttackState)] = AnimationKey.Attack,
            [typeof(IJumpState)] = AnimationKey.Jump,
            [typeof(IRunState)] = AnimationKey.Run,
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
                _keys.TryGetValue(_stateMachine.Current.GetType(), out AnimationKey key))
            {
                _animator.Play(key);
            }
        }
    }
}