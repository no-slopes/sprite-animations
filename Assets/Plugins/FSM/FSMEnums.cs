namespace HandyFSM
{
    /// <summary>
    /// Enum representing the mode of changing state.
    /// </summary>
    public enum StateChangeMode
    {
        /// <summary>
        /// Change state respectfully.
        /// </summary>
        Respectfully,

        /// <summary>
        /// Change state forcefully.
        /// </summary>
        Forcefully
    }

    /// <summary>
    /// The machine's Statuses.
    /// </summary>
    public enum MachineStatus
    {
        /// <summary>
        /// The machine is turned on.
        /// </summary>
        On,

        /// <summary>
        /// The machine is paused.
        /// </summary>
        Paused,

        /// <summary>
        /// The machine is turned off.
        /// </summary>
        Off,
    }
}