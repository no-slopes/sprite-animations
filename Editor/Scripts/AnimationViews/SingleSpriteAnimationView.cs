using SpriteAnimations.Performers;
using UnityEngine;
using UnityEngine.UIElements;
using static SpriteAnimations.SpriteAnimation;

namespace SpriteAnimations.Editor
{
    public class SingleSpriteAnimationView : SpriteAnimationView
    {
        #region Fields

        private VisualElement _root;
        private CycleElement _cycleElement;
        private SingleSpriteAnimation _singleSpriteAnimation;

        #endregion

        #region Getters

        public override SpriteAnimationType AnimationType => SpriteAnimationType.Single;
        public override VisualElement Root
        {
            get
            {
                if (_root == null)
                {
                    _root = GenerateRootElement();
                }

                return _root;
            }
        }

        #endregion

        #region Constructors

        public SingleSpriteAnimationView(SpriteAnimatorEditorWindow window) : base(window)
        {
            _cycleElement = new CycleElement(this);
        }

        #endregion

        #region Initializing

        public override void Initialize(SpriteAnimation animation)
        {
            base.Initialize(animation);
            _singleSpriteAnimation = animation as SingleSpriteAnimation;
            _cycleElement.Initialize(_singleSpriteAnimation.Cycle);
        }

        public override void Dismiss()
        {
            base.Dismiss();
            _singleSpriteAnimation = null;
            _cycleElement.Dismiss();
        }

        #endregion

        #region UI

        private VisualElement GenerateRootElement()
        {
            VisualElement root = new();
            root.Add(_cycleElement);
            return root;
        }

        #endregion
    }
}