using SpriteAnimations.Performers;
using UnityEngine;
using UnityEngine.UIElements;
using static SpriteAnimations.SpriteAnimation;

namespace SpriteAnimations.Editor
{
    public class SimpleSpriteAnimationView : SpriteAnimationView
    {
        #region Fields

        private VisualElement _root;
        private CycleElement _cycleElement;
        private SimpleSpriteAnimation _simpleSpriteAnimation;

        #endregion

        #region Getters

        public override SpriteAnimationType AnimationType => SpriteAnimationType.Simple;
        public override VisualElement Root
        {
            get
            {
                _root ??= GenerateRootElement();

                return _root;
            }
        }

        #endregion

        #region Constructors

        public SimpleSpriteAnimationView(ContentElement contentElement) : base(contentElement)
        {
            _cycleElement = new CycleElement(contentElement);
        }

        #endregion

        #region Initializing

        public override void Initialize(SpriteAnimation animation)
        {
            base.Initialize(animation);
            _simpleSpriteAnimation = animation as SimpleSpriteAnimation;
            _cycleElement.Initialize(_simpleSpriteAnimation.Cycle);
        }

        public override void Dismiss()
        {
            base.Dismiss();
            _simpleSpriteAnimation = null;
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