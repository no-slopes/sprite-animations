using System;
using System.Collections.Generic;
using static SpriteAnimations.SpriteAnimation;

namespace SpriteAnimations.Editor
{
    public class SpriteAnimationViewFactory
    {
        private Dictionary<SpriteAnimationType, SpriteAnimationView> _views = new();
        private SpriteAnimatorEditorWindow _spriteAnimatorEditorWindow;

        public SpriteAnimationViewFactory(SpriteAnimatorEditorWindow editorWindow)
        {
            _spriteAnimatorEditorWindow = editorWindow;
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
                SpriteAnimationType.Single => new SingleSpriteAnimationView(_spriteAnimatorEditorWindow),
                _ => throw new ArgumentOutOfRangeException(nameof(animationType), animationType, null),
            };

            _views.Add(animationType, view);

            return view;
        }
    }
}
