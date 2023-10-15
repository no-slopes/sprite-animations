using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

#if UNITY_EDITOR
using SpriteAnimations.Editor;
#endif

namespace SpriteAnimations
{
    /// <summary>
    /// The Sprite Animator. Responsible for playing animations by changing the sprites
    /// of a SpriteRenderer based on the fps of the playing animation.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [AddComponentMenu("Sprite Animator")]
    public class SpriteAnimator : MonoBehaviour
    {
        #region Components

        #endregion

        [Tooltip("The sprite renderer used for the animation.")]
        [SerializeField]
        protected SpriteRenderer _spriteRenderer;

        [Tooltip("The update mode of the animator.")]
        [SerializeField]
        protected UpdateMode _updateMode = UpdateMode.Update;

        [Tooltip("If the animator should start playing on start.")]
        [SerializeField]
        protected bool _playOnStart = true;

        [Tooltip("The animation that will be played when this animator behaviour starts.")]
        [SerializeField]
        protected SpriteAnimation _defaultAnimation;

        [Tooltip("The animations that can be played by this Sprite Animator. Use the Animations Manager to edit this.")]
        [SerializeField]
        protected List<SpriteAnimation> _spriteAnimations = new();

        [Tooltip("The event that will be invoked when the animation changes.")]
        [SerializeField]
        protected UnityEvent<SpriteAnimation> _animationChanged;

        [Tooltip("The event that will be invoked when the animator state changes.")]
        [SerializeField]
        protected UnityEvent<SpriteAnimatorState> _stateChanged;

        #region Fields

        protected PerformerFactory _performersFactory;
        protected SpriteAnimation _currentAnimation;

        protected AnimationPerformer _currentPerformer;
        protected Dictionary<string, SpriteAnimation> _animations;

        #endregion

        #region Properties

        /// <summary>
        /// The default animation that will be played when this animator behaviour starts.
        /// </summary>
        public SpriteAnimation DefaultAnimation => _defaultAnimation;

        /// <summary>
        /// The list of animations registered to this animator. 
        /// 
        /// Important: Changing this at runtime will have no effect on the capacity of the animator
        /// to play changed list of animations.
        /// </summary>
        public List<SpriteAnimation> Animations => _spriteAnimations;

        protected SpriteAnimatorState _state = SpriteAnimatorState.Stopped;

        /// <summary>
        /// Is the animator playing?
        /// </summary>
        public bool IsPlaying => _state == SpriteAnimatorState.Playing;

        /// <summary>
        /// Is The animator paused?
        /// </summary>
        public bool IsPaused => _state == SpriteAnimatorState.Paused;

        /// <summary>
        /// Is The animation stopped?
        /// </summary>
        public bool IsStopped => _state == SpriteAnimatorState.Stopped;

        #endregion

        #region Getters

        /// <summary>
        /// The current state of the animator.
        /// </summary>
        public SpriteAnimatorState State => _state;

        /// <summary>
        /// The sprite renderer used by the animator.
        /// </summary>
        public SpriteRenderer SpriteRenderer => _spriteRenderer;

        /// <summary>
        /// The current animation being played by the animator.
        /// </summary>
        public SpriteAnimation CurrentAnimation => _currentAnimation;

        /// <summary>
        /// The event that will be invoked when the animation changes.
        /// </summary>
        public UnityEvent<SpriteAnimation> AnimationChanged => _animationChanged;

        /// <summary>
        /// The event that will be invoked when the animator state changes.
        /// </summary>
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
            if (!_playOnStart) return;

            if (_defaultAnimation != null)
            {
                Play(DefaultAnimation);
                return;
            }

            if (_spriteAnimations.Count > 0)
            {
                Play(_spriteAnimations[0]);
                return;
            }
        }

        protected virtual void Update()
        {
            if (!IsPlaying || !_updateMode.Equals(UpdateMode.Update)) return;
            _currentPerformer?.Tick(Time.deltaTime);
        }

        protected virtual void LateUpdate()
        {
            if (!IsPlaying || !_updateMode.Equals(UpdateMode.LateUpdate)) return;
            _currentPerformer?.Tick(Time.deltaTime);
        }

        protected virtual void FixedUpdate()
        {
            if (!IsPlaying || !_updateMode.Equals(UpdateMode.FixedUpdate)) return;
            _currentPerformer?.Tick(Time.deltaTime);
        }

        #endregion

        #region Controlling Animator

        /// <summary>
        /// Plays the default animation. Or the first animation registered to the animator if 
        /// there is no default animation registered.
        /// </summary>
        public AnimationPerformer Play()
        {
            if (_currentAnimation == null)
            {
                return Play(DefaultAnimation);
            }

            if (_spriteAnimations.Count > 0)
            {
                return Play(_spriteAnimations[0]);
            }

            return Play(_currentAnimation);
        }

        /// <summary>
        /// Plays the specified animation by its name. Note that the animation must be registered to the 
        /// animator in order to be found.
        /// </summary>
        /// <typeparam name="TAnimator">The type of the animator.</typeparam>
        /// <param name="name">The name of the animation to play.</param>
        /// <returns>The AnimationPerformer instance for the played animation, or null if the animation is not found.</returns>
        public TAnimator Play<TAnimator>(string name) where TAnimator : AnimationPerformer
        {
            // Try to get the animation by name
            if (!TryGetAnimationByName(name, out var animation))
            {
                // Log an error if the animation is not found
                Logger.LogError($"Animation '{name}' not found.", this);
                return null;
            }

            // Play the animation
            return Play<TAnimator>(animation);
        }

        /// <summary>
        /// Plays the specified animation by its name. Note that the animation must be registered to the 
        /// animator in order to be found.
        /// </summary>
        /// <param name="name">The name of the animation.</param>
        /// <returns>The AnimationPerformer instance for the played animation, or null if the animation is not found.</returns>
        public AnimationPerformer Play(string name)
        {
            // Try to get the animation by its name
            if (!TryGetAnimationByName(name, out var animation))
            {
                // Log an error if the animation is not found
                Logger.LogError($"Animation '{name}' not found.", this);
                return null;
            }

            // Play the animation
            return Play(animation);
        }

        /// <summary>
        /// Plays the specified animation.
        /// </summary>
        /// <typeparam name="TAnimator">The type of the animation performer.</typeparam>
        /// <param name="animation">The animation to play.</param>
        /// <returns>The animation performer.</returns>
        public TAnimator Play<TAnimator>(SpriteAnimation animation) where TAnimator : AnimationPerformer
        {
            // If the animation is already playing, return the existing animation performer.
            if (animation == _currentAnimation)
                return _performersFactory.Get<TAnimator>(animation);

            // If the animation is null, return null and log an error.
            if (animation == null)
            {
                Logger.LogError("Could not evaluate an animation to be played. Null given", this);
                return null;
            }

            // Change the current animation.
            ChangeAnimation(animation);

            // Set the sprite animator state to playing.
            _state = SpriteAnimatorState.Playing;

            // Return the animation performer.
            return _performersFactory.Get<TAnimator>(animation);
        }

        /// <summary>
        /// Plays the given sprite animation.
        /// </summary>
        /// <param name="animation">The animation to play.</param>
        /// <returns>The animation performer.</returns>
        public AnimationPerformer Play(SpriteAnimation animation)
        {
            // If the animation is already playing, return the existing animation performer
            if (animation == _currentAnimation)
                return _performersFactory.Get(animation);
            // If the animation is null, log an error and return null
            if (animation == null)
            {
                Logger.LogError("Could not evaluate an animation to be played. Check animation passed as parameter and the animation frames.", this);
                return null;
            }
            // Change the current animation
            ChangeAnimation(animation);
            // Set the state to playing
            _state = SpriteAnimatorState.Playing;
            // Return the animation performer for the given animation
            return _performersFactory.Get(animation);
        }

        /// <summary>
        /// Pauses the animator. Use Resume to continue.
        /// </summary>
        public AnimationPerformer Pause()
        {
            _state = SpriteAnimatorState.Paused;
            _stateChanged.Invoke(_state);
            return _currentPerformer;
        }

        /// <summary>
        /// Resumes a paused animator.
        /// </summary>
        public AnimationPerformer Resume()
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

            _currentPerformer = _performersFactory.Get(animation); // Sets the current handler to the given animation.
            _currentAnimation = animation; // current animation is now the given animation.
            _currentPerformer.StartAnimation(_currentAnimation); // Starts the given animation.

            _animationChanged.Invoke(_currentAnimation); // Fires the animation changed event.
        }

        #endregion

        #region Handling Animation

        /// <summary>
        /// Gets a registered animation by its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns> The animation or null if not found </returns>
        public SpriteAnimation GetAnimationByName(string name)
        {
            if (!TryGetAnimationByName(name, out var animation))
            {
                return null;
            }

            return animation;
        }

        /// <summary>
        /// Tries to get a registered animation by its name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="animation"></param>
        /// <returns> True if the animation is found, false otherwise </returns>
        public bool TryGetAnimationByName(string name, out SpriteAnimation animation)
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

        #endregion
    }
}