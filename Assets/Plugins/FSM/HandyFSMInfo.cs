using UnityEngine;
using UnityEngine.Events;

namespace HandyFSM
{
    [CreateAssetMenu(fileName = "State Announcer", menuName = "Handy Tools/FSM/Info", order = 0)]
    public class StateAnnouncer : ScriptableObject
    {
        #region Inspector

        [SerializeField]
        private UnityEvent<State> _stateChanged;

        #endregion

        #region Fields

        private State _currentState;

        #endregion

        #region Getters

        public bool HasCurrentState => _currentState != null;
        public State CurrentState => _currentState;
        public UnityEvent<State> StateChanged => _stateChanged;

        #endregion

        #region Announcing

        public void AnnounceStateChange(State state)
        {
            _currentState = state;
            _stateChanged?.Invoke(state);
        }

        #endregion
    }
}