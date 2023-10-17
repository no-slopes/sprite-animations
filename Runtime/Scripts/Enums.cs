namespace SpriteAnimations
{
    /// <summary>
    /// The state of the animator
    /// </summary>
    public enum AnimatorState
    {
        /// <summary>
        /// The animator is playing the last defined animation
        /// </summary>
        Playing = 0,
        /// <summary>
        /// The animator is paused, meaning it will not change frames of the current
        /// animation untill it is resumed
        /// </summary>
        Paused = 1,
        /// <summary>
        /// The animator is stopped, meaning no animation is currently defined.
        /// </summary>
        Stopped = 2
    }

    /// <summary>
    /// The update mode of the animator
    /// </summary>
    public enum UpdateMode
    {
        /// <summary>
        /// Will evaluate frames every Update
        /// </summary>
        Update,
        /// <summary>
        /// Will evaluate frames every FixedUpdate
        /// </summary>
        FixedUpdate,
        /// <summary>
        /// Will evaluate frames every LateUpdate
        /// </summary>
        LateUpdate,
    }

    /// <summary>
    /// A direction in the windrose (8-direction)
    /// </summary>
    public enum WindroseDirection
    {
        // &lt; equals <
        // &gt; equals >

        /// <summary>
        /// The north direction. Can be represented as a Vector2 having x == 0 and y &gt; 0
        /// </summary>
        North,
        /// <summary>
        /// The north-east direction. Can be represented as a Vector2 having x &gt; 0 and y &gt; 0
        /// </summary>
        NorthEast,
        /// <summary>
        /// The east direction. Can be represented as a Vector2 having x &gt; 0 and y == 0
        /// </summary>
        East,
        /// <summary>
        /// The south-east direction. Can be represented as a Vector2 having x &gt; 0 and y  &lt; 0
        /// </summary>
        SouthEast,
        /// <summary>
        /// The south direction. Can be represented as a Vector2 having x == 0 and y  &lt; 0
        /// </summary>
        South,
        /// <summary>
        /// The south-west direction. Can be represented as a Vector2 having x  &lt; 0 and y  &lt; 0
        /// </summary>
        SouthWest,
        /// <summary>
        /// The west direction. Can be represented as a Vector2 having x  &lt; 0 and y == 0
        /// </summary>
        West,
        /// <summary>
        /// The north-west direction. Can be represented as a Vector2 having x  &lt; 0 and y &gt; 0
        /// </summary>
        NorthWest
    }

    /// <summary>
    /// The flip strategy for a windrose animation
    /// </summary>
    public enum WindroseFlipStrategy
    {
        /// <summary>
        /// The WindroseAnimator will flip East (NorthEast, East and SouthEast)  sprites and play them as West (West, NorthWest and SouthWest) sprites
        /// </summary>
        FlipEastToPlayWest,
        /// <summary>
        /// There will be no flip at all.
        /// </summary>
        NoFlip
    }
}

