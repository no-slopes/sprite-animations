using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
        public void LoadStatesFromBaseType(Type baseStateType)
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

            // Initialize each instantiated state
            instatiatedState.ForEach(state => state.Load(_machine));
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
            state.Load(_machine);
        }

        public void LoadState(IState state)
        {
            CommitState(state.GetType(), state);
            state.Load(_machine);
        }

        /// <summary>
        /// Commits the state for the given type addind it to the dictionary.
        /// </summary>
        /// <param name="type">The type to commit the state for.</param>
        /// <param name="state">The state to be committed.</param>
        protected void CommitState(Type type, IState state)
        {
            if (_states.ContainsKey(type)) return;

            // Add the state to the dictionary
            _states.Add(type, state);
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