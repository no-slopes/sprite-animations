using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace SpriteAnimations
{
    /// <summary>
    /// The Single Cycle Animation. This animation has only a single cycle.
    /// </summary>
    [CreateAssetMenu(fileName = "Single Cycle Animation", menuName = "Sprite Animations/Single Cycle Animation")]
    public class SpriteAnimationSingleCycle : SpriteAnimation
    {
        #region Static

        /// <summary>
        /// Creates a new <see cref="SpriteAnimationSingleCycle"/> on demand.
        /// </summary>
        /// <param name="fps"></param>
        /// <param name="cycle"></param>
        /// <param name="isLoopable"></param>
        /// <returns></returns>
        public static SpriteAnimationSingleCycle OnDemand(int fps, List<Sprite> cycle, bool isLoopable = false)
        {
            var animation = CreateInstance<SpriteAnimationSingleCycle>();
            animation.FPS = fps;

            animation.IsLoopable = isLoopable;
            animation.GenerateCycle();
            animation.Cycle.Animation = animation;

            foreach (Sprite sprite in cycle)
            {
                animation.Cycle.AddFrame(sprite);
            }
            return animation;
        }

        #endregion

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

        /// <summary>
        /// If the animation should loop
        /// </summary>
        public bool IsLoopable { get => _isLoopable; set => _isLoopable = value; }

        /// <summary>
        /// The animation Cycle
        /// </summary>
        public Cycle Cycle => _cycle;

        public override Type PerformerType => typeof(SingleCycleAnimator);
        public override AnimationType AnimationType => AnimationType.SingleCycle;

        #endregion

        #region Cycle

        /// <summary>
        /// Sets a frame ID overriding the possible existent one. Note that if you are doing this while using
        /// the UnityEditor and this is not an OnDemand animation, the ScriptableObject
        /// will save the ID meaning that the previous ID will be lost.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="id"></param>
        public void SetFrameID(int index, string id)
        {
            if (!_cycle.TryGetFrame(index, out Frame frame))
            {
                Logger.LogError($"Trying to override the ID of frame {index} but index is out of range.", this);
                return;
            }

            frame.Id = id;
        }

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
        /// <param name="cycle">The list of sprites to be assigned to the frames of the new animation.</param>
        /// <returns>A new <see cref="SpriteAnimationSingleCycle"/> with the assigned sprites.</returns>
        public SpriteAnimationSingleCycle UseAsTemplate(List<Sprite> cycle)
        {
            // Check if the number of frames in the animation matches the number of sprites in the list
            if (cycle.Count != _cycle.Size)
            {
                Logger.LogWarning($"The number of frames in the animation ({_cycle.Size}) " +
                    $"does not match the number of frames in the given list of sprites).", this);
            }

            // Create a clone of this instance
            SpriteAnimationSingleCycle clone = Instantiate(this);

            // Assign each sprite in the list to the corresponding frame in the clone
            for (int i = 0; i < clone.Cycle.Size; i++)
            {
                // Skip frames that cannot be retrieved
                if (!clone.Cycle.TryGetFrame(i, out Frame frame)) continue;

                // Assign the sprite to the frame
                frame.Sprite = cycle[i];
            }

            return clone;
        }

        #endregion
    }
}