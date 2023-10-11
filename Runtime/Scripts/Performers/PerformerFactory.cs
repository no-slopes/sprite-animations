using System;
using System.Collections.Generic;

namespace SpriteAnimations.Performers
{
    public class PerformerFactory
    {
        private SpriteAnimator _animator;
        private Dictionary<Type, AnimationPerformer> _performer = new();

        public PerformerFactory(SpriteAnimator animator)
        {
            _animator = animator;
        }

        public TAnimator Get<TAnimator>(SpriteAnimation animation) where TAnimator : AnimationPerformer
        {
            if (!_performer.TryGetValue(animation.PerformerType, out AnimationPerformer performer))
            {
                performer = Fabricate(animation);
            }

            return performer as TAnimator;
        }


        public AnimationPerformer Get(SpriteAnimation animation)
        {
            if (!_performer.TryGetValue(animation.PerformerType, out AnimationPerformer performer))
            {
                performer = Fabricate(animation);
            }

            return performer;
        }

        protected AnimationPerformer Fabricate(SpriteAnimation animation)
        {
            AnimationPerformer performer = Activator.CreateInstance(animation.PerformerType) as AnimationPerformer;
            performer.Animator = _animator;

            _performer.Add(animation.PerformerType, performer);

            return performer;
        }
    }
}
