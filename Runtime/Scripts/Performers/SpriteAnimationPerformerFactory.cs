using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpriteAnimations.Performers
{
    public class SpriteAnimationPerformerFactory
    {
        private SpriteAnimator _animator;
        private Dictionary<Type, SpriteAnimationPerformer> _handlers = new();

        public SpriteAnimationPerformerFactory(SpriteAnimator animator)
        {
            _animator = animator;
        }

        public SpriteAnimationPerformer GetPerformer(SpriteAnimation animation)
        {
            if (!_handlers.TryGetValue(animation.PerformerType, out SpriteAnimationPerformer handler))
            {
                handler = Fabricate(animation);
            }

            return handler;
        }

        protected SpriteAnimationPerformer Fabricate(SpriteAnimation animation)
        {
            SpriteAnimationPerformer performer = Activator.CreateInstance(animation.PerformerType) as SpriteAnimationPerformer;
            performer.Animator = _animator;

            _handlers.Add(animation.PerformerType, performer);

            return performer;
        }
    }
}
