using System;
using System.Collections.Generic;

namespace SpriteAnimations.Editor
{
    public class SpriteAnimationViewFactory
    {
        private Dictionary<AnimationType, SpriteAnimationView> _views = new();

        public SpriteAnimationView GetView(AnimationType animationType)
        {
            if (!_views.TryGetValue(animationType, out SpriteAnimationView view))
            {
                view = Fabricate(animationType);
            }

            return view;
        }

        protected SpriteAnimationView Fabricate(AnimationType animationType)
        {
            SpriteAnimationView view = animationType switch
            {
                AnimationType.SingleCycle => new SpriteAnimationViewSimple(),
                AnimationType.Windrose => new SpriteAnimationViewWindrose(),
                _ => throw new ArgumentOutOfRangeException(nameof(animationType), animationType, null),
            };

            _views.Add(animationType, view);

            return view;
        }
    }
}
