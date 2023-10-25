using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace HandyFSM
{
    /// <summary>
    /// The state machine
    /// </summary>
    [DefaultExecutionOrder(100)]
    public class StateMachine : MonoBehaviour
    {
        #region Fields

        /// <summary>
        /// The current machine's status of the MachineStatus enum type. 
        /// </summary>
        [BoxGroup("Status")]
        [LabelText("Current status")]
        [Tooltip("The current machine's status. Should be On, Off or Paused")]
        [SerializeField]
        [ReadOnly]
        protected MachineStatus _status = MachineStatus.Off;

        /// <summary>
        /// The current state name
        /// </summary>
        [BoxGroup("Status")]
        [LabelText("Current State")]
        [Tooltip("The machine's current state name")]
        [ReadOnly]
        [SerializeField]
        [ShowIf("ShowCurrentState")]
        protected string _currentStateName;

        [BoxGroup("Configuration")]
        [EnumToggleButtons]
        [SerializeField]
        protected InitializationMode _initalizationMode = InitializationMode.Automatic;

        [BoxGroup("Configuration")]
        [SerializeField]
        protected List<ScriptableState> _scriptableStates;

        [BoxGroup("Configuration")]
        [SerializeField]
        protected ScriptableState _defaultScriptableState;

        [BoxGroup("Events")]
        [SerializeField]
        protected UnityEvent<MachineStatus> _statusChanged;

        [BoxGroup("Events")]
        [SerializeField]
        protected UnityEvent<IState> _stateChanged;

        [BoxGroup("Events")]
        [BoxGroup("Events/External")]
        [SerializeField]
        protected StateAnnouncer _stateAnnouncer;

        #endregion

        #region Fields        

        protected Type _machineType;
        protected Type _defaultStateType;

        protected IState _defaultState;

        protected IState _currentState;
        protected IState _previousState;

        protected StateProvider _stateProvider;

        #endregion

        #region Getters

        /// <summary>
        /// If the machine is on
        /// </summary>
        public bool IsOn => _status.Equals(MachineStatus.On);

        /// <summary>
        /// If the machine is paused
        /// </summary>
        public bool IsPaused => _status.Equals(MachineStatus.Paused);

        /// <summary>
        /// If the machine is off
        /// </summary>
        public bool IsOff => _status.Equals(MachineStatus.Off);

        /// <summary>
        /// If the machine is working. Either On or Paused
        /// </summary>
        public bool IsWorking => IsOn || IsPaused;

        /// <summary>
        /// A getter for the machine's Status
        /// </summary>
        public MachineStatus Status => _status;

        /// <summary>
        /// This is the current active state for the this State Machine
        /// </summary>
        public IState CurrentState => _currentState;

        /// <summary>
        /// This is the immediate previous state the machine was in.
        /// </summary>
        public IState PreviousState => _previousState;

        /// <summary>
        /// Getter for the machine's default state
        /// </summary>
        public IState DefaultState => _defaultState;

        /// <summary>
        /// If CurrentStateName should be shown in the inspector
        /// </summary>
        protected bool ShowCurrentState => !Status.Equals(MachineStatus.Off);

        // Events

        /// <summary>
        /// Whenever the machine status changes
        /// </summary>
        public UnityEvent<MachineStatus> StatusChanged => _statusChanged;

        /// <summary>
        /// Whenever the current state changes
        /// </summary>
        public UnityEvent<IState> StateChanged => _stateChanged;

        #endregion

        #region Behaviour   

        protected virtual void Awake()
        {
            _status = MachineStatus.Off;

            _machineType = GetType();
            _stateProvider = new StateProvider(this);

            RecognizeAndInitializeStates();
        }

        protected virtual void Start()
        {
            if (_defaultState != null && !_initalizationMode.Equals(InitializationMode.Automatic)) return;
            TurnOn(_defaultState);
        }

        protected virtual void Update()
        {
            if (!_status.Equals(MachineStatus.On)) return;

            EvaluateTransition();
            _currentState?.Tick();
        }

        protected virtual void LateUpdate()
        {
            if (!_status.Equals(MachineStatus.On)) return;

            EvaluateTransition();
            _currentState?.LateTick();
        }

        protected virtual void FixedUpdate()
        {
            if (!_status.Equals(MachineStatus.On)) return;

            EvaluateTransition();
            _currentState?.FixedTick();
        }

        protected virtual void OnDisable()
        {
            _currentState?.OnExit(); // Exiting current state
        }

        #endregion

        #region Machine Engine

        /// <summary>
        /// This method recognizes and initializes the states for the machine.
        /// </summary>
        protected void RecognizeAndInitializeStates()
        {
            _stateProvider.LoadStatesFromScriptablesList(_scriptableStates, false);

            // Get the PropertyInfo for the LoadableStateType property of the machine type
            // for the case of this class being inherited.
            PropertyInfo loadableStateInfo = _machineType.GetProperty("LoadableStateType");
            if (loadableStateInfo != null)
            {
                // Get the loadable state type and load states from that base type
                Type baseStateType = (Type)loadableStateInfo.GetValue(this);
                _stateProvider.LoadStatesFromBaseType(baseStateType, false);
            }

            if (_defaultScriptableState != null)
            {
                // If a default scriptable state is set, get the state from the state provider
                _defaultState = _stateProvider.Get(_defaultScriptableState.GetType());
            }
            else
            {
                // Get the PropertyInfo for the DefaultStateType property of the machine type
                PropertyInfo defaultStateInfo = _machineType.GetProperty("DefaultStateType");
                if (defaultStateInfo != null)
                {
                    // Get the default state type and get the state from the state provider
                    _defaultStateType = (Type)defaultStateInfo.GetValue(this);
                    _defaultState = _stateProvider.Get(_defaultStateType);
                    return;
                }
            }

            // Initialize all the states
            // This should occur after the states have been loaded so they can recognize each other
            // when using Machine.GetState<>();
            _stateProvider.InitializeAllStates();
        }

        /// <summary>
        /// Turns the machine on and enters the given state
        /// </summary>
        /// <param name="stateType"></param>
        public virtual void TurnOn(Type stateType)
        {
            if (!_stateProvider.TryGet(stateType, out IState state))
            {
                Debug.LogError($"Trying to turn machine on but {stateType.Name} is not loaded.", this);
                return;
            }

            TurnOn(state);
        }

        /// <summary>
        /// Turns the machine on and enters the given state
        /// </summary>
        /// <param name="state"></param>
        public virtual void TurnOn(IState state)
        {
            if (IsWorking)
            {
                Debug.LogError($"Trying to turn machine on but it is already working", this);
                return;
            }

            if (!_stateProvider.IsLoaded(state))
            {
                Debug.LogError($"Trying to turn machine on but {nameof(state)} is not loaded.", this);
                return;
            }

            ChangeState(state);
            ChangeStatus(MachineStatus.On);
        }

        /// <summary>
        /// Pauses the machine
        /// </summary>
        public virtual void Resume()
        {
            if (!IsPaused) return;
            ChangeStatus(MachineStatus.On);
        }

        /// <summary>
        /// Pauses the machine
        /// </summary>
        public virtual void Pause()
        {
            if (!IsOn) return;
            ChangeStatus(MachineStatus.Paused);
        }

        /// <summary>
        /// Stops the machine
        /// </summary>
        public virtual void Stop()
        {
            if (!IsWorking) return;

            _currentState?.OnExit();
            _currentState = null;

            ChangeStatus(MachineStatus.Off);
        }

        /// <summary>
        /// Changes the status of the machine
        /// </summary>
        /// <param name="status"></param>
        public virtual void ChangeStatus(MachineStatus status)
        {
            _status = status;
            _statusChanged?.Invoke(_status);
        }

        #endregion

        #region Providing The machine

        /// <summary>
        /// Casts the current instance to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to cast to.</typeparam>
        /// <returns>The instance casted to the specified type.</returns>
        public T As<T>() where T : StateMachine
        {
            return this as T;
        }

        #endregion

        #region Machine's Logic

        /// <summary>
        /// Defines a given state as active
        /// </summary>
        /// <param name="state"> The state to be set as active </param>
        /// <param name="forceInterruption"> If an uninterruptible state should be interrupted </param>
        public virtual void RequestStateChange(IState state, StateChangeMode mode = StateChangeMode.Respectfully)
        {
            if (_status != MachineStatus.On || state == null) return;

            if (_currentState != null && !_currentState.Interruptible && mode.Equals(StateChangeMode.Respectfully)) return; // State cannot be interrupted, but will if forced.

            ChangeState(state);
        }

        /// <summary>
        /// Defines a given state as active
        /// </summary>
        /// <param name="state"> The state to be set as active </param>
        /// <param name="forceInterruption"> If an uninterruptible state should be interrupted </param>
        public virtual void RequestStateChange<T>(StateChangeMode mode = StateChangeMode.Respectfully) where T : State
        {
            if (!_stateProvider.TryGet<T>(out IState state))
            {
                Debug.LogError($"A state under the Type {nameof(T)} was requested but it is not present int the state factory ", this);
                return;
            }

            RequestStateChange(state, mode);
        }

        /// <summary>
        /// Ends the current state of the machine.
        /// </summary>
        /// <param name="target">The target state to change to. If null, the default state will be used.</param>
        public virtual void EndState(IState target = null)
        {
            // Check if the machine is turned on
            if (_status != MachineStatus.On)
            {
                Debug.LogError($"Trying to end state on a machine that is not turned on. ", this);
                return;
            }

            // Change to the target state if provided
            if (target != null)
            {
                ChangeState(target);
                return;
            }

            // Change to the default state if available
            if (_defaultState != null)
            {
                ChangeState(_defaultState);
                return;
            }

            // Invoke the exit action of the current state
            _currentState?.OnExit();
        }

        /// <summary>
        /// Ends the specified state of type T.
        /// </summary>
        /// <typeparam name="T">The type of the state.</typeparam>
        public virtual void EndState<T>() where T : IState
        {
            // Check if the requested state of type T exists in the state factory
            if (!_stateProvider.TryGet<T>(out IState state))
            {
                Debug.LogError($"A state under the Type {nameof(T)} was requested but it is not present in the state factory ", this);
                return;
            }

            // End the specified state
            EndState(state);
        }

        /// <summary>
        /// Changes the state.
        /// </summary>
        /// <param name="state">The new state to change to.</param>
        protected virtual void ChangeState(IState state)
        {
            // Do not change state if it is the same as the current state or null
            if (state == _currentState || state == null) return;

            // Define the previous state
            _previousState = _currentState;

            // Invoke the exit action of the current state
            _currentState?.OnExit();

            // Change the current state
            _currentState = state;

            // Announce the new state
            _stateChanged.Invoke(_currentState);

            // Invoke the enter action of the new state
            _currentState.OnEnter();

            // Update the current state name
            _currentStateName = CurrentState.Name;
        }

        /// <summary>
        /// Handles the tick event.
        /// </summary>
        protected virtual void EvaluateTransition()
        {
            if (_currentState == null) return;

            // Evaluate the next state
            if (_currentState.ShouldTransition(out IState targetState))
            {
                RequestStateChange(targetState);
            }
        }

        #endregion

        #region Collisions

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            _currentState?.OnCollisionEnter2D(collision);
        }

        protected virtual void OnCollisionStay2D(Collision2D collision)
        {
            _currentState?.OnCollisionStay2D(collision);
        }

        protected virtual void OnCollisionExit2D(Collision2D collision)
        {
            _currentState?.OnCollisionExit2D(collision);
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            _currentState?.OnTriggerEnter2D(other);
        }

        protected virtual void OnTriggerStay2D(Collider2D other)
        {
            _currentState?.OnTriggerStay2D(other);
        }

        protected virtual void OnTriggerExit2D(Collider2D other)
        {
            _currentState?.OnTriggerExit2D(other);
        }

        #endregion

        #region Providing States

        /// <summary>
        /// Load states from the specified state type.
        /// </summary>
        /// <param name="stateType">The type of the states to load.</param>
        public void LoadStatesOfType(Type stateType)
        {
            _stateProvider.LoadStatesFromBaseType(stateType);
        }

        /// <summary>
        /// Loads and initalizes states from a list of ScriptableState objects
        /// </summary>
        /// <param name="states">A list of ScriptableState objects to load.</param>
        public void LoadStatesFromScriptablesList(List<ScriptableState> states)
        {
            _stateProvider.LoadStatesFromScriptablesList(states, true);
        }
        /// <summary>
        /// Loads the state of the given type.
        /// </summary>
        /// <param name="stateType">The type of the state to load.</param>
        public void LoadState(Type stateType)
        {
            _stateProvider.LoadState(stateType);
        }

        /// <summary>
        /// Loads the state by passing it to the state provider.
        /// </summary>
        /// <param name="state">The state object to be loaded.</param>
        public void LoadState(IState state)
        {
            _stateProvider.LoadState(state);
        }

        /// <summary>
        /// Retrieves the state of type T loaded into this machine.
        /// </summary>
        /// <typeparam name="T">The type of state to retrieve.</typeparam>
        /// <returns>The state of type T.</returns>
        public T GetState<T>() where T : IState
        {
            // Use the _stateProvider to get the state of type T
            return _stateProvider.Get<T>();
        }

        /// <summary>
        /// Tries to get the state of type T loaded into this machine.
        /// </summary>
        /// <typeparam name="T">The type of the state to get.</typeparam>
        /// <param name="state">The retrieved state of type T.</param>
        /// <returns>True if the state was successfully retrieved, false otherwise.</returns>
        public bool TryGetState<T>(out IState state) where T : IState
        {
            return _stateProvider.TryGet<T>(out state);
        }

        #endregion

        #region Enums

        protected enum InitializationMode
        {
            Automatic,
            Manual
        }

        #endregion
    }

}
