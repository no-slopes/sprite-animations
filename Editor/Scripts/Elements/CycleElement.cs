using System;
using SpriteAnimations.Performers;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpriteAnimations.Editor
{
    public class CycleElement : VisualElement
    {
        #region Fields

        private SpriteAnimationCycle _cycle;
        private ContentElement _contentElement;

        private AnimationPreviewElement _animationPreviewElement;
        private FramesListElement _framesListElement;

        #endregion

        #region Properties

        public SpriteAnimationCycle Cycle
        {
            get => _cycle;
            set
            {
                _cycle = value;
            }
        }

        public ContentElement View => _contentElement;

        #endregion

        #region Constructors

        public CycleElement(ContentElement contentElement)
        {
            style.flexDirection = FlexDirection.Row;
            style.flexGrow = 1;

            _contentElement = contentElement;

            VisualTreeAsset tree = Resources.Load<VisualTreeAsset>("UI Documents/Cycle");
            TemplateContainer template = tree.Instantiate();
            template.style.flexGrow = 1;

            VisualElement previewContainer = template.Q<VisualElement>("animation-preview-container");
            previewContainer.AddToClassList("animation-preview");
            _animationPreviewElement = GenerateAnimationPreviewElement(_contentElement);
            previewContainer.Add(_animationPreviewElement);

            VisualElement framesContainer = template.Q<VisualElement>("frames-container");
            _framesListElement = new FramesListElement();
            framesContainer.Add(_framesListElement);

            Add(template);
        }

        #endregion

        #region Animation Preview Element

        private AnimationPreviewElement GenerateAnimationPreviewElement(ContentElement contentElement)
        {
            return new(contentElement);
        }

        #endregion

        #region Tick

        #endregion

        #region Flow

        public void Initialize(SpriteAnimationCycle cycle)
        {
            _cycle = cycle;
            _animationPreviewElement.Frames = cycle.Frames;
            _framesListElement.Initialize(cycle.Frames);
        }

        public void Dismiss()
        {
            _cycle = null;
            _framesListElement.Dismiss();
            // Super important to prevent memory leaks.
            _animationPreviewElement.Stop();
            _animationPreviewElement.Dismiss();
        }

        #endregion
    }
}