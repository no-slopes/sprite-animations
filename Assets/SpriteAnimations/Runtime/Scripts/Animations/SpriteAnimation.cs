using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SpriteAnimations
{
    public delegate void NameChangedEvent(string newName);
    public abstract class SpriteAnimation : ScriptableObject
    {
        #region Menus

#if UNITY_EDITOR
        [MenuItem("Assets/Sprite Animations/Manage Animation", priority = 5)]
        private static void ManageAnimation()
        {
            SpriteAnimation selectedAnimation = Selection.activeObject as SpriteAnimation;
            if (!selectedAnimation)
            {
                Logger.LogError($"Selected object is not an {nameof(SpriteAnimation)}");
                return;
            }

            Debug.Log($"Selected {nameof(SpriteAnimation)}: {selectedAnimation.name}");
        }
        [MenuItem("Assets/Sprite Animations/Manage Animation", true)]
        static bool CheckIfMainMethodIsValid()
        {
            return Selection.activeObject is SpriteAnimation;
        }
#endif

        #endregion

        #region Inspector

        [SerializeField]
        private SpriteAnimationManageButton _manageButton;

        /// <summary>
        /// The animation name to be used by the SpriteAnimator
        /// </summary>
        [SerializeField]
        protected string _animationName;

        /// <summary>
        /// Amount of frames per second.
        /// </summary>
        [SerializeField]
        protected int _fps = 6;

        #endregion

        #region Properties

        /// <summary>
        /// The amount of frames per second.
        /// </summary>
        public int FPS { get => _fps; set => _fps = value; }

        /// <summary>
        /// The name of the animation. Can be used to identify the animation
        /// in the SpriteAnimator.
        /// </summary>
        public string AnimationName
        {
            get => _animationName;
            set
            {
                _animationName = value;
                NameChanged?.Invoke(_animationName);
            }
        }

        #endregion

        #region Abstratctions

        public abstract Type PerformerType { get; }
        public abstract AnimationType AnimationType { get; }

        #endregion

        #region Calculations

        /// <summary>
        /// Calculates the duration of the animation in seconds.
        /// Depending on the type of animation this can have serious perfomance impacts
        /// as it has to evaluate all frames of all the animation cycles.
        /// </summary>
        /// <returns>The duration of the animation in seconds.</returns>
        public float CalculateDuration()
        {
            // Calculate the number of frames in the animation
            int frameCount = CalculateFramesCount();

            // If there are no frames, return 0 duration
            if (frameCount <= 0) return 0;

            // Calculate and return the duration in seconds
            return frameCount / (float)_fps;
        }

        /// <summary>
        /// Calculates the total amount of frames in the animation.
        /// Depending on the type of animation this can have serious perfomance impacts
        /// as it has to evaluate all frames of all the animation cycles.
        /// </summary>
        /// <returns>The count of frames.</returns>
        public abstract int CalculateFramesCount();

        #endregion

        #region Events

        /// <summary>
        /// Listen to this event to be notified when the animation name changes
        /// </summary>
        public event NameChangedEvent NameChanged;

        #endregion

    }

    [System.Serializable]
    public class SpriteAnimationManageButton
    {

    }
}