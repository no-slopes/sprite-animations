using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpriteAnimations.Editor
{
    public class SpriteAnimatorEditorWindow : EditorWindow
    {
        #region Static

        [MenuItem("Tools/Handy Tools/Sprite Animator")]
        public static SpriteAnimatorEditorWindow OpenEditorWindow()
        {
            var window = GetWindow<SpriteAnimatorEditorWindow>();
            window.titleContent = new GUIContent("Sprite Animator");
            window.minSize = new Vector2(1024, 650);
            window.Show();

            return window;
        }

        public static SpriteAnimatorEditorWindow OpenEditorWindow(SpriteAnimator spriteAnimator)
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
        private ListView _animationsListView;
        private Button _createAnimationButton;
        private Button _deleteAnimationButton;
        private CreateAnimationOptionsMenu _createAnimationOptionsMenu;

        private ObjectField _animatorSelectorField;
        private Button _fromSelectionButton;
        private VisualElement _contentContainer;
        private VisualElement _animationViewElement;

        private ObjectField _animationField;
        private TextField _animationNameField;
        private SliderInt _fpsSlider;

        private SpriteAnimator _spriteAnimator;
        private SpriteAnimation _selectedAnimation;

        private SpriteAnimationView _currentView;

        private SpriteAnimationViewFactory _viewsFactory;

        #endregion

        #region Getters

        public SliderInt FPSSlider => _fpsSlider;
        public TextField AnimationNameField => _animationNameField;
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

        public VisualElement AnimationViewElement => _animationViewElement;

        #endregion

        #region GUI

        private void OnEnable()
        {
            _viewsFactory = new SpriteAnimationViewFactory(this);
            _timeTracker = (float)EditorApplication.timeSinceStartup;

            VisualTreeAsset visualTree = Resources.Load<VisualTreeAsset>("UI Documents/SpriteAnimatorUIDocument");
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
            _currentView?.Dismiss();
        }

        private void Update()
        {
            if (_currentView == null) return;

            float deltaTime = (float)EditorApplication.timeSinceStartup - _timeTracker;
            _timeTracker += deltaTime;
            _currentView?.PerformTick(deltaTime);
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

            _animationsListView = root.Q<ListView>("animations-list");

            _animationsListView.makeItem = () => new AnimationListItemElement();
            _animationsListView.bindItem = (e, i) =>
            {
                AnimationListItemElement item = e as AnimationListItemElement;
                item.Animation = _spriteAnimator.Animations[i];
            };

            _animationsListView.selectedIndicesChanged += OnListItemsSelected;

            _animationsListView.fixedItemHeight = 50;
            _animationsListView.reorderable = true;

            _createAnimationButton = root.Q<Button>("create-animation-button");
            _createAnimationOptionsMenu = new CreateAnimationOptionsMenu();
            _createAnimationOptionsMenu.AnimationCreated += OnAnimationCreated;
            _createAnimationButton.clicked += () => _createAnimationOptionsMenu.Display(_createAnimationButton);
            root.Add(_createAnimationOptionsMenu);

            _deleteAnimationButton = root.Q<Button>("delete-animation-button");
            _deleteAnimationButton.clicked += () => DestroySelectedAnimation();

            _animatorSelectorField = root.Q<ObjectField>("animator-selector-field");
            _fromSelectionButton = root.Q<Button>("from-selection-button");

            _contentContainer = root.Q<VisualElement>("content");
            _animationViewElement = root.Q<VisualElement>("content-body");
            _animationField = root.Q<ObjectField>("animation-field");
            _animationField.SetEnabled(false);
            _animationNameField = root.Q<TextField>("animation-name-field");
            _fpsSlider = root.Q<SliderInt>("fps-slider");
        }

        private void DisplayNoAnimatorSelectedContainer()
        {
            _createAnimationOptionsMenu?.Hide();

            _selectedAnimatorContainer.style.display = DisplayStyle.None;
            _noAnimatorContainer.style.display = DisplayStyle.Flex;

            _contentContainer.style.display = DisplayStyle.None;
            _animationsListView.Clear();
            _animationsListView.ClearSelection();

            _currentView = null;
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
            _animationsListView.Clear();

            _spriteAnimator.Animations.RemoveAll(animation => animation == null);

            _animationsListView.itemsSource = spriteAnimator.Animations;

            if (spriteAnimator.Animations.Count > 0)
            {
                SelectAnimation(spriteAnimator.Animations[0]);
                _animationsListView.SetSelection(0);
            }
        }

        private void OnListItemsSelected(IEnumerable<int> enumerable)
        {
            _createAnimationOptionsMenu?.Hide();
            int[] indexes = enumerable.ToArray();

            if (indexes.Length <= 0) return;

            SelectAnimation(_spriteAnimator.Animations[indexes[0]]);
        }

        #endregion

        #region Animation

        private void SelectAnimation(SpriteAnimation animation)
        {
            _currentView?.Dismiss();

            _animationViewElement.Clear();

            _selectedAnimation = animation;
            _animationField.value = _selectedAnimation;
            _currentView = _viewsFactory.GetView(_selectedAnimation.AnimationType);

            _currentView.Initialize(_selectedAnimation);

            _animationViewElement.Add(_currentView.Root);

            _contentContainer.style.display = DisplayStyle.Flex;
        }

        private void OnAnimationCreated(SpriteAnimation animation)
        {
            _spriteAnimator.Animations.Add(animation);
            _animationsListView.Rebuild();
            SelectAnimation(animation);
            _animationsListView.SetSelection(_spriteAnimator.Animations.Count - 1);
        }

        private void DestroySelectedAnimation()
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

            _animationsListView.Rebuild();

            if (_spriteAnimator.Animations.Count > 0)
            {
                SelectAnimation(_spriteAnimator.Animations[0]);
                _animationsListView.SetSelection(0);
            }
        }

        #endregion
    }
}