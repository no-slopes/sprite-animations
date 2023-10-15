using System;
using System.Collections.Generic;

namespace SpriteAnimations.Editor
{
    public class SpriteAnimationViewFactory
    {
        private Dictionary<SpriteAnimationType, SpriteAnimationView> _views = new();

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
                SpriteAnimationType.Simple => new SpriteAnimationViewSimple(),
                SpriteAnimationType.Windrose => new SpriteAnimationViewWindrose(),
                _ => throw new ArgumentOutOfRangeException(nameof(animationType), animationType, null),
            };

            _views.Add(animationType, view);

            return view;
        }
    }
}
