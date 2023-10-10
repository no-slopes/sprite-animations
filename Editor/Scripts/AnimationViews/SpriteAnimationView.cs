using UnityEngine;
using UnityEditor.PackageManager.UI;
using UnityEngine.UIElements;
using static SpriteAnimations.SpriteAnimation;
using UnityEditor;
using UnityEditor.Playables;

namespace SpriteAnimations.Editor
{
    public abstract class SpriteAnimationView
    {
        #region Fields

        protected ContentElement _contentElement;
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

        public SpriteAnimationView(ContentElement contentElement)
        {
            _contentElement = contentElement;
        }

        #endregion

        #region Initializing

        public virtual void Initialize(SpriteAnimation animation)
        {
            _animation = animation;

            _contentElement.FPSSlider.value = _animation.FPS;
            _contentElement.AnimationNameField.value = _animation.AnimationName;

            _contentElement.AnimationNameField.RegisterValueChangedCallback(OnAnimationNameChanged);
            _contentElement.FPSSlider.RegisterValueChangedCallback(OnFPSChange);
        }

        public virtual void Dismiss()
        {
            if (_animation != null)
            {
                EditorUtility.SetDirty(_animation);
                AssetDatabase.SaveAssetIfDirty(_animation);
            }

            _animation = null;
            _contentElement.AnimationNameField.UnregisterValueChangedCallback(OnAnimationNameChanged);
            _contentElement.FPSSlider.UnregisterValueChangedCallback(OnFPSChange);
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
    }
}