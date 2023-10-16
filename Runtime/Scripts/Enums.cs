namespace SpriteAnimations
{
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
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
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

