using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpriteAnimations.Editor
{
    public class SpriteAnimationViewCombo : SpriteAnimationView
    {
        #region Fields

        private FloatField _waitingTimeField;
        private CycleElement _cycleElement;
        private SpriteAnimationCombo _comboAnimation;
        private AnimationPreviewElement _animationPreview;
        private ComboCyclesHandler _cyclesListHandler;

        #endregion

        #region Getters

        public override AnimationType AnimationType => AnimationType.Combo;
        public ComboCyclesHandler Cycles => _cyclesListHandler;

        #endregion

        #region Constructors

        public SpriteAnimationViewCombo()
        {
            VisualTreeAsset tree = Resources.Load<VisualTreeAsset>("UI Documents/Animations Views/AnimationViewCombo");
            TemplateContainer template = tree.CloneTree();
            template.style.flexGrow = 1;

            _waitingTimeField = template.Q<FloatField>("waiting-time-field");
            _waitingTimeField.RegisterValueChangedCallback(OnWaitingTimeValueChanges);

            VisualElement animationPreviewContainer = template.Q<VisualElement>("animation-preview-container");
            animationPreviewContainer.Clear();
            _animationPreview = new AnimationPreviewElement();
            animationPreviewContainer.Add(_animationPreview);

            VisualElement cycleContainer = template.Q<VisualElement>("cycle-container");

            _cycleElement = new CycleElement();
            cycleContainer.Add(_cycleElement);

            ListView cyclesList = template.Q<ListView>("cycles-list");
            cyclesList.selectedIndicesChanged += OnCycleSelected;

            Button createCycleButton = template.Q<Button>("create-cycle-button");

            _cyclesListHandler = new ComboCyclesHandler(cyclesList, createCycleButton);

            _contentContainer.Add(template);
        }

        #endregion

        #region Initializing

        public override void Initialize(SpriteAnimation animation)
        {
            base.Initialize(animation);
            _comboAnimation = animation as SpriteAnimationCombo;
            _waitingTimeField.value = _comboAnimation.WaitingTime;
            _cyclesListHandler.Initialize(_comboAnimation);
            LoadCycle(_comboAnimation.FirstCycle);
        }

        public override void Dismiss()
        {
            base.Dismiss();
            _comboAnimation = null;
            _cyclesListHandler.Dismiss();
            _animationPreview.Dismiss();
            _cycleElement.Dismiss();
        }

        #endregion

        #region Form Events

        private void OnWaitingTimeValueChanges(ChangeEvent<float> changeEvent)
        {
            _comboAnimation.WaitingTime = changeEvent.newValue;
        }

        #endregion

        #region Cycle

        private void LoadCycle(Cycle cycle)
        {
            _animationPreview?.Dismiss();
            _cycleElement?.Dismiss();

            _cycleElement.Initialize(cycle, this);
            _animationPreview.Initialize(this, this, _cycleElement, _viewZoomSlider);

        }

        private void OnCycleSelected(IEnumerable<int> selectedIndexes)
        {
            if (selectedIndexes.Count() <= 0) return;
            Cycle selectedCycle = _comboAnimation.Cycles[selectedIndexes.First()];
            LoadCycle(selectedCycle);
        }

        #endregion
    }
}