using BelousovSDK.FiniteStateMachine.Predicates;

namespace BelousovSDK.FiniteStateMachine
{
    internal class Transition
    {
        public Transition(IState to, IPredicate condition)
        {
            To = to;
            Condition = condition;
        }
        
        public IState To { get; }
        
        public IPredicate Condition { get; }
    }
}