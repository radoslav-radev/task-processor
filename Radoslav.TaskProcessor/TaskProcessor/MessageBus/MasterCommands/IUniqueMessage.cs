namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// Basic implementation of a message that can be uniquely identified.
    /// </summary>
    public interface IUniqueMessage
    {
        /// <summary>
        /// Gets an unique value by which the message can be identified.
        /// </summary>
        /// <remarks> Value must be unique per message type.</remarks>
        /// <value>Unique value by which the message can be identified.</value>
        string MessageUniqueId { get; }
    }
}