using UnityEngine;
using TMPro;

namespace HandyFSM
{
    public class StateDebugger : MonoBehaviour
    {
        #region Inspector

        [SerializeField]
        private TextMeshProUGUI _stateTmpro;

        [SerializeField]
        private StateAnnouncer _stateAnnouncer;

        #endregion

        #region Fields

        private State _currentState;

        #endregion

        #region Behaviour

        protected void Awake()
        {
            DefineState(null);
        }

        private void OnEnable()
        {
            if (_stateAnnouncer.HasCurrentState)
            {
                _currentState = _stateAnnouncer.CurrentState;
            }

            _stateAnnouncer.StateChanged.AddListener(DefineState);
        }

        private void OnDisable()
        {
            _stateAnnouncer.StateChanged?.RemoveListener(DefineState);
        }

        #endregion

        #region Handling State

        private void DefineState(State state)
        {
            _currentState = state;

            if (_currentState != null)
            {
                _stateTmpro.text = _currentState.Name;
            }
            else
            {
                _stateTmpro.text = string.Empty;
            }
        }

        #endregion
    }
}