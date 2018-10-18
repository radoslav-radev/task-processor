using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor
{
    public sealed class TaskViewModel : INotifyPropertyChanged
    {
        #region Fields & Constructor

        private readonly Guid taskId;
        private readonly Type taskType;
        private readonly TaskPriority priority;
        private readonly DateTime submittedUtc;
        private readonly string pollingQueue;

        private double percentage;
        private string status;
        private Guid? taskProcessorId;
        private DateTime? startedUtc;
        private DateTime? canceledUtc;
        private DateTime? completedUtc;
        private float? cpuPercent;
        private float? ramPercent;
        private TimeSpan? cpuTime;
        private string error;

        internal TaskViewModel(ITaskRuntimeInfo taskInfo)
        {
            this.taskId = taskInfo.TaskId;
            this.taskType = taskInfo.TaskType;
            this.priority = taskInfo.Priority;
            this.submittedUtc = taskInfo.SubmittedUtc;
            this.status = taskInfo.Status.ToString();
            this.startedUtc = taskInfo.StartedUtc;
            this.canceledUtc = taskInfo.CanceledUtc;
            this.completedUtc = taskInfo.CompletedUtc;
            this.taskProcessorId = taskInfo.TaskProcessorId;
            this.pollingQueue = taskInfo.PollingQueue;
            this.error = taskInfo.Error;
        }

        #endregion Fields & Constructor

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged Members

        #region Properties

        public Guid TaskId
        {
            get
            {
                return this.taskId;
            }

            set
            {
                if (this.taskId == value)
                {
                    return;
                }

                throw new InvalidOperationException("Task ID is read-only; editing is enable only in order to copy task ID to clipboard.");
            }
        }

        public Type TaskType
        {
            get { return this.taskType; }
        }

        public TaskPriority Priority
        {
            get { return this.priority; }
        }

        public string PollingQueue
        {
            get { return this.pollingQueue; }
        }

        public string Status
        {
            get
            {
                return this.status;
            }

            internal set
            {
                if (this.status == value)
                {
                    return;
                }

                this.status = value;

                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Status"));
                    this.PropertyChanged(this, new PropertyChangedEventArgs("CanCancel"));
                }
            }
        }

        public double Percentage
        {
            get
            {
                return this.percentage;
            }

            internal set
            {
                if (this.percentage == value)
                {
                    return;
                }

                this.percentage = value;

                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Percentage"));
                }
            }
        }

        public Guid? TaskProcessorId
        {
            get
            {
                return this.taskProcessorId;
            }

            internal set
            {
                if (this.taskProcessorId == value)
                {
                    return;
                }

                this.taskProcessorId = value;

                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("TaskProcessorId"));
                }
            }
        }

        public DateTime SubmittedUtc
        {
            get
            {
                return this.submittedUtc;
            }
        }

        public DateTime? StartedUtc
        {
            get
            {
                return this.startedUtc;
            }

            internal set
            {
                if (this.startedUtc == value)
                {
                    return;
                }

                this.startedUtc = value;

                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("StartedUtc"));
                }
            }
        }

        public DateTime? CanceledUtc
        {
            get
            {
                return this.canceledUtc;
            }

            internal set
            {
                if (this.canceledUtc == value)
                {
                    return;
                }

                this.canceledUtc = value;

                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("CanceledUtc"));
                }
            }
        }

        public DateTime? CompletedUtc
        {
            get
            {
                return this.completedUtc;
            }

            internal set
            {
                if (this.completedUtc == value)
                {
                    return;
                }

                this.completedUtc = value;

                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("CompletedUtc"));
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Duration"));
                }
            }
        }

        public TimeSpan? Duration
        {
            get
            {
                if (this.completedUtc.HasValue)
                {
                    return this.completedUtc.Value - this.submittedUtc;
                }
                else
                {
                    return null;
                }
            }
        }

        public float? CpuPercent
        {
            get
            {
                return this.cpuPercent;
            }

            internal set
            {
                this.cpuPercent = value;

                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("CpuPercent"));
                }
            }
        }

        public float? RamPercent
        {
            get
            {
                return this.ramPercent;
            }

            internal set
            {
                this.ramPercent = value;

                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("RamPercent"));
                }
            }
        }

        public TimeSpan? CpuTime
        {
            get
            {
                return this.cpuTime;
            }

            internal set
            {
                this.cpuTime = value;

                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("CpuTime"));
                }
            }
        }

        public string Error
        {
            get
            {
                return this.error;
            }

            internal set
            {
                this.error = value;

                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Error"));
                }
            }
        }

        #endregion Properties

        internal void ShowTaskSummary(ITaskSummary summary)
        {
            string summaryText;

            if (summary == null)
            {
                summaryText = "No summary.";
            }
            else if (summary is StringTaskSummary)
            {
                summaryText = (StringTaskSummary)summary;
            }
            else if (summary is DictionaryTaskSummary)
            {
                summaryText = string.Join(Environment.NewLine,
                    ((DictionaryTaskSummary)summary).Select(p => "{0} = {1}".FormatInvariant(p.Key, p.Value)));
            }
            else
            {
                summaryText = string.Concat(summary, Environment.NewLine, Environment.NewLine,
                    string.Join(Environment.NewLine,
                        summary.GetType().GetProperties()
                            .Select(p => "{0} = {1}".FormatInvariant(p.Name, p.GetValue(summary)))));
            }

            MessageBox.Show(summaryText, "Task Summary", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}