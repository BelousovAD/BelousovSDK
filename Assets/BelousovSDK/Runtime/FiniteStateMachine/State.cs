namespace BelousovSDK.FiniteStateMachine
{
    public class State
    {
        public State(StateType type) =>
            Type = type;
        
        public StateType Type { get; }
    }
}