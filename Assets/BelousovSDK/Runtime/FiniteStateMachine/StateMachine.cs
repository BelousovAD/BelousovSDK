using System;
using System.Collections.Generic;
using System.Linq;
using BelousovSDK.FiniteStateMachine.Predicates;

namespace BelousovSDK.FiniteStateMachine
{
    public class StateMachine
    {
        private readonly HashSet<Transition> _anyTransitions = new ();
        private readonly Dictionary<IState, StateNode> _nodes = new ();
        private StateNode _current;

        public event Action StateChanged;

        public IState Current => _current?.State;

        public void Update()
        {
            Transition transition = GetActivatedTransition();

            if (transition is not null)
            {
                SwitchStateTo(transition.To);
            }
        }

        public void AddAnyTransition(IState to, IPredicate condition) =>
            _anyTransitions.Add(new Transition(GetOrAddNode(to).State, condition));

        public void AddTransition(IState from, IState to, IPredicate condition) =>
            GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);

        public void SetFirstState(IState state)
        {
            if (_current is null)
            {
                _current = _nodes[state];
                StateChanged?.Invoke();
            }
        }

        private StateNode GetOrAddNode(IState state)
        {
            StateNode node = _nodes.GetValueOrDefault(state);

            if (node is null)
            {
                node = new StateNode(state);
                _nodes.Add(state, node);
            }

            return node;
        }

        private void SwitchStateTo(IState state)
        {
            if (ReferenceEquals(_current.State, state))
            {
                return;
            }
            
            _current = _nodes[state];
            StateChanged?.Invoke();
        }

        private Transition GetActivatedTransition() =>
            CheckTransitions(_anyTransitions) ?? CheckTransitions(_current.Transitions);

        private Transition CheckTransitions(IEnumerable<Transition> transitions) =>
            transitions.FirstOrDefault(transition => transition.Condition.Check());

        private class StateNode
        {
            public StateNode(IState state)
            {
                State = state;
                Transitions = new HashSet<Transition>();
            }
            
            public IState State { get; }
            
            public HashSet<Transition> Transitions { get; }

            public void AddTransition(IState to, IPredicate predicate) =>
                Transitions.Add(new Transition(to, predicate));
        }
    }
}