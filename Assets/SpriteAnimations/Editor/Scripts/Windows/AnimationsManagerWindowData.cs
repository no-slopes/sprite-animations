using UnityEditor;
using UnityEngine;

namespace SpriteAnimations.Editor
{
    [FilePath("AnimationsManagerWindowData.asset", FilePathAttribute.Location.PreferencesFolder)]
    public class AnimationsManagerWindowData : ScriptableSingleton<AnimationsManagerWindowData>
    {
        #region Fields

        [SerializeField]
        private SpriteAnimator _animator;

        [SerializeField]
        private GameObject _animatorObj;

        #endregion

        #region Getters

        public SpriteAnimator Animator => _animator;
        public GameObject AnimatorObj => _animatorObj;

        #endregion

        #region Methods

        public void SetAnimator(SpriteAnimator animator)
        {
            _animator = animator;

            if (_animator != null)
            {
                _animatorObj = _animator.gameObject;
            }
            else
            {
                _animatorObj = null;
            }
        }

        #endregion
    }
}