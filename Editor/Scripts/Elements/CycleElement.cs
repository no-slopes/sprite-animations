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

        private VisualElement _previewColumnContainer;
        private VisualElement _framesListColumnContainer;

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

            _contentElement = contentElement;

            _previewColumnContainer = new()
            {
                style = {
                    flexDirection = FlexDirection.Column,
                    flexGrow = 1,
                    alignContent = Align.FlexStart,
                    maxWidth = 210,
                    paddingTop = 5,
                    paddingBottom = 5,
                    paddingLeft = 5,
                    paddingRight = 5,
                },
            };

            _animationPreviewElement = GenerateAnimationPreviewElement(_contentElement);

            _previewColumnContainer.Add(_animationPreviewElement);
            _previewColumnContainer.AddToClassList("animation-preview");

            Add(_previewColumnContainer);

            _framesListColumnContainer = new()
            {
                style = {
                    flexDirection = FlexDirection.Column,
                    flexGrow = 1,
                    alignContent = Align.FlexStart,
                    paddingTop = 5,
                    paddingBottom = 5,
                    paddingLeft = 5,
                    paddingRight = 5,
                    minWidth = 200,
                }
            };

            _framesListElement = new FramesListElement();
            _framesListColumnContainer.Add(_framesListElement);

            Add(_framesListColumnContainer);
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