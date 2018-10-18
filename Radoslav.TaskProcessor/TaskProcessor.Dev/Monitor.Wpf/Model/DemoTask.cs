using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Radoslav.TaskProcessor.Model
{
    [Serializable]
    public class DemoTask : ITask
    {
        #region Constructors

        public DemoTask()
        {
        }

        public DemoTask(int totalDurationInSeconds)
        {
            Random randomizer = new Random();

            List<int> durations = new List<int>();

            while (totalDurationInSeconds > 0)
            {
                int nextDurationInSeconds = randomizer.Next(1, 3);

                durations.Add(nextDurationInSeconds);

                totalDurationInSeconds -= nextDurationInSeconds;
            }

            this.Durations = durations
                .Select(d => new DemoTaskDuration() { DurationInSeconds = d })
                .ToArray();
        }

        #endregion Constructors

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = SuppressMessages.Serialization)]
        public DemoTaskDuration[] Durations { get; set; }

        public bool ThrowError { get; set; }

        internal ITaskSummary CreateTaskSummary(SummaryType summaryType, bool scheduled)
        {
            switch (summaryType)
            {
                case SummaryType.Text:
                    return new StringTaskSummary("{0}{1} = {2} s".FormatInvariant(
                        scheduled ? "Scheduled: " : string.Empty,
                        string.Join(" + ", this.Durations.Select(d => d.DurationInSeconds)),
                        this.Durations.Sum(d => d.DurationInSeconds)));

                case SummaryType.Dictionary:
                    DictionaryTaskSummary dictionary = new DictionaryTaskSummary();

                    if (scheduled)
                    {
                        dictionary.Add("Scheduled", scheduled.ToString());
                    }

                    for (int i = 0; i < this.Durations.Length; i++)
                    {
                        dictionary.Add("Duration " + i, "{0} s".FormatInvariant(this.Durations[i].DurationInSeconds));
                    }

                    return dictionary;

                default:
                    throw new NotSupportedException<SummaryType>(summaryType);
            }
        }
    }
}