using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpriteAnimations.Editor
{
    public class CreateAnimationOptionsMenu : VisualElement
    {
        #region Fields

        #endregion

        #region Constructors

        public CreateAnimationOptionsMenu()
        {
        }

        #endregion

        #region Visibility

        // public void Display(VisualElement caller)
        // {
        //     Vector2 position = caller.localBound.min;
        //     _root.style.left = position.x;
        //     _root.style.top = position.y + caller.layout.height / 2;
        //     _root.style.visibility = Visibility.Visible;

        //     _nameField.value = "New Animation";
        //     _typeField.value = SpriteAnimationType.Simple;

        //     _nameField.Focus();
        // }

        // public void Hide()
        // {
        //     _root.style.visibility = Visibility.Hidden;
        // }

        #endregion
    }
}