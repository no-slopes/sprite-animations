using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpriteAnimations.Editor
{
    public class SingleManagerWindow : EditorWindow
    {
        #region Static

        public static SingleManagerWindow OpenEditorWindow(SpriteAnimation animation)
        {
            var window = GetWindow<SingleManagerWindow>();
            window.titleContent = new GUIContent("Single Manager");
            window.minSize = new Vector2(1024, 650);
            window.SetAnimationAndInitialize(animation);
            window.Show();
            return window;
        }

        #endregion

        #region Fields

        private float _timeTracker;

        // UI Elements
        private SpriteAnimation _animation;

        private VisualElement _container;
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

            VisualTreeAsset visualTree = Resources.Load<VisualTreeAsset>("UI Documents/SingleManagerUI");
            TemplateContainer templateContainer = visualTree.CloneTree();
            templateContainer.style.flexGrow = 1;

            _container = templateContainer.Q<VisualElement>("container");

            if (_animation != null && _view == null)
            {
                InitializeView(_animation);
            }

            rootVisualElement.Add(templateContainer);
        }

        private void OnDisable()
        {
            _view?.Dismiss();
        }

        private void Update()
        {
            if (_animation == null || _view == null) return;

            float deltaTime = (float)EditorApplication.timeSinceStartup - _timeTracker;
            _timeTracker += deltaTime;
            _view.PerformTick(deltaTime);
        }

        #endregion

        #region Animation

        public void SetAnimationAndInitialize(SpriteAnimation animation)
        {
            _animation = animation;
            InitializeView(_animation);
        }

        private void InitializeView(SpriteAnimation spriteAnimation)
        {
            if (spriteAnimation == null) return;
            _view = _viewsFactory.GetView(spriteAnimation.AnimationType);
            _container.Add(_view);
            _view.Initialize(spriteAnimation);
        }

        #endregion
    }
}