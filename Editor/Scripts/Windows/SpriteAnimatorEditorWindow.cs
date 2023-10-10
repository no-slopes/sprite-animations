using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpriteAnimations.Editor
{
    public class AnimationsManagerWindow : EditorWindow
    {
        #region Static

        [MenuItem("Tools/Sprite Animations/Animations Manager")]
        public static AnimationsManagerWindow OpenEditorWindow()
        {
            var window = GetWindow<AnimationsManagerWindow>();
            window.titleContent = new GUIContent("Animations Manager");
            window.minSize = new Vector2(1024, 650);
            window.Show();

            return window;
        }

        public static AnimationsManagerWindow OpenEditorWindow(SpriteAnimator spriteAnimator)
        {
            var window = OpenEditorWindow();
            window.AnimatorSelectorField.value = spriteAnimator;
            return window;
        }

        #endregion

        #region Fields

        private float _timeTracker;

        // UI Elements
        private VisualElement _noAnimatorContainer;
        private VisualElement _selectedAnimatorContainer;
        private VisualElement _contentContainer;

        private ObjectField _animatorSelectorField;
        private Button _fromSelectionButton;

        private SidebarElement _sidebarElement;
        private ContentElement _contentElement;

        private SpriteAnimator _spriteAnimator;
        private SpriteAnimation _selectedAnimation;

        #endregion

        #region Getters

        public ObjectField AnimatorSelectorField
        {
            get
            {
                if (_animatorSelectorField == null)
                {
                    _animatorSelectorField = rootVisualElement.Q<ObjectField>("animator-selector");
                }

                return _animatorSelectorField;
            }
        }

        #endregion

        #region GUI

        private void OnEnable()
        {
            _timeTracker = (float)EditorApplication.timeSinceStartup;

            VisualTreeAsset visualTree = Resources.Load<VisualTreeAsset>("UI Documents/Main");
            TemplateContainer templateContainer = visualTree.Instantiate();
            rootVisualElement.Add(templateContainer);
            templateContainer.StretchToParentSize();

            LoadVisualElements(rootVisualElement);

            AnimatorSelectorField.RegisterValueChangedCallback((e) =>
            {
                SpriteAnimator spriteAnimator = e.newValue as SpriteAnimator;
                EvaluateSpriteAnimator(spriteAnimator);
            });

            _fromSelectionButton.clicked += () =>
            {
                GameObject selectedObject = Selection.activeObject as GameObject;
                if (selectedObject == null) return;
                SpriteAnimator spriteAnimator = selectedObject.GetComponent<SpriteAnimator>();
                if (spriteAnimator == null) return;
                AnimatorSelectorField.value = spriteAnimator;
            };

            if (AnimatorSelectorField.value == null)
            {
                EvaluateSpriteAnimator(null);
                return;
            }

            EvaluateSpriteAnimator(AnimatorSelectorField.value as SpriteAnimator);
        }

        private void OnDisable()
        {
            _contentElement?.Dismiss();
        }

        private void Update()
        {
            if (_selectedAnimation == null || _contentElement == null) return;

            float deltaTime = (float)EditorApplication.timeSinceStartup - _timeTracker;
            _timeTracker += deltaTime;
            _contentElement.PerformTick(deltaTime);
        }

        public void EvaluateSpriteAnimator(SpriteAnimator spriteAnimator)
        {
            if (spriteAnimator == null)
            {
                _spriteAnimator = null;
                DisplayNoAnimatorSelectedContainer();
                return;
            }

            _spriteAnimator = spriteAnimator;
            LoadSpriteAnimator(_spriteAnimator);
            DisplaySelectedAnimatorContainer();
        }

        private void LoadVisualElements(VisualElement root)
        {
            _noAnimatorContainer = root.Q<VisualElement>("no-animator-container");
            _selectedAnimatorContainer = root.Q<VisualElement>("selected-animator-container");

            VisualElement sidebar = root.Q<VisualElement>("sidebar");
            _sidebarElement = new SidebarElement();
            _sidebarElement.AnimationSelected += OnAnimationSelected;
            sidebar.Add(_sidebarElement);

            _contentContainer = root.Q<VisualElement>("content");
            _contentElement = new ContentElement();
            _contentElement.DestroyAnimationRequested += DestroySelectedAnimation;
            _contentContainer.Add(_contentElement);

            _animatorSelectorField = root.Q<ObjectField>("animator-selector-field");
            _fromSelectionButton = root.Q<Button>("from-selection-button");
        }

        private void DisplayNoAnimatorSelectedContainer()
        {
            _selectedAnimatorContainer.style.display = DisplayStyle.None;
            _noAnimatorContainer.style.display = DisplayStyle.Flex;

            _contentContainer.style.display = DisplayStyle.None;

            _contentElement.Dismiss();
        }

        private void DisplaySelectedAnimatorContainer()
        {
            _noAnimatorContainer.style.display = DisplayStyle.None;
            _selectedAnimatorContainer.style.display = DisplayStyle.Flex;
        }

        private void DisplayContent()
        {
            _contentContainer.style.display = DisplayStyle.Flex;
        }

        private void LoadSpriteAnimator(SpriteAnimator spriteAnimator)
        {
            spriteAnimator.Animations.RemoveAll(animation => animation == null);
            _sidebarElement.Initialize(spriteAnimator.Animations);
        }

        #endregion

        #region Animation

        private void OnAnimationSelected(SpriteAnimation animation)
        {
            _contentElement.Dismiss();

            _selectedAnimation = animation;

            _contentElement.Initialize(animation);
            _contentContainer.style.display = DisplayStyle.Flex;
        }

        public void DestroySelectedAnimation()
        {
            if (_selectedAnimation == null) return;

            bool confirmed = EditorUtility.DisplayDialog(
               $"Attention", // title
               $"This will destroy the animation asset irreversibly. Proceed?",
               "Yes", // OK button
               "No" // Cancel button
            );

            if (!confirmed) return;

            _spriteAnimator.Animations.Remove(_selectedAnimation);

            string path = AssetDatabase.GetAssetPath(_selectedAnimation);

            if (!AssetDatabase.DeleteAsset(path))
            {
                Debug.LogWarning("Could not delete animation.");
                return;
            }

            AssetDatabase.SaveAssets();

            _sidebarElement.Reload();
        }

        #endregion
    }
}