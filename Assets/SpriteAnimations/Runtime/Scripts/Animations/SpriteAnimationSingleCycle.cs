using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace SpriteAnimations
{
    [CreateAssetMenu(fileName = "Single Cycle Animation", menuName = "Sprite Animations/Single Cycle Animation")]
    public class SpriteAnimationSingleCycle : SpriteAnimation
    {
        #region Editor

        /// <summary>
        /// If the animation should loop
        /// </summary>
        [SerializeField]
        protected bool _isLoopable = false;

        /// <summary>
        /// The animation Cycle
        /// </summary>
        [SerializeField]
        protected Cycle _cycle;

        #endregion  

        #region Getters

        public bool IsLoopable { get => _isLoopable; set => _isLoopable = value; }
        public Cycle Cycle => _cycle;

        public override Type PerformerType => typeof(SingleCycleAnimator);
        public override AnimationType AnimationType => AnimationType.SingleCycle;

        #endregion

        #region Cycle

        /// <summary>
        /// This must be executed upon the asset creation
        /// </summary>
        public void GenerateCycle()
        {
            _cycle = new Cycle(this);
        }

        #endregion

        #region Calculations

        public override int CalculateFramesCount()
        {
            return _cycle.Size;
        }

        #endregion

        #region Templating

        /// <summary>
        /// Creates a new <see cref="SpriteAnimationSingleCycle"/>  using this instance as a template.
        /// </summary>
        /// <param name="sprites">The list of sprites to be assigned to the frames of the new animation.</param>
        /// <returns>A new <see cref="SpriteAnimationSingleCycle"/> with the assigned sprites.</returns>
        public SpriteAnimationSingleCycle UseAsTemplate(List<Sprite> sprites)
        {
            // Check if the number of frames in the animation matches the number of sprites in the list
            if (sprites.Count != _cycle.Size)
            {
                Logger.LogError($"The number of frames in the animation ({_cycle.Size}) " +
                    $"does not match the number of frames in the given list of sprites).", this);
                return null;
            }

            // Create a clone of this instance
            SpriteAnimationSingleCycle clone = Instantiate(this);

            // Assign each sprite in the list to the corresponding frame in the clone
            for (int i = 0; i < clone.Cycle.Size; i++)
            {
                // Skip frames that cannot be retrieved
                if (!clone.Cycle.TryGetFrame(i, out Frame frame)) continue;

                // Assign the sprite to the frame
                frame.Sprite = sprites[i];
            }

            return clone;
        }

        #endregion
    }
}