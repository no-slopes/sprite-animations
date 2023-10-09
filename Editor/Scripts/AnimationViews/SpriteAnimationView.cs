using UnityEngine;
using UnityEditor.PackageManager.UI;
using UnityEngine.UIElements;
using static SpriteAnimations.SpriteAnimation;
using UnityEditor;
using UnityEditor.Playables;

namespace SpriteAnimations.Editor
{
    public delegate void TickEvent(float deltaTime);
    public abstract class SpriteAnimationView
    {
        #region Fields

        protected SpriteAnimatorEditorWindow _window;
        protected SpriteAnimation _animation;

        #endregion

        #region Properties

        #endregion

        #region Getters

        public SpriteAnimation Animation => _animation;
        public abstract SpriteAnimationType AnimationType { get; }
        public abstract VisualElement Root { get; }

        #endregion

        #region Constructors

        public SpriteAnimationView(SpriteAnimatorEditorWindow window)
        {
            _window = window;
        }

        #endregion

        #region Initializing

        public virtual void Initialize(SpriteAnimation animation)
        {
            _animation = animation;

            _window.FPSSlider.value = _animation.FPS;
            _window.AnimationNameField.value = _animation.AnimationName;

            _window.AnimationNameField.RegisterValueChangedCallback(OnAnimationNameChanged);
            _window.FPSSlider.RegisterValueChangedCallback(OnFPSChange);
        }

        public virtual void Dismiss()
        {
            if (_animation != null)
            {
                EditorUtility.SetDirty(_animation);
                AssetDatabase.SaveAssetIfDirty(_animation);
            }

            _animation = null;
            _window.AnimationNameField.UnregisterValueChangedCallback(OnAnimationNameChanged);
            _window.FPSSlider.UnregisterValueChangedCallback(OnFPSChange);
        }

        #endregion

        #region Tick

        public virtual void PerformTick(float deltaTime)
        {
            Tick?.Invoke(deltaTime);
        }

        #endregion

        #region FPS

        private void OnFPSChange(ChangeEvent<int> changeEvent)
        {
            _animation.FPS = changeEvent.newValue;
        }

        private void OnAnimationNameChanged(ChangeEvent<string> changeEvent)
        {
            _animation.AnimationName = changeEvent.newValue;
        }

        #endregion

        #region Events

        public event TickEvent Tick; // event

        #endregion
    }
}