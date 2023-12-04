
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpriteAnimations
{
    public class SpriteAnimationComposite : SpriteAnimation
    {
        #region Static

        /// <summary>
        /// Creates a SpriteAnimationComposite animation on demand.
        /// </summary>
        /// <param name="fps">The frames per second of the animation.</param>
        /// <param name="antecipationCycle">The list of sprites for the antecipation cycle.</param>
        /// <param name="coreCycle">The list of sprites for the core cycle.</param>
        /// <param name="recoveryCycle">The list of sprites for the recovery cycle.</param>
        /// <returns>The created SpriteAnimationComposite object.</returns>
        public static SpriteAnimationComposite OnDemand(int fps, List<Sprite> antecipationCycle, List<Sprite> coreCycle, List<Sprite> recoveryCycle)
        {
            // Create a new instance of SpriteAnimationComposite
            var animation = CreateInstance<SpriteAnimationComposite>();

            // Set the frames per second of the animation
            animation.FPS = fps;

            // Generate the cycles for the animation
            animation.GenerateCycles();

            // Add frames to the antecipation cycle
            foreach (Sprite sprite in antecipationCycle)
            {
                animation.AntecipationCycle.AddFrame(sprite);
            }

            // Add frames to the core cycle
            foreach (Sprite sprite in coreCycle)
            {
                animation.CoreCycle.AddFrame(sprite);
            }

            // Add frames to the recovery cycle
            foreach (Sprite sprite in recoveryCycle)
            {
                animation.RecoveryCycle.AddFrame(sprite);
            }

            // Return the created animation
            return animation;
        }

        #endregion

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

        public void SetFrameID(CompositeCycle compositeCycle, int index, string id)
        {
            Cycle cycle = compositeCycle switch
            {
                CompositeCycle.Antecipation => _antecipationCycle,
                CompositeCycle.Core => _coreCycle,
                CompositeCycle.Recovery => _recoveryCycle,
                _ => _antecipationCycle,
            };

            if (!cycle.TryGetFrame(index, out Frame frame))
            {
                Logger.LogError($"Trying to override the ID of frame {index} but index is out of range.", this);
                return;
            }

            frame.Id = id;
        }

        #endregion

        #region Calculations

        public override int CalculateFramesCount()
        {
            return _antecipationCycle.Size + _coreCycle.Size + _recoveryCycle.Size;
        }

        #endregion

        #region Templating

        /// <summary>
        /// Creates a new SpriteAnimationComposite instance using this animation as template.
        /// </summary>
        /// <param name="antecipationCycle">The antecipation cycle template.</param>
        /// <param name="coreCycle">The core cycle template.</param>
        /// <param name="recoveryCycle">The recovery cycle template.</param>
        /// <returns>A new SpriteAnimationComposite instance.</returns>
        public SpriteAnimationComposite UseAsTemplate(List<Sprite> antecipationCycle, List<Sprite> coreCycle, List<Sprite> recoveryCycle)
        {
            // Create a clone of this instance
            SpriteAnimationComposite clone = Instantiate(this);

            // Evaluate and adjust the size of the antecipation cycle
            EvaluateCycleSize(CompositeCycle.Antecipation, antecipationCycle.Count, _antecipationCycle.Size);
            for (int i = 0; i < clone.AntecipationCycle.Size; i++)
            {
                if (i > antecipationCycle.Count - 1) continue;
                if (!clone.AntecipationCycle.TryGetFrame(i, out Frame frame)) continue;
                frame.Sprite = antecipationCycle[i];
            }

            // Evaluate and adjust the size of the core cycle
            EvaluateCycleSize(CompositeCycle.Core, coreCycle.Count, _coreCycle.Size);
            for (int i = 0; i < clone.CoreCycle.Size; i++)
            {
                if (i > coreCycle.Count - 1) continue;
                if (!clone.CoreCycle.TryGetFrame(i, out Frame frame)) continue;
                frame.Sprite = coreCycle[i];
            }

            // Evaluate and adjust the size of the recovery cycle
            EvaluateCycleSize(CompositeCycle.Recovery, recoveryCycle.Count, _recoveryCycle.Size);
            for (int i = 0; i < clone.RecoveryCycle.Size; i++)
            {
                if (i > recoveryCycle.Count - 1) continue;
                if (!clone.RecoveryCycle.TryGetFrame(i, out Frame frame)) continue;
                frame.Sprite = recoveryCycle[i];
            }
            /// <summary>
            /// Evaluates the size of the provided cycle and logs a warning if it doesn't match the original cycle size.
            /// </summary>
            /// <param name="compositeCycle">The composite cycle being evaluated.</param>
            /// <param name="newCycleSize">The new cycle size.</param>
            /// <param name="originalCycleSize">The original cycle size.</param>
            void EvaluateCycleSize(CompositeCycle compositeCycle, int newCycleSize, int originalCycleSize)
            {
                if (newCycleSize != originalCycleSize)
                {
                    Logger.LogWarning($"The number of frames for the Composite Cycle {compositeCycle} ({newCycleSize}) "
                    + $"does not match the number of frames in the template's cycle ({originalCycleSize}). "
                    + $"The animation might not work as expected.", this);
                }
            }
            return clone;
        }

        #endregion
    }

    public enum CompositeCycle
    {
        Antecipation,
        Core,
        Recovery,
    }
}