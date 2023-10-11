using System;
using System.Collections.Generic;

namespace SpriteAnimations.Performers
{
    public class PerformerFactory
    {
        private SpriteAnimator _animator;
        private Dictionary<Type, AnimationPerformer> _handlers = new();

        public PerformerFactory(SpriteAnimator animator)
        {
            _animator = animator;
        }

        public AnimationPerformer GetPerformer(SpriteAnimation animation)
        {
            if (!_handlers.TryGetValue(animation.PerformerType, out AnimationPerformer handler))
            {
                handler = Fabricate(animation);
            }

            return handler;
        }

        protected AnimationPerformer Fabricate(SpriteAnimation animation)
        {
            AnimationPerformer performer = Activator.CreateInstance(animation.PerformerType) as AnimationPerformer;
            performer.Animator = _animator;

            _handlers.Add(animation.PerformerType, performer);

            return performer;
        }
    }
}
