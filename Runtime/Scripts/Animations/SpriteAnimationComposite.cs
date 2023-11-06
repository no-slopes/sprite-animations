
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpriteAnimations
{
    public class SpriteAnimationComposite : SpriteAnimation
    {
        #region Inspector

        [SerializeField]
        private Cycle _antecipationCycle;

        [SerializeField]
        private Cycle _coreCycle;

        [SerializeField]
        private Cycle _recoveryCycle;

        [SerializeField]
        private bool _loopableCore;

        #endregion

        #region Getters

        public Cycle AntecipationCycle { get => _antecipationCycle; set => _antecipationCycle = value; }
        public Cycle CoreCycle { get => _coreCycle; set => _coreCycle = value; }
        public Cycle RecoveryCycle { get => _recoveryCycle; set => _recoveryCycle = value; }
        public bool IsLoopableCore { get => _loopableCore; set => _loopableCore = value; }

        public override Type PerformerType => typeof(CompositeAnimator);
        public override AnimationType AnimationType => AnimationType.Composite;

        #endregion

        #region Cycles        

        /// <summary>
        /// This must be executed upon the asset creation
        /// </summary>
        public void GenerateCycles()
        {
            _antecipationCycle = new Cycle(this);
            _coreCycle = new Cycle(this);
            _recoveryCycle = new Cycle(this);
        }

        #endregion

        #region Calculations

        public override int CalculateFramesCount()
        {
            return _antecipationCycle.Size + _coreCycle.Size + _recoveryCycle.Size;
        }

        #endregion
    }
}