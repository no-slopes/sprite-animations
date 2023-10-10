using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor.UIElements;

namespace SpriteAnimations.Editor
{
    // public delegate void AnimationSelectedEvent(SpriteAnimation animation);

    public delegate void TickEvent(float deltaTime);
    public delegate void DestroyAnimationRequestedEvent();

    public class ContentElement : VisualElement
    {
        #region Fields

        private SpriteAnimation _currentAnimation;

        private TemplateContainer _template;

        private Button _deleteAnimationButton;

        private VisualElement _animationViewElement;

        private ObjectField _animationField;
        private TextField _animationNameField;
        private SliderInt _fpsSlider;

        private SpriteAnimationView _currentView;
        private SpriteAnimationViewFactory _viewsFactory;

        #endregion        

        #region Getters

        public SpriteAnimation CurrentAnimation => _currentAnimation;

        public SliderInt FPSSlider => _fpsSlider;
        public TextField AnimationNameField => _animationNameField;

        public VisualElement AnimationViewElement => _animationViewElement;

        #endregion

        #region Constructors

        public ContentElement()
        {
            VisualTreeAsset tree = Resources.Load<VisualTreeAsset>("UI Documents/Content");
            _template = tree.Instantiate();

            _viewsFactory = new SpriteAnimationViewFactory(this);

            _deleteAnimationButton = _template.Q<Button>("delete-animation-button");
            _deleteAnimationButton.clicked += () => DestroyAnimationRequested?.Invoke();

            _animationViewElement = _template.Q<VisualElement>("content-body");
            _animationField = _template.Q<ObjectField>("animation-field");
            _animationField.SetEnabled(false);
            _animationNameField = _template.Q<TextField>("animation-name-field");
            _fpsSlider = _template.Q<SliderInt>("fps-slider");

            Add(_template);
        }

        #endregion

        #region Initialization

        public void Initialize(SpriteAnimation spriteAnimation)
        {
            _currentAnimation = spriteAnimation;

            _animationField.value = _currentAnimation;

            _currentView = _viewsFactory.GetView(_currentAnimation.AnimationType);
            _currentView.Initialize(_currentAnimation);

            _animationViewElement.Add(_currentView.Root);
        }

        public void Dismiss()
        {
            _currentView?.Dismiss();
            _animationViewElement.Clear();
            _currentAnimation = null;
        }

        #endregion

        #region Animation

        public event DestroyAnimationRequestedEvent DestroyAnimationRequested;

        #endregion      

        #region Tick

        public event TickEvent Tick;

        public virtual void PerformTick(float deltaTime)
        {
            Tick?.Invoke(deltaTime);
        }

        #endregion
    }
}