using System;
using UnityEngine;
using UnityEngine.UIElements;
using static SpriteAnimations.SpriteAnimation;

namespace SpriteAnimations.Editor
{
    public class AnimationListItemElement : VisualElement
    {
        #region Fields

        private Label _animationNameText;
        private Label _animationTypeText;
        private SpriteAnimation _animation;

        #endregion

        #region Properties

        public SpriteAnimation Animation
        {
            get => _animation;
            set
            {
                _animation = value;
                _animationNameText.text = !string.IsNullOrEmpty(_animation.AnimationName) ? _animation.AnimationName : _animation.name;
                _animationTypeText.text = ResolveAnimationTypeLabel(_animation.AnimationType);
                _animation.NameChanged += (newName) =>
                {
                    _animationNameText.text = !string.IsNullOrEmpty(newName) ? newName : _animation.name;
                };
            }
        }

        #endregion

        #region Constructors

        public AnimationListItemElement()
        {
            VisualTreeAsset tree = Resources.Load<VisualTreeAsset>("UI Documents/AnimationsListItem");
            TemplateContainer template = tree.Instantiate();

            _animationNameText = template.Q<Label>("name-text");
            _animationTypeText = template.Q<Label>("type-text");

            AddToClassList("interactable");
            Add(template);
        }

        #endregion

        #region Animation Type

        private string ResolveAnimationTypeLabel(SpriteAnimationType animationType)
        {
            return $"[{animationType}]";
        }

        #endregion
    }
}