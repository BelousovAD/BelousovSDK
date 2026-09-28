using System;
using System.Collections.Generic;
using System.Linq;
using BelousovSDK.FiniteStateMachine.Predicates;

namespace BelousovSDK.FiniteStateMachine
{
    public class StateMachine
    {
        private readonly HashSet<Transition> _anyTransitions = new ();
        private readonly Dictionary<StateType, StateNode> _nodes = new ();
        private StateNode _current;

        public event Action StateChanged;

        public State Current => _current?.State;

        public void Update()
        {
            Transition transition = GetActivatedTransition();

            if (transition is not null)
            {
                SwitchStateTo(transition.To.Type);
            }
        }

        public void AddAnyTransition(State to, IPredicate condition) =>
            _anyTransitions.Add(new Transition(GetOrAddNode(to).State, condition));

        public void AddTransition(State from, State to, IPredicate condition) =>
            GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);

        public void SetFirstState(StateType type)
        {
            if (_current is null)
            {
                _current = _nodes[type];
                StateChanged?.Invoke();
            }
        }

        private StateNode GetOrAddNode(State state)
        {
            StateNode node = _nodes.GetValueOrDefault(state.Type);

            if (node is null)
            {
                node = new StateNode(state);
                _nodes.Add(state.Type, node);
            }

            return node;
        }

        private void SwitchStateTo(StateType type)
        {
            if (_current.State.Type == type)
            {
                return;
            }
            
            _current = _nodes[type];
            StateChanged?.Invoke();
        }

        private Transition GetActivatedTransition() =>
            CheckTransitions(_anyTransitions) ?? CheckTransitions(_current.Transitions);

        private Transition CheckTransitions(IEnumerable<Transition> transitions) =>
            transitions.FirstOrDefault(transition => transition.Condition.Check());

        private class StateNode
        {
            public StateNode(State state)
            {
                State = state;
                Transitions = new HashSet<Transition>();
            }
            
            public State State { get; }
            
            public HashSet<Transition> Transitions { get; }

            public void AddTransition(State to, IPredicate predicate) =>
                Transitions.Add(new Transition(to, predicate));
        }
    }
}