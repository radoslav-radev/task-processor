namespace Radoslav.TaskProcessor.Model
{
    /// <summary>
    /// An enumeration defining why a task processor has became a master or a slave.
    /// </summary>
    public enum MasterModeChangeReason
    {
        /// <summary>
        /// No reason.
        /// </summary>
        None,

        /// <summary>
        /// The task processor has started and became the master because there was not current master.
        /// </summary>
        Start,

        /// <summary>
        /// The task processor changed mode during heartbeat.
        /// </summary>
        Heartbeat,

        /// <summary>
        /// The task processor was explicitly set to a master or a slave.
        /// </summary>
        Explicit,

        /// <summary>
        /// The task processor was stopped.
        /// </summary>
        Stop
    }
}