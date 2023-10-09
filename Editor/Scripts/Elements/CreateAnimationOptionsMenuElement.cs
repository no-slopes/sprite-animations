using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpriteAnimations.Editor
{
    public delegate void AnimationCreatedEvent(SpriteAnimation animation);
    public class CreateAnimationOptionsMenu : VisualElement
    {
        #region Fields

        private VisualElement _root;
        private TextField _nameField;
        private EnumField _typeField;
        private Button _createButton;

        #endregion

        #region Constructors

        public CreateAnimationOptionsMenu()
        {
            VisualTreeAsset tree = Resources.Load<VisualTreeAsset>("UI Documents/CreateAnimationOptionsMenu");
            TemplateContainer template = tree.Instantiate();
            _root = template.Q<VisualElement>("container");

            template.style.width = 300;
            template.style.visibility = Visibility.Hidden;
            template.style.position = Position.Absolute;

            Button closeButton = template.Q<Button>("close-button");
            closeButton.clicked += Hide;

            _nameField = template.Q<TextField>("name-field");
            _typeField = template.Q<EnumField>("type-field");
            _createButton = template.Q<Button>("create-button");
            _createButton.clicked += OnCreateClicked;

            Add(template);
        }

        #endregion

        #region Visibility

        public void Display(VisualElement caller)
        {
            Vector2 position = caller.worldBound.min;
            _root.style.left = position.x;
            _root.style.top = position.y + caller.layout.height / 2;
            _root.style.visibility = Visibility.Visible;

            _nameField.value = "New Animation";
            _typeField.value = SpriteAnimationType.Single;

            _nameField.Focus();
        }

        public void Hide()
        {
            _root.style.visibility = Visibility.Hidden;
        }

        #endregion

        #region Creating

        private void OnCreateClicked()
        {
            string name = _nameField.value;
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogWarning("Animation name cannot be empty");
                return;
            }

            string path = EditorUtility.OpenFolderPanel("Select Folder", "Assets", "");

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            string[] parts = path.Split("/Assets");
            path = "Assets" + parts[^1];

            SpriteAnimation animation = _typeField.value switch
            {
                SpriteAnimationType.Single => CreateSingleSpriteAnimation(path, name),
                _ => throw new ArgumentOutOfRangeException(nameof(_typeField.value), null, null)
            };

            animation.AnimationName = name;
            AnimationCreated?.Invoke(animation);
            Hide();
        }

        private SingleSpriteAnimation CreateSingleSpriteAnimation(string path, string name)
        {
            SingleSpriteAnimation sriteAnimationAsset = ScriptableObject.CreateInstance<SingleSpriteAnimation>();
            AssetDatabase.CreateAsset(sriteAnimationAsset, $"{path}/{name}.asset");
            AssetDatabase.SaveAssets();
            return sriteAnimationAsset;
        }

        #endregion

        #region Events

        public event AnimationCreatedEvent AnimationCreated;

        #endregion
    }
}