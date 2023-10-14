using System;
using System.Collections.Generic;

namespace SpriteAnimations
{
    /// <summary>
    /// The factory responsible for creating animation performers for the SpriteAnimator
    /// </summary>
    public class PerformerFactory
    {
        private SpriteAnimator _animator;
        private Dictionary<Type, AnimationPerformer> _performer = new();

        /// <summary>
        /// Creates a new performer factory
        /// </summary>
        /// <param name="animator"></param>
        public PerformerFactory(SpriteAnimator animator)
        {
            _animator = animator;
        }


        /// <summary>
        /// Retrieves an animator of type TAnimator for the specified sprite animation.
        /// </summary>
        /// <typeparam name="TAnimator">The type of the animator.</typeparam>
        /// <param name="animation">The sprite animation.</param>
        /// <returns>An animator of type TAnimator.</returns>
        public TAnimator Get<TAnimator>(SpriteAnimation animation) where TAnimator : AnimationPerformer
        {
            // Check if the performer dictionary contains the specified performer type
            if (!_performer.TryGetValue(animation.PerformerType, out AnimationPerformer performer))
            {
                // If not found, fabricate a new performer
                performer = Fabricate(animation);
            }

            // Return the performer as TAnimator
            return performer as TAnimator;
        }


        /// <summary>
        /// Retrieves the AnimationPerformer for the given SpriteAnimation.
        /// If the performer is not found, a new one is created.
        /// </summary>
        /// <param name="animation">The SpriteAnimation to retrieve the performer for.</param>
        /// <returns>The AnimationPerformer for the given SpriteAnimation.</returns>
        public AnimationPerformer Get(SpriteAnimation animation)
        {
            // Check if the performer is already cached
            if (!_performer.TryGetValue(animation.PerformerType, out AnimationPerformer performer))
            {
                // If not, create a new performer
                performer = Fabricate(animation);
            }

            return performer;
        }

        /// <summary>
        /// Fabricates an AnimationPerformer for the given SpriteAnimation type.
        /// </summary>
        /// <param name="animation">The SpriteAnimation to create an AnimationPerformer for.</param>
        /// <returns>The fabricated AnimationPerformer.</returns>
        protected AnimationPerformer Fabricate(SpriteAnimation animation)
        {
            // Create an instance of the AnimationPerformer using the PerformerType specified in the animation
            AnimationPerformer performer = Activator.CreateInstance(animation.PerformerType) as AnimationPerformer;
            // Set the Animator property of the performer to the _animator field
            performer.Animator = _animator;
            // Add the performer to the _performer dictionary using the PerformerType as the key
            _performer.Add(animation.PerformerType, performer);
            // Return the fabricated performer
            return performer;
        }
    }
}
