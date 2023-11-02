using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpriteAnimations
{
    /// <summary>
    /// The Combo animation. This animation can have multiple cycles
    /// and they can be executed sequentially. Awesome for waiting for input
    /// from the player.
    /// </summary>
    [CreateAssetMenu(fileName = "Combo Animation", menuName = "Sprite Animations/Combo Animation")]
    public class SpriteAnimationCombo : SpriteAnimation
    {
        #region Static

        /// <summary>
        /// Creates a sprite animation combo on demand.
        /// </summary>
        /// <param name="fps">The frames per second of the animation.</param>
        /// <param name="cycles">The list of cycles, each containing a list of sprites.</param>
        /// <param name="waitingTime">The waiting time between cycles.</param>
        /// <returns>The created sprite animation combo.</returns>
        public static SpriteAnimationCombo OnDemand(int fps, List<List<Sprite>> cycles, float waitingTime = 1.25f)
        {
            // Create a new instance of SpriteAnimationCombo
            var animation = CreateInstance<SpriteAnimationCombo>();

            // Set the waiting time and frames per second of the animation
            animation.WaitingTime = waitingTime;
            animation.FPS = fps;

            // Create cycles and add frames to each cycle
            foreach (List<Sprite> sprites in cycles)
            {
                Cycle cycle = animation.CreateCycle();
                cycle.Animation = animation;

                // Add each sprite as a frame to the cycle
                foreach (Sprite sprite in sprites)
                {
                    cycle.AddFrame(sprite);
                }
            }

            return animation;
        }

        #endregion

        #region Editor

        /// <summary>
        /// The waiting time between cycles before the animation is interrupted
        /// </summary>
        [SerializeField]
        protected float _waitingTime = 1.25f;

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

        #region Templating

        /// <summary>
        /// Creates a new <see cref="SpriteAnimationCombo"/> using the provided cycles as a template.
        /// </summary>
        /// <param name="cycles">The list of cycles to use as a template.</param>
        /// <returns>A new <see cref="SpriteAnimationCombo"/> instance.</returns>
        public SpriteAnimationCombo UseAsTemplate(List<List<Sprite>> cycles)
        {
            var clone = Instantiate(this);

            for (int i = 0; i < clone.Cycles.Count; i++)
            {
                if (!TryGetCycle(i, out Cycle originalCycle))
                {
                    Logger.LogError($"Cycle under index {i} was not found in the template", this);
                    continue;
                }

                LengthCheck(i, cycles[i].Count, originalCycle.Size);

                Cycle cloneCycle = clone.GetCycle(i);

                for (int frameIndex = 0; frameIndex < cloneCycle.Size; frameIndex++)
                {
                    if (!cloneCycle.TryGetFrame(frameIndex, out Frame frame))
                    {
                        Logger.LogError($"Frame under index {frameIndex} for cycle {i} was not found", this);
                        continue;
                    }

                    frame.Sprite = cycles[i][frameIndex];
                }
            }

            // Check if the length of the cycle matches the template's cycle length
            void LengthCheck(int index, int length, int originalLength)
            {
                if (length == originalLength) return;

                Logger.LogWarning($"The length of the cycle under index {index} does not match the template's cycle length ({originalLength}). "
                + $"The animation might not work as expected.", this);
            }

            return clone;
        }

        #endregion
    }
}