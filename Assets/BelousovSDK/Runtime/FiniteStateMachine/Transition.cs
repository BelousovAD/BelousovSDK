using BelousovSDK.FiniteStateMachine.Predicates;

namespace BelousovSDK.FiniteStateMachine
{
    internal class Transition
    {
        public Transition(State to, IPredicate condition)
        {
            To = to;
            Condition = condition;
        }
        
        public State To { get; }
        
        public IPredicate Condition { get; }
    }
}