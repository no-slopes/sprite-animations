using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpriteAnimations
{
    [CreateAssetMenu(fileName = "Combo Animation", menuName = "Sprite Animations/Combo Animation")]
    public class SpriteAnimationCombo : SpriteAnimation
    {
        #region Editor

        /// <summary>
        /// The waiting time between cycles before the animation is interrupted
        /// </summary>
        [SerializeField]
        protected float _waitingTime = 0.75f;

        /// <summary>
        /// The animation cycles
        /// </summary>
        [SerializeField]
        protected List<Cycle> _cycles = new();

        #endregion  

        #region Properties

        /// <summary>
        /// The waiting time between cycles before the animation is interrupted
        /// </summary>
        public float WaitingTime { get => _waitingTime; set => _waitingTime = value; }

        public List<Cycle> Cycles { get => _cycles; set => _cycles = value; }

        #endregion

        #region Getters

        public override Type PerformerType => typeof(ComboAnimator);
        public override AnimationType AnimationType => AnimationType.Combo;

        public Cycle FirstCycle => _cycles.FirstOrDefault();

        #endregion

        #region Cycles

        /// <summary>
        /// Creates a new cycle and adds it to the collection.
        /// </summary>
        /// <returns>The created cycle.</returns>
        public Cycle CreateCycle()
        {
            // Create a new instance of the Cycle class
            Cycle newCycle = new(this);

            // Add the new cycle to the collection
            _cycles.Add(newCycle);

            // Return the newly added cycle
            return newCycle;
        }

        /// <summary>
        /// Removes a cycle from the list of cycles.
        /// </summary>
        /// <param name="cycle">The cycle to remove.</param>
        public void RemoveCycle(Cycle cycle)
        {
            _cycles.Remove(cycle);
        }

        /// <summary>
        /// Remove a cycle from the list of cycles.
        /// </summary>
        /// <param name="index">The index of the cycle to be removed.</param>
        public void RemoveCycle(int index)
        {
            _cycles.RemoveAt(index);
        }

        /// <summary>
        /// Get the cycle at the specified index.
        /// </summary>
        /// <param name="index">The index of the cycle to retrieve.</param>
        /// <returns>The cycle at the specified index, or null if the index is out of range.</returns>
        public Cycle GetCycle(int index)
        {
            // Check if the index is out of range
            if (index >= _cycles.Count)
            {
                // Log a warning with the out of range index
                Logger.LogWarning($"Trying to get cycle at {index} but index is out of range.", this);
                return null;
            }

            // Return the cycle at the specified index
            return _cycles.ElementAt(index);
        }

        /// <summary>
        /// Tries to get a cycle from the list of cycles at the specified index.
        /// </summary>
        /// <param name="index">The index of the cycle to retrieve.</param>
        /// <param name="cycle">The retrieved cycle.</param>
        /// <returns>True if the cycle was retrieved successfully, false otherwise.</returns>
        public bool TryGetCycle(int index, out Cycle cycle)
        {
            // Get the number of cycles in the list
            int cycleCount = _cycles.Count;

            // If the index is greater than or equal to the cycle count,
            // set the output parameter to null and return false
            if (index >= cycleCount)
            {
                cycle = null;
                return false;
            }

            // Get the cycle at the specified index and set it as the output parameter
            cycle = _cycles.ElementAt(index);
            return true;
        }

        #endregion

        #region Calculations

        public override int CalculateFramesCount()
        {
            return _cycles.Sum(cycle => cycle.Size);
        }

        #endregion
    }
}