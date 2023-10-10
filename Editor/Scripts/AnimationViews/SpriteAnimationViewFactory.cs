using System;
using System.Collections.Generic;

namespace SpriteAnimations.Editor
{
    public class SpriteAnimationViewFactory
    {
        private Dictionary<SpriteAnimationType, SpriteAnimationView> _views = new();
        private ContentElement _contentElement;

        public SpriteAnimationViewFactory(ContentElement contentElement)
        {
            _contentElement = contentElement;
        }

        public SpriteAnimationView GetView(SpriteAnimationType animationType)
        {
            if (!_views.TryGetValue(animationType, out SpriteAnimationView view))
            {
                view = Fabricate(animationType);
            }

            return view;
        }

        protected SpriteAnimationView Fabricate(SpriteAnimationType animationType)
        {
            SpriteAnimationView view = animationType switch
            {
                SpriteAnimationType.Simple => new SimpleSpriteAnimationView(_contentElement),
                _ => throw new ArgumentOutOfRangeException(nameof(animationType), animationType, null),
            };

            _views.Add(animationType, view);

            return view;
        }
    }
}
