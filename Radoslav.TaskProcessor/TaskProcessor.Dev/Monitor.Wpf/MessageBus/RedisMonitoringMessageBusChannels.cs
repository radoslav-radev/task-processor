namespace Radoslav.TaskProcessor.MessageBus.Redis
{
    internal static class RedisMonitoringMessageBusChannels
    {
        internal const string TaskRequestedChannel = "Radoslav$TaskProcessor$TaskRequested";
        internal const string TaskProgressChannel = "Radoslav$TaskProcessor$TaskProgress";
        internal const string TaskCancelCompletedChannel = "Radoslav$TaskProcessor$TaskCancelCompleted";
        internal const string TaskFailedChannel = "Radoslav$TaskProcessor$TaskFailed";
        internal const string TaskCompletedChannel = "Radoslav$TaskProcessor$TaskCompleted";

        internal const string TaskProcessorStateChannel = "Radoslav$TaskProcessor$State";
        internal const string PerformanceReportChannel = "Radoslav$TaskProcessor$PerformanceReport";
    }
}