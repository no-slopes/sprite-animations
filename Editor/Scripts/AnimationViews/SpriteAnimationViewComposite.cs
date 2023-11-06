using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpriteAnimations.Editor
{
    public class SpriteAnimationViewComposite : SpriteAnimationView
    {
        #region Fields

        private Toggle _loopableCoreField;
        private CycleElement _cycleElement;
        private SpriteAnimationComposite _compositeSpriteAnimation;
        private AnimationPreviewElement _animationPreview;
        private Cycle _currentCycle;
        private List<Button> _tabButtons;

        #endregion

        #region Getters

        public override AnimationType AnimationType => AnimationType.Composite;

        #endregion

        #region Constructors

        public SpriteAnimationViewComposite()
        {
            VisualTreeAsset tree = Resources.Load<VisualTreeAsset>("UI Documents/Animations Views/AnimationViewComposite");
            TemplateContainer template = tree.CloneTree();
            template.style.flexGrow = 1;

            _loopableCoreField = template.Q<Toggle>("loopable-core-field");
            _loopableCoreField.RegisterValueChangedCallback(OnLoopableValueChanges);

            VisualElement animationPreviewContainer = template.Q<VisualElement>("animation-preview-container");
            animationPreviewContainer.Clear();
            _animationPreview = new AnimationPreviewElement();
            animationPreviewContainer.Add(_animationPreview);

            VisualElement cycleContainer = template.Q<VisualElement>("cycle-view");

            _tabButtons = template.Query<Button>(className: "tab-button").ToList();

            _tabButtons.ForEach(tabButton =>
            {
                tabButton.clicked += () => OnCycleButtonClicked(tabButton);
            });

            _cycleElement = new CycleElement();
            cycleContainer.Add(_cycleElement);

            _contentContainer.Add(template);
        }

        #endregion

        #region Initializing

        public override void Initialize(SpriteAnimation animation)
        {
            base.Initialize(animation);
            _compositeSpriteAnimation = animation as SpriteAnimationComposite;
            _loopableCoreField.value = _compositeSpriteAnimation.IsLoopableCore;
            InitializeCycle(_compositeSpriteAnimation.AntecipationCycle); // Must be initialized before the preview

            foreach (Button button in _tabButtons)
            {
                if (button.viewDataKey != "antecipation") return;
                button.AddToClassList("active");
                break;
            }

            _animationPreview.Initialize(this, this, _cycleElement, _viewZoomSlider);
        }

        public override void Dismiss()
        {
            base.Dismiss();
            _compositeSpriteAnimation = null;
            _cycleElement?.Dismiss();
        }

        #endregion

        #region Cycles

        private void InitializeCycle(Cycle cycle)
        {
            _cycleElement?.Dismiss();
            _currentCycle = cycle;
            _cycleElement.Initialize(cycle, this);
        }

        private void EvaluateCycle(string cycleName)
        {
            switch (cycleName)
            {
                case "antecipation":
                    InitializeCycle(_compositeSpriteAnimation.AntecipationCycle);
                    break;
                case "core":
                    InitializeCycle(_compositeSpriteAnimation.CoreCycle);
                    break;
                case "recovery":
                    InitializeCycle(_compositeSpriteAnimation.RecoveryCycle);
                    break;
                default:
                    InitializeCycle(_compositeSpriteAnimation.AntecipationCycle);
                    break;
            }
        }

        private void OnCycleButtonClicked(Button button)
        {
            _tabButtons.ForEach(tabButton => tabButton.RemoveFromClassList("active"));
            button.AddToClassList("active");
            EvaluateCycle(button.viewDataKey);
        }

        #endregion

        #region Loopable

        private void OnLoopableValueChanges(ChangeEvent<bool> changeEvent)
        {
            _compositeSpriteAnimation.IsLoopableCore = changeEvent.newValue;
        }

        #endregion
    }
}