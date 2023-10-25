using System;

namespace HandyFSM
{
    public class StateTransition
    {
        #region Fields

        private Func<bool> _condition;
        private IState _targetState;
        private int _priority;

        #endregion

        #region Properties

        /// <summary>
        /// The condition wich evaluates if transition should be made
        /// </summary>
        public Func<bool> ConditionMet => _condition;
        public IState TargetState => _targetState;
        public int Priority => _priority;

        #endregion

        #region Constructors

        public StateTransition(Func<bool> condition, IState targetState, int priority = 0)
        {
            _condition = condition;
            _targetState = targetState;
            _priority = priority;
        }

        #endregion
    }
}