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
            window.SetAnimatorAndLoad(spriteAnimator);
            return window;
        }

        #endregion

        #region Fields

        private float _timeTracker;

        // UI Elements
        private VisualElement _noAnimatorContainer;
        private VisualElement _selectedAnimatorContainer;

        private ObjectField _animatorSelectorField;
        private Button _fromSelectionButton;

        private SidebarElement _sidebarElement;

        private SpriteAnimation _selectedAnimation;

        private VisualElement _viewContainer;
        private SpriteAnimationView _view;
        private SpriteAnimationViewFactory _viewsFactory;

        #endregion

        #region Getters

        public AnimationsManagerWindowData Data => AnimationsManagerWindowData.instance;

        #endregion

        #region GUI

        private void OnEnable()
        {
            _timeTracker = (float)EditorApplication.timeSinceStartup;
            _viewsFactory = new SpriteAnimationViewFactory();

            VisualTreeAsset visualTree = Resources.Load<VisualTreeAsset>("UI Documents/AnimationsManagerUI");
            TemplateContainer templateContainer = visualTree.CloneTree();
            rootVisualElement.Add(templateContainer);
            templateContainer.style.flexGrow = 1;

            LoadVisualElements(rootVisualElement); // Can only load animations

            _animatorSelectorField.RegisterValueChangedCallback((e) =>
            {
                SpriteAnimator spriteAnimator = e.newValue as SpriteAnimator;
                SetAnimatorAndLoad(spriteAnimator);
            });

            _fromSelectionButton.clicked += () =>
            {
                GameObject selectedObject = Selection.activeObject as GameObject;
                if (selectedObject == null) return;
                SpriteAnimator spriteAnimator = selectedObject.GetComponent<SpriteAnimator>();
                if (spriteAnimator == null) return;
                SetAnimatorAndLoad(spriteAnimator);
            };

            LoadAnimatorFromData();

            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable()
        {
            _view?.Dismiss();
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void Update()
        {
            if (_selectedAnimation == null || _view == null) return;

            float deltaTime = (float)EditorApplication.timeSinceStartup - _timeTracker;
            _timeTracker += deltaTime;
            _view.PerformTick(deltaTime);
        }

        public void EvaluateDisplay()
        {
            if (Data.Animator == null)
            {
                DisplayNoAnimatorSelectedContainer();
            }
            else
            {
                DisplaySelectedAnimatorContainer();
            }
        }

        private void LoadAnimatorFromData()
        {
            if (Data.Animator != null)
            {
                _animatorSelectorField.SetValueWithoutNotify(Data.Animator);
                LoadAnimations(Data.Animator);
                EvaluateDisplay();
                return;
            }

            if (Data.AnimatorObj == null)
            {
                EvaluateDisplay();
                return;
            }

            if (!Data.AnimatorObj.TryGetComponent(out SpriteAnimator animator))
            {
                EvaluateDisplay();
                return;
            }

            SetAnimatorAndLoad(animator);
        }

        public void SetAnimator(SpriteAnimator animator)
        {
            Data.SetAnimator(animator);
            _animatorSelectorField?.SetValueWithoutNotify(animator);
        }

        public void SetAnimatorAndLoad(SpriteAnimator animator)
        {
            SetAnimator(animator);
            LoadAnimations(animator);
            EvaluateDisplay();
        }

        private void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            if (change != PlayModeStateChange.EnteredEditMode) return;
            LoadAnimatorFromData();
        }

        private void LoadVisualElements(VisualElement root)
        {
            _noAnimatorContainer = root.Q<VisualElement>("no-animator-container");
            _selectedAnimatorContainer = root.Q<VisualElement>("selected-animator-container");

            VisualElement sidebar = root.Q<VisualElement>("sidebar");
            _sidebarElement = new SidebarElement();
            _sidebarElement.AnimationSelected += OnAnimationSelected;
            sidebar.Add(_sidebarElement);

            _viewContainer = root.Q<VisualElement>("animation-view-container");

            _animatorSelectorField = root.Q<ObjectField>("animator-selector-field");
            _fromSelectionButton = root.Q<Button>("from-selection-button");
        }

        private void DisplayNoAnimatorSelectedContainer()
        {
            _selectedAnimatorContainer.style.display = DisplayStyle.None;
            _noAnimatorContainer.style.display = DisplayStyle.Flex;

            _viewContainer.style.display = DisplayStyle.None;

            DissmissCurrentView();
        }

        private void DisplaySelectedAnimatorContainer()
        {
            _noAnimatorContainer.style.display = DisplayStyle.None;
            _selectedAnimatorContainer.style.display = DisplayStyle.Flex;
            _viewContainer.style.display = DisplayStyle.Flex;
        }

        private void LoadAnimations(SpriteAnimator spriteAnimator)
        {
            if (spriteAnimator == null) return;
            spriteAnimator.AnimationsList.RemoveAll(animation => animation == null);
            _sidebarElement.Initialize(spriteAnimator.AnimationsList);
        }

        #endregion

        #region Animation

        private void OnAnimationSelected(SpriteAnimation animation)
        {
            DissmissCurrentView();

            _selectedAnimation = animation;

            InitializeView(_selectedAnimation);
        }

        private void DissmissCurrentView()
        {
            _viewContainer?.Clear();
            _view?.Dismiss();

            if (_view != null)
                _view.DestroyAnimationRequested -= DestroySelectedAnimation;
        }

        private void InitializeView(SpriteAnimation spriteAnimation)
        {
            _view = _viewsFactory.GetView(_selectedAnimation.AnimationType);
            _viewContainer.Add(_view);
            _view.Initialize(_selectedAnimation);
            _view.DestroyAnimationRequested += DestroySelectedAnimation;
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

            Data.Animator.AnimationsList.Remove(_selectedAnimation);

            string path = AssetDatabase.GetAssetPath(_selectedAnimation);

            if (!AssetDatabase.DeleteAsset(path))
            {
                Logger.LogWarning("Could not delete animation.");
                return;
            }

            AssetDatabase.SaveAssets();
            DissmissCurrentView();
            _sidebarElement.Reload();
        }

        #endregion
    }
}