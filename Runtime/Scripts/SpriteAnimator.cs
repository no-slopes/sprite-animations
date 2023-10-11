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
            AnimationsManagerWindow.OpenEditorWindow(this);
        }
#endif

        [SerializeField]
        protected SpriteRenderer _spriteRenderer;

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

        protected PerformerFactory _performersFactory;

        protected Performer _currentPerformer;
        protected Dictionary<string, SpriteAnimation> _animations;

        #endregion

        #region Properties

        public SpriteAnimation DefaultAnimation => _spriteAnimations.Count > 0 ? _spriteAnimations[0] : null;
        public List<SpriteAnimation> Animations => _spriteAnimations;

        protected SpriteAnimatorState _state = SpriteAnimatorState.Stopped;

        public bool Playing => _state == SpriteAnimatorState.Playing;
        public bool Paused => _state == SpriteAnimatorState.Paused;
        public bool Stopped => _state == SpriteAnimatorState.Stopped;

        #endregion

        #region Getters

        public SpriteAnimatorState State => _state;
        public SpriteRenderer SpriteRenderer => _spriteRenderer;

        public SpriteAnimation CurrentAnimation => _currentAnimation;

        public UnityEvent<SpriteAnimation> AnimationChanged => _animationChanged;
        public UnityEvent<SpriteAnimatorState> StateChanged => _stateChanged;

        #endregion

        #region Behaviour

        protected virtual void Awake()
        {
            if (_spriteRenderer == null)
            {
                if (!TryGetComponent(out _spriteRenderer))
                {
                    Logger.LogError($"Sprite Renderer not found.", this);
                    return;
                }
            }

            _performersFactory = new PerformerFactory(this);
            _animations = new Dictionary<string, SpriteAnimation>();
            _spriteAnimations.ForEach(a => _animations.Add(a.AnimationName, a));
        }

        protected virtual void Start()
        {
            if (_playAutomatically && DefaultAnimation != null)
                Play(DefaultAnimation);
        }

        protected virtual void Update()
        {
            if (!Playing || !_updateMode.Equals(UpdateMode.Update)) return;
            _currentPerformer?.Tick(Time.deltaTime);
        }

        protected virtual void LateUpdate()
        {
            if (!Playing || !_updateMode.Equals(UpdateMode.LateUpdate)) return;
            _currentPerformer?.Tick(Time.deltaTime);
        }

        protected virtual void FixedUpdate()
        {
            if (!Playing || !_updateMode.Equals(UpdateMode.FixedUpdate)) return;
            _currentPerformer?.Tick(Time.deltaTime);
        }

        #endregion

        #region Controlling Animator

        /// <summary>
        /// Plays the default animation. 
        /// </summary>
        public Performer Play()
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
        public Performer Play(string name)
        {
            if (!TryeGetAnimationByName(name, out var animation))
            {
                Logger.LogError($"Animation '{name}' found.", this);
                return null;
            }
            return Play(animation);
        }

        /// <summary>
        /// Plays the given animation. Does not require registering.
        /// </summary>
        /// <param name="animation"></param>
        public Performer Play(SpriteAnimation animation)
        {
            if (animation == _currentAnimation) return _currentPerformer;

            if (animation == null) // If the animation is null, prevent changing.
            {
                Logger.LogError($"Could not evaluate an animation to be played."
                + $"Check animation passed as parameter and the animation frames.", this);
                return null;
            }

            ChangeAnimation(animation);

            _state = SpriteAnimatorState.Playing;
            return _currentPerformer;
        }

        /// <summary>
        /// Pauses the animator. Use Resume to continue.
        /// </summary>
        public Performer Pause()
        {
            _state = SpriteAnimatorState.Paused;
            _stateChanged.Invoke(_state);
            return _currentPerformer;
        }

        /// <summary>
        /// Resumes a paused animator.
        /// </summary>
        public Performer Resume()
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
            _currentPerformer = null;
        }

        /// <summary>
        /// This changes the current animation to be played by the animator. It will call 
        /// the current animation Stop() method and the new animation Start() method.
        /// </summary>
        /// <param name="animation"></param>
        protected void ChangeAnimation(SpriteAnimation animation)
        {
            _currentPerformer?.StopAnimation(); // Stop current animation.

            _currentPerformer = _performersFactory.GetPerformer(animation); // Sets the current handler to the given animation.
            _currentAnimation = animation; // current animation is now the given animation.
            _currentPerformer.StartAnimation(_currentAnimation); // Starts the given animation.

            _animationChanged.Invoke(_currentAnimation); // Fires the animation changed event.
        }

        #endregion

        #region Handling Animation

        public SpriteAnimation GetAnimationByName(string name)
        {
            if (!TryeGetAnimationByName(name, out var animation))
            {
                return null;
            }

            return animation;
        }

        public bool TryeGetAnimationByName(string name, out SpriteAnimation animation)
        {
            return _animations.TryGetValue(name, out animation);
        }

        #endregion

        #region Evaluations

        /// <summary>
        /// Calculates the duration of the current animation.
        /// Depending on the type of animation this can have serious perfomance impacts
        /// as it has to evaluate all frames of all the animation cycles.
        /// </summary>
        /// <returns>The duration of the current animation.</returns>
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