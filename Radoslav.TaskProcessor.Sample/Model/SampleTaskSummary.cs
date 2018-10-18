using System;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.Sample
{
    /// <remarks>This class must be serializable to binary, XML, JSON and/or other formats you may use.</remarks>
    [Serializable]
    public sealed class SampleTaskSummary : ITaskSummary
    {
        public string Description { get; set; }

        public int TaskId { get; set; }

        public int TenantId { get; set; }
    }
}