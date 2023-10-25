using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HandyFSM
{
    /// <summary>
    /// Represents a State controlled by the StateMachine class.
    /// </summary>
    [DefaultExecutionOrder(200)]
    public abstract class State : IState
    {
        #region Fields

        private string _name;
        private bool _interruptible;
        private StateMachine _machine;
        private List<StateTransition> _transitions = new();

        #endregion

        #region Getters

        /// <summary>
        /// Either the state name defined on Inspector or the type of the state class
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// If the state can be interrupted.
        /// </summary>
        public bool Interruptible => _interruptible;

        public StateMachine Machine => _machine;

        #endregion

        #region Cycle Methods

        /// <summary>
        /// This will be called before the  method.
        /// </summary>
        public virtual void Load(StateMachine machine)
        {
            _machine = machine;
            _name = GetType().Name;
            SortTransitions();
            OnLoad();
        }

        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public virtual void Tick() { }
        public virtual void FixedTick() { }
        public virtual void LateTick() { }

        protected virtual void OnLoad() { }

        #endregion

        #region Collision Methods

        public virtual void OnCollisionEnter2D(Collision2D collision) { }
        public virtual void OnCollisionStay2D(Collision2D collision) { }
        public virtual void OnCollisionExit2D(Collision2D collision) { }
        public virtual void OnTriggerEnter2D(Collider2D other) { }
        public virtual void OnTriggerStay2D(Collider2D other) { }
        public virtual void OnTriggerExit2D(Collider2D other) { }

        #endregion        

        #region Transitions

        /// <summary>
        /// Adds a transition into the available transitions of this state
        /// </summary>
        /// <param name="Condition"> A bool returning callback wich evaluates if the state should become active or not </param>
        /// <param name="targetState"> The state wich should become active based on condition </param>
        /// <param name="priority"> Priority level </param>
        protected virtual void AddTransition(Func<bool> Condition, State targetState, int priority = 0)
        {
            StateTransition transition = new(Condition, targetState, priority);
            AddTransition(transition);
        }

        /// <summary>
        /// Adds a transition into the available transitions of this state
        /// </summary>
        /// <param name="transition"> The State Trasitions to be add </param>
        protected virtual void AddTransition(StateTransition transition)
        {
            _transitions.Add(transition);
        }

        /// <summary>
        /// Sorts transitions based on priority. Descending
        /// </summary>
        public virtual void SortTransitions()
        {
            _transitions.OrderByDescending(transition => transition.Priority).ToList();
        }

        /// <summary>
        /// Checks if there is a valid transition and sets the output parameter with the target state.
        /// </summary>
        /// <param name="state">The target state if a valid transition is found.</param>
        /// <returns>True if a valid transition is found, otherwise false.</returns>
        public bool ShouldTransition(out IState state)
        {
            // Initialize the output parameter
            state = null;

            // Iterate through each transition
            for (int i = 0; i < _transitions.Count; i++)
            {
                // Get the current transition
                StateTransition transition = _transitions[i];

                // Check if the condition for the transition is met
                if (!transition.ConditionMet()) continue;

                // Set the output parameter to the target state
                state = transition.TargetState;

                // Return true to indicate a successful transition
                return true;
            }

            // No transition condition was met, return false
            return false;
        }

        #endregion

        #region Configurations

        protected void SetName(string name)
        {
            _name = name;
        }

        protected void SetInterruptible(bool interruptible)
        {
            _interruptible = interruptible;
        }

        #endregion
    }
}