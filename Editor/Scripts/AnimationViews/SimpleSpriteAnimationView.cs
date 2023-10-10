using SpriteAnimations.Performers;
using UnityEngine;
using UnityEngine.UIElements;
using static SpriteAnimations.SpriteAnimation;

namespace SpriteAnimations.Editor
{
    public class SimpleSpriteAnimationView : SpriteAnimationView
    {
        #region Fields

        private TemplateContainer _template;

        private Toggle _loopableField;
        private CycleElement _cycleElement;
        private SimpleSpriteAnimation _simpleSpriteAnimation;

        #endregion

        #region Getters

        public override SpriteAnimationType AnimationType => SpriteAnimationType.Simple;

        #endregion

        #region Constructors

        public SimpleSpriteAnimationView(ContentElement contentElement) : base(contentElement)
        {

            VisualTreeAsset tree = Resources.Load<VisualTreeAsset>("UI Documents/AnimationViewSimple");
            _template = tree.Instantiate();

            _loopableField = _template.Q<Toggle>("loopable-field");
            _loopableField.RegisterValueChangedCallback(OnLoopableValueChanges);

            VisualElement cycleContainer = _template.Q<VisualElement>("cycle-container");
            _cycleElement = new CycleElement(contentElement);
            cycleContainer.Add(_cycleElement);

            Add(_template);
        }

        #endregion

        #region Initializing

        public override void Initialize(SpriteAnimation animation)
        {
            base.Initialize(animation);
            _simpleSpriteAnimation = animation as SimpleSpriteAnimation;
            _loopableField.value = _simpleSpriteAnimation.IsLoopable;
            _cycleElement.Initialize(_simpleSpriteAnimation.Cycle);
        }

        public override void Dismiss()
        {
            base.Dismiss();
            _simpleSpriteAnimation = null;
            _cycleElement.Dismiss();
        }

        #endregion

        #region Loopable

        private void OnLoopableValueChanges(ChangeEvent<bool> changeEvent)
        {
            _simpleSpriteAnimation.IsLoopable = changeEvent.newValue;
        }

        #endregion
    }
}