using UnityEngine;
using UnityEditor.PackageManager.UI;
using UnityEngine.UIElements;
using static SpriteAnimations.SpriteAnimation;
using UnityEditor;
using UnityEditor.Playables;
using UnityEditor.UIElements;

namespace SpriteAnimations.Editor
{
    public delegate void FPSChangedEvent(int fps);
    public delegate void TickEvent(float deltaTime);
    public delegate void DestroyAnimationRequestedEvent();
    public abstract class SpriteAnimationView : VisualElement, ITickProvider, IFPSProvider
    {
        #region Fields

        protected SpriteAnimation _animation;

        protected VisualElement _contentContainer;
        protected Button _deleteAnimationButton;

        protected ObjectField _animationField;
        protected TextField _animationNameField;
        protected SliderInt _fpsSlider;
        protected Slider _viewZoomSlider;


        #endregion

        #region Properties

        #endregion

        #region Getters

        public int FPS => _animation.FPS;
        public string AnimationName => _animationNameField.value;

        public SpriteAnimation Animation => _animation;
        public abstract AnimationType AnimationType { get; }
        public Slider ViewZoomSlider => _viewZoomSlider;

        #endregion

        #region Constructors

        public SpriteAnimationView()
        {
            style.flexGrow = 1;
            VisualTreeAsset tree = Resources.Load<VisualTreeAsset>("UI Documents/Animations Views/AnimationView");
            TemplateContainer template = tree.Instantiate();
            template.style.flexGrow = 1;

            _viewZoomSlider = template.Q<Slider>("view-zoom-slider");

            _deleteAnimationButton = template.Q<Button>("delete-animation-button");
            _deleteAnimationButton.clicked += () => DestroyAnimationRequested?.Invoke();

            _contentContainer = template.Q<VisualElement>("content-container");
            _contentContainer.style.flexGrow = 1;

            _animationField = template.Q<ObjectField>("animation-field");
            _animationField.SetEnabled(false);
            _animationNameField = template.Q<TextField>("animation-name-field");
            _fpsSlider = template.Q<SliderInt>("fps-slider");

            Add(template);
        }

        #endregion

        #region Initializing

        public virtual void Initialize(SpriteAnimation animation)
        {
            _animation = animation;

            _animationField.value = _animation;
            _fpsSlider.value = _animation.FPS;
            _animationNameField.value = _animation.AnimationName;

            _animationNameField.RegisterValueChangedCallback(OnAnimationNameChanged);
            _fpsSlider.RegisterValueChangedCallback(OnFPSChange);
        }

        public virtual void Dismiss()
        {
            if (_animation != null)
            {
                EditorUtility.SetDirty(_animation);
                AssetDatabase.SaveAssetIfDirty(_animation);
            }

            _animation = null;
            _animationNameField.UnregisterValueChangedCallback(OnAnimationNameChanged);
            _fpsSlider.UnregisterValueChangedCallback(OnFPSChange);
        }

        #endregion

        #region FPS

        public event FPSChangedEvent FPSChanged;

        private void OnFPSChange(ChangeEvent<int> changeEvent)
        {
            _animation.FPS = changeEvent.newValue;
            FPSChanged?.Invoke(_animation.FPS);
        }

        private void OnAnimationNameChanged(ChangeEvent<string> changeEvent)
        {
            _animation.AnimationName = changeEvent.newValue;
        }

        #endregion

        #region Tick

        public event TickEvent Tick;

        public virtual void PerformTick(float deltaTime)
        {
            Tick?.Invoke(deltaTime);
        }

        #endregion        

        #region Animation

        public event DestroyAnimationRequestedEvent DestroyAnimationRequested;

        #endregion
    }
}