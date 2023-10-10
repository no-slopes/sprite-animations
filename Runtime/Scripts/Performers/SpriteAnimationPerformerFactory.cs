using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpriteAnimations.Performers
{
    public class SpriteAnimationPerformerFactory
    {
        private SpriteRenderer _spriteRenderer;
        private Dictionary<Type, SpriteAnimationPerformer> _handlers = new();

        public SpriteAnimationPerformerFactory(SpriteRenderer spriteRenderer)
        {
            _spriteRenderer = spriteRenderer;
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
            performer.SpriteRenderer = _spriteRenderer;

            _handlers.Add(animation.PerformerType, performer);

            return performer;
        }
    }
}
