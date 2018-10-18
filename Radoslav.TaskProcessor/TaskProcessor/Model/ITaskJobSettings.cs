namespace Radoslav.TaskProcessor.Model
{
    /// <summary>
    /// Basic functionality of task job settings.
    /// </summary>
    /// <remarks>
    /// <para>These settings are passed to the task worker every time a task of certain type is executed.</para>
    /// <para>The settings must be kept in storage between different software releases, keep this in mind when
    /// changing the implementing class for a new release. Serialization and deserialization must continue to work.</para>
    /// </remarks>
    public interface ITaskJobSettings
    {
    }
}