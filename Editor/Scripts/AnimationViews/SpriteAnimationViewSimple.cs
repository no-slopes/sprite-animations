using UnityEngine;
using UnityEngine.UIElements;

namespace SpriteAnimations.Editor
{
    public class SpriteAnimationViewSimple : SpriteAnimationView
    {
        #region Fields

        private Toggle _loopableField;
        private CycleElement _cycleElement;
        private SpriteAnimationSingleCycle _simpleSpriteAnimation;
        private AnimationPreviewElement _animationPreview;

        #endregion

        #region Getters

        public override AnimationType AnimationType => AnimationType.SingleCycle;

        #endregion

        #region Constructors

        public SpriteAnimationViewSimple()
        {
            VisualTreeAsset tree = Resources.Load<VisualTreeAsset>("UI Documents/AnimationViewSimple");
            TemplateContainer template = tree.Instantiate();
            template.style.flexGrow = 1;

            _loopableField = template.Q<Toggle>("loopable-field");
            _loopableField.RegisterValueChangedCallback(OnLoopableValueChanges);

            VisualElement animationPreviewContainer = template.Q<VisualElement>("animation-preview-container");
            animationPreviewContainer.Clear();
            _animationPreview = new AnimationPreviewElement();
            animationPreviewContainer.Add(_animationPreview);

            VisualElement cycleContainer = template.Q<VisualElement>("content");

            cycleContainer.Add(_viewZoomSlider); // Created at the base class

            _cycleElement = new CycleElement();
            cycleContainer.Add(_cycleElement);

            _contentContainer.Add(template);
        }

        #endregion

        #region Initializing

        public override void Initialize(SpriteAnimation animation)
        {
            base.Initialize(animation);
            _simpleSpriteAnimation = animation as SpriteAnimationSingleCycle;
            _loopableField.value = _simpleSpriteAnimation.IsLoopable;
            _cycleElement.Initialize(_simpleSpriteAnimation.Cycle, this); // Must be initialized before the preview
            _animationPreview.Initialize(this, this, _cycleElement, _viewZoomSlider);
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