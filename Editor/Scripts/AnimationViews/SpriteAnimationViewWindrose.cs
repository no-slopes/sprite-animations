using UnityEngine;
using UnityEngine.UIElements;

namespace SpriteAnimations.Editor
{
    public class SpriteAnimationViewWindrose : SpriteAnimationView
    {
        #region Fields

        private Toggle _loopableField;
        private CycleElement _cycleElement;
        private SpriteAnimationWindrose _spriteAnimationWindrose;
        private WindroseSelectorElement _windroseSelector;
        private AnimationPreviewElement _animationPreview;

        #endregion

        #region Getters

        public override SpriteAnimationType AnimationType => SpriteAnimationType.Simple;

        #endregion

        #region Constructors

        public SpriteAnimationViewWindrose()
        {
            VisualTreeAsset tree = Resources.Load<VisualTreeAsset>("UI Documents/AnimationViewWindrose");
            TemplateContainer template = tree.Instantiate();
            template.style.flexGrow = 1;

            _loopableField = template.Q<Toggle>("loopable-field");
            _loopableField.RegisterValueChangedCallback(OnLoopableValueChanges);

            VisualElement windroseSelectorContainer = template.Q<VisualElement>("windrose-selector-container");
            windroseSelectorContainer.Clear();
            _windroseSelector = new WindroseSelectorElement();
            _windroseSelector.DirectionSelected += OnWindroseDirectionSelected;
            windroseSelectorContainer.Add(_windroseSelector);

            VisualElement animationPreviewContainer = template.Q<VisualElement>("animation-preview-container");
            animationPreviewContainer.Clear();
            _animationPreview = new AnimationPreviewElement();
            animationPreviewContainer.Add(_animationPreview);

            VisualElement cycleContainer = template.Q<VisualElement>("cycle-container");
            _cycleElement = new CycleElement();
            cycleContainer.Add(_cycleElement);

            _contentContainer.Add(template);
        }

        #endregion

        #region Initializing

        public override void Initialize(SpriteAnimation animation)
        {
            base.Initialize(animation);
            _spriteAnimationWindrose = animation as SpriteAnimationWindrose;
            _loopableField.value = _spriteAnimationWindrose.IsLoopable;
            _windroseSelector.Initialize();
        }

        public override void Dismiss()
        {
            base.Dismiss();
            _spriteAnimationWindrose = null;
            _cycleElement.Dismiss();
        }

        #endregion

        #region Windrose

        private void OnWindroseDirectionSelected(WindroseDirection windroseDirection)
        {
            SpriteAnimationCycle cycle = _spriteAnimationWindrose.FindOrCreateCycle(windroseDirection);

            _cycleElement.Dismiss();
            _cycleElement.Initialize(cycle);

            _animationPreview.Dismiss();
            _animationPreview.Initialize(this, this, _cycleElement);
        }

        #endregion

        #region Loopable

        private void OnLoopableValueChanges(ChangeEvent<bool> changeEvent)
        {
            _spriteAnimationWindrose.IsLoopable = changeEvent.newValue;
        }

        #endregion
    }
}