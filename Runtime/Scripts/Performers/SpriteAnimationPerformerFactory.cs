using System;
using System.Collections.Generic;

namespace SpriteAnimations.Performers
{
    public class SpriteAnimationPerformerFactory
    {
        private Dictionary<Type, SpriteAnimationPerformer> _handlers = new();

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
            SpriteAnimationPerformer handler = Activator.CreateInstance(animation.PerformerType) as SpriteAnimationPerformer;

            _handlers.Add(animation.PerformerType, handler);

            return handler;
        }
    }
}
