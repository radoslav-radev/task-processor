namespace Radoslav.TaskProcessor.MessageBus.Redis
{
    /// <summary>
    /// Class with the names of the Redis channels used for communication between the task processor instances.
    /// </summary>
    internal static class RedisTaskProcessorChannels
    {
        internal const string TaskStartedChannel = "Radoslav$TaskProcessor$TaskStarted";
        internal const string TaskAssignedChannel = "Radoslav$TaskProcessor$AssignTask";
        internal const string TaskCanceledChannel = "Radoslav$TaskProcessor$CancelTask";

        internal const string MasterModeChangeRequestChannel = "Radoslav$TaskProcessor$ChangeMasterMode";
        internal const string MasterModeChangedChannel = "Radoslav$TaskProcessor$MasterModeChanged";
        internal const string StopTaskProcessorChannel = "Radoslav$TaskProcessor$Stop";
        internal const string PerformanceMonitoringChannel = "Radoslav$TaskProcessor$PerformanceMonitoring";
        internal const string ConfigurationChangedChannel = "Radoslav$TaskProcessor$ConfigurationChanged";
    }
}