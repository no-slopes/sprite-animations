using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;
using SpriteAnimations.Performers;

#if UNITY_EDITOR
using SpriteAnimations.Editor;
#endif

namespace SpriteAnimations
{
    /// <summary>
    /// This compoment represents the animator for a sprite renderer.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [AddComponentMenu("Sprite Animator")]
    public class SpriteAnimator : MonoBehaviour
    {
        #region Components

        #endregion

#if UNITY_EDITOR
        public void OpenAnimatorWindow()
        {
            SpriteAnimatorEditorWindow.OpenEditorWindow(this);
        }
#endif

        [SerializeField]
        protected UpdateMode _updateMode = UpdateMode.Update;

        [SerializeField]
        protected SpriteAnimation _currentAnimation;

        [SerializeField]
        [Space]
        protected List<SpriteAnimation> _spriteAnimations = new();

        [SerializeField]
        [Space]
        protected bool _playAutomatically = true;

        [SerializeField]
        protected UnityEvent<SpriteAnimation> _animationChanged;

        [SerializeField]
        protected UnityEvent<SpriteAnimatorState> _stateChanged;

        #region Fields

        protected SpriteRenderer _spriteRenderer;

        protected SpriteAnimationPerformerFactory _performersFactory;
        protected SpriteAnimationFrame _currentFrame;

        protected SpriteAnimationPerformer _currentPerformer;
        protected Dictionary<SpriteAnimationFrame, UnityEvent> _frameEvents = new();

        #endregion

        #region Getters

        public SpriteAnimation DefaultAnimation => _spriteAnimations.Count > 0 ? _spriteAnimations[0] : null;
        public List<SpriteAnimation> Animations => _spriteAnimations;

        #endregion

        protected SpriteAnimatorState _state = SpriteAnimatorState.Stopped;

        public bool Playing => _state == SpriteAnimatorState.Playing;
        public bool Paused => _state == SpriteAnimatorState.Paused;
        public bool Stopped => _state == SpriteAnimatorState.Stopped;

        #region Getters

        public SpriteAnimatorState State => _state;

        public SpriteAnimation CurrentAnimation => _currentAnimation;
        public SpriteAnimationFrame CurrentFrame => _currentFrame;

        public UnityEvent<SpriteAnimation> AnimationChanged => _animationChanged;
        public UnityEvent<SpriteAnimatorState> StateChanged => _stateChanged;

        #endregion

        #region Behaviour

        protected virtual void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _performersFactory = new SpriteAnimationPerformerFactory();
        }

        protected virtual void Start()
        {
            if (_playAutomatically && DefaultAnimation != null)
                Play(DefaultAnimation);
        }

        protected virtual void Update()
        {
            if (_updateMode.Equals(UpdateMode.Update))
                EvaluateAndChangeCurrentFrame();
        }

        protected virtual void LateUpdate()
        {
            if (_updateMode.Equals(UpdateMode.LateUpdate))
                EvaluateAndChangeCurrentFrame();
        }

        protected virtual void FixedUpdate()
        {
            if (_updateMode.Equals(UpdateMode.FixedUpdate))
                EvaluateAndChangeCurrentFrame();
        }

        #endregion

        #region Controlling Animator

        /// <summary>
        /// Plays the default animation. 
        /// </summary>
        public SpriteAnimationPerformer Play()
        {
            if (_currentAnimation == null)
            {
                return Play(DefaultAnimation);
            }

            return Play(_currentAnimation);
        }

        /// <summary>
        /// Plays the specified animation by its name. Note that the animation must be registered to the 
        /// animator in orther to be found.
        /// </summary>
        /// <param name="name"></param>
        public SpriteAnimationPerformer Play(string animationName)
        {
            return Play(GetAnimationByName(animationName));
        }

        /// <summary>
        /// Plays the given animation. Does not require registering.
        /// </summary>
        /// <param name="animation"></param>
        public SpriteAnimationPerformer Play(SpriteAnimation animation)
        {
            if (animation == _currentAnimation) return _currentPerformer;

            if (animation == null) // If the animation is null, prevent changing.
            {
                Debug.LogError($"Sprite Animator for {gameObject.name} - Could not evaluate an animation to be played. Check animation passed as parameter and the animation frames.");
                return null;
            }

            ChangeAnimation(animation);

            _state = SpriteAnimatorState.Playing;
            return _currentPerformer;

        }

        /// <summary>
        /// Pauses the animator. Use Resume to continue.
        /// </summary>
        public SpriteAnimationPerformer Pause()
        {
            _state = SpriteAnimatorState.Paused;
            _stateChanged.Invoke(_state);
            return _currentPerformer;
        }

        /// <summary>
        /// Resumes a paused animator.
        /// </summary>
        public SpriteAnimationPerformer Resume()
        {
            _state = SpriteAnimatorState.Playing;
            _stateChanged.Invoke(_state);
            return _currentPerformer;
        }

        /// <summary>
        /// Stops animating
        /// </summary>
        public void Stop()
        {
            _state = SpriteAnimatorState.Stopped;
            _stateChanged.Invoke(_state);
            _currentPerformer?.StopAnimation(); // Stop current animation.
            _currentAnimation = null;
            _currentFrame = null;
        }

        /// <summary>
        /// This changes the current animation to be played by the animator. It will call 
        /// the current animation Stop() method and the new animation Start() method.
        /// </summary>
        /// <param name="animation"></param>
        protected void ChangeAnimation(SpriteAnimation animation)
        {
            _currentPerformer?.StopAnimation(); // Stop current animation.

            _currentPerformer = _performersFactory.GetPerformer(_currentAnimation); // Sets the current handler to the given animation.
            _currentAnimation = animation; // current animation is now the given animation.
            _currentPerformer.StartAnimation(_currentAnimation); // Starts the given animation.

            _animationChanged.Invoke(_currentAnimation); // Fires the animation changed event.
        }

        #endregion

        #region Handling Animation

        /// <summary>
        /// Returns an animation wich was registered to the animator based on given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SpriteAnimation GetAnimationByName(string name)
        {
            return _spriteAnimations.FirstOrDefault(a => a.name == name);
        }

        /// <summary>
        /// This should be called every LateUpdate to evaluate the current animation and change the sprite.
        /// </summary>
        protected void EvaluateAndChangeCurrentFrame()
        {
            if (!Playing) return;

            SpriteAnimationFrame frame = _currentPerformer.EvaluateFrame(Time.deltaTime);

            if (!Playing || frame == null || frame == _currentFrame) return;

            _spriteRenderer.sprite = frame.Sprite;

            _currentFrame = frame;
        }

        #endregion

        #region Evaluations

        public float CalculateCurrentAnimationDuration()
        {
            if (_currentAnimation == null) return 0;
            return _currentAnimation.CalculateDuration();
        }

        #endregion

        #region Enums

        public enum SpriteAnimatorState
        {
            Playing = 0,
            Paused = 1,
            Stopped = 2
        }

        public enum UpdateMode
        {
            Update,
            FixedUpdate,
            LateUpdate,
        }

        #endregion
    }
}