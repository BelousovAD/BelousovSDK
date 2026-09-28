using UnityEngine;

namespace BelousovSDK.FiniteStateMachine
{
    public abstract class StateMachineProvider : MonoBehaviour
    {
        public StateMachine StateMachine { get; } = new ();

        protected virtual void Awake() =>
            ConfigureStateMachine();

        protected virtual void Update() =>
            StateMachine.Update();

        protected abstract void ConfigureStateMachine();
    }
}