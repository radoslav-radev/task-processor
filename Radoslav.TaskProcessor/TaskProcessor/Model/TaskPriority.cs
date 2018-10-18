namespace Radoslav.TaskProcessor.Model
{
    /// <summary>
    /// An enumeration for task priority.
    /// </summary>
    /// <remarks>Values must be ordered from highest to lowest because they are used for ordering tasks according to their priority.</remarks>
    public enum TaskPriority
    {
        /// <summary>
        /// Very high priority.
        /// </summary>
        VeryHigh = 2,

        /// <summary>
        /// High priority.
        /// </summary>
        High = 1,

        /// <summary>
        /// Normal (default) priority.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// Low priority.
        /// </summary>
        Low = -1
    }
}