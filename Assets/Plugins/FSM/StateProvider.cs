using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HandyFSM
{
    public class StateProvider
    {
        #region Fields

        protected Dictionary<Type, IState> _states;
        protected StateMachine _machine;

        #endregion

        #region Constructors

        public StateProvider(StateMachine machine)
        {
            _machine = machine;
            _states = new Dictionary<Type, IState>();
        }

        #endregion

        #region Loading States

        /// <summary>
        /// Loads the states derived from the provided base state type into 
        /// the provider.
        /// </summary>
        /// <param name="baseStateType">The base state type.</param>
        public void LoadStatesFromBaseType<T>() where T : State
        {
            LoadStatesFromBaseType(typeof(T));
        }

        /// <summary>
        /// Loads the states derived from the provided base state type into 
        /// the provider.
        /// </summary>
        /// <param name="baseStateType">The base state type.</param>
        public void LoadStatesFromBaseType(Type baseStateType, bool initializeAfterCommit = true)
        {

            // Get all the classes that derive from the base state type and are not abstract
            IEnumerable<Type> childrenTypes = baseStateType.Assembly.GetTypes()
                .Where(t => t.IsClass && baseStateType.IsAssignableFrom(t) && !t.IsAbstract);

            // List to hold instantiated states
            List<State> instatiatedState = new();

            // Instantiate and add each child state to the list and dictionary
            foreach (Type childType in childrenTypes)
            {
                State childState = Activator.CreateInstance(childType) as State;
                instatiatedState.Add(childState);
                CommitState(childType, childState);
            }

            if (!initializeAfterCommit) return;

            // Initialize each instantiated state
            instatiatedState.ForEach(state => state.Initialize(_machine));
        }

        /// <summary>
        /// Receives a list of scriptable states, commits all of them and initializes them.
        /// </summary>
        /// <param name="states">The list of scriptable states to load.</param>
        public void LoadStatesFromScriptablesList(List<ScriptableState> states, bool initializeAfterCommit = true)
        {
            List<IState> commitedStates = new();

            // Instantiate and commit each scriptable state
            foreach (ScriptableState scriptableState in states)
            {
                ScriptableState clone = UnityEngine.Object.Instantiate(scriptableState);

                // Commit the state
                if (!CommitState(clone.GetType(), clone)) continue;
                commitedStates.Add(clone);
            }

            if (!initializeAfterCommit) return;

            // We can only initialize the states after commiting them because they might have
            // transitions wich need other states to work.
            foreach (IState commitedState in commitedStates)
            {
                commitedState.Initialize(_machine);
            }
        }

        public void InitializeAllStates()
        {
            foreach (IState state in _states.Values)
            {
                state.Initialize(_machine);
            }
        }

        /// <summary>
        /// Loads a state of the specified type into the provider.
        /// </summary>
        /// <param name="stateType">The type of the state to load.</param>
        public void LoadState<T>() where T : State
        {
            LoadState(typeof(T));
        }

        /// <summary>
        /// Loads a state of the specified type into the provider.
        /// </summary>
        /// <param name="stateType">The type of the state to load.</param>
        public void LoadState(Type stateType)
        {
            // Create an instance of the specified state type
            State state = Activator.CreateInstance(stateType) as State;

            // Commit the state
            CommitState(stateType, state);

            // Initialize the state with the state machine
            state.Initialize(_machine);
        }

        public void LoadState(IState state)
        {
            CommitState(state.GetType(), state);
            state.Initialize(_machine);
        }

        /// <summary>
        /// Commits the state for the given type addind it to the dictionary.
        /// </summary>
        /// <param name="type">The type to commit the state for.</param>
        /// <param name="state">The state to be committed.</param>
        protected bool CommitState(Type type, IState state)
        {
            if (_states.ContainsKey(type)) return false;

            // Add the state to the dictionary
            _states.Add(type, state);
            return true;
        }

        #endregion

        #region Serving

        public bool IsLoaded(IState state)
        {
            return _states.ContainsKey(state.GetType());
        }

        public IState Get(Type stateType)
        {
            if (_states.TryGetValue(stateType, out IState state))
            {
                return state;
            }

            return null;
        }

        public T Get<T>() where T : IState
        {
            return (T)Get(typeof(T));
        }

        public bool TryGet(Type type, out IState state)
        {
            return _states.TryGetValue(type, out state);
        }

        public bool TryGet<T>(out IState state) where T : IState
        {
            return _states.TryGetValue(typeof(T), out state);
        }

        #endregion
    }
}