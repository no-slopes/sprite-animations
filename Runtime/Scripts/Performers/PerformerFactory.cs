using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpriteAnimations.Performers
{
    public class PerformerFactory
    {
        private SpriteAnimator _animator;
        private Dictionary<Type, Performer> _handlers = new();

        public PerformerFactory(SpriteAnimator animator)
        {
            _animator = animator;
        }

        public Performer GetPerformer(SpriteAnimation animation)
        {
            if (!_handlers.TryGetValue(animation.PerformerType, out Performer handler))
            {
                handler = Fabricate(animation);
            }

            return handler;
        }

        protected Performer Fabricate(SpriteAnimation animation)
        {
            Performer performer = Activator.CreateInstance(animation.PerformerType) as Performer;
            performer.Animator = _animator;

            _handlers.Add(animation.PerformerType, performer);

            return performer;
        }
    }
}
