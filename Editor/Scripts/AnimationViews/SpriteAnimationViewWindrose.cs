using SpriteAnimations.Performers;
using UnityEngine;
using UnityEngine.UIElements;
using static SpriteAnimations.SpriteAnimation;
using static SpriteAnimations.SpriteAnimationWindrose;

namespace SpriteAnimations.Editor
{
    public class SpriteAnimationViewWindrose : SpriteAnimationView
    {
        #region Fields

        private TemplateContainer _template;

        private Toggle _loopableField;
        private CycleElement _cycleElement;
        private SpriteAnimationWindrose _spriteAnimationWindrose;
        private WindroseSelectorElement _windroseSelector;

        #endregion

        #region Getters

        public override SpriteAnimationType AnimationType => SpriteAnimationType.Simple;

        #endregion

        #region Constructors

        public SpriteAnimationViewWindrose(ContentElement contentElement) : base(contentElement)
        {
            style.flexGrow = 1;

            VisualTreeAsset tree = Resources.Load<VisualTreeAsset>("UI Documents/AnimationViewWindrose");
            _template = tree.Instantiate();
            _template.style.flexGrow = 1;

            VisualElement windroseSelectorContainer = _template.Q<VisualElement>("windrose-selector-container");
            _windroseSelector = new WindroseSelectorElement();
            _windroseSelector.DirectionSelected += OnWindroseDirectionSelected;
            windroseSelectorContainer.Add(_windroseSelector);

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
            _spriteAnimationWindrose = animation as SpriteAnimationWindrose;
            _loopableField.value = _spriteAnimationWindrose.IsLoopable;
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
            _cycleElement.Dismiss();
            _cycleElement.Initialize(_spriteAnimationWindrose.FindOrCreateCycle(windroseDirection));
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