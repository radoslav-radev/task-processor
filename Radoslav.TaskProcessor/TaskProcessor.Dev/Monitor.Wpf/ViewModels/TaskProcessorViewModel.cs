using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor
{
    public sealed class TaskProcessorViewModel : INotifyPropertyChanged
    {
        private readonly Guid taskProcessorId;
        private readonly string machineName;
        private readonly ObservableCollection<TaskViewModel> activeTasks = new ObservableCollection<TaskViewModel>();

        private bool isMaster;
        private TaskProcessorState state;
        private int? cpuPercent;
        private int? ramPercent;

        internal TaskProcessorViewModel(Guid taskProcessorId, string machineName)
        {
            this.taskProcessorId = taskProcessorId;
            this.machineName = machineName;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged Members

        public Guid TaskProcessorId
        {
            get { return this.taskProcessorId; }
        }

        public string MachineName
        {
            get { return this.machineName; }
        }

        public bool IsMaster
        {
            get
            {
                return this.isMaster;
            }

            internal set
            {
                this.isMaster = value;

                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("IsMaster"));
                    this.PropertyChanged(this, new PropertyChangedEventArgs("IsMakeMasterEnabled"));
                }
            }
        }

        public TaskProcessorState State
        {
            get
            {
                return this.state;
            }

            internal set
            {
                this.state = value;

                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("State"));
                    this.PropertyChanged(this, new PropertyChangedEventArgs("IsMakeMasterEnabled"));
                }
            }
        }

        public int? CpuPercent
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

        public int? RamPercent
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

        public ObservableCollection<TaskViewModel> ActiveTasks
        {
            get { return this.activeTasks; }
        }

        public bool IsMakeMasterEnabled
        {
            get
            {
                return !this.isMaster && (this.state == TaskProcessorState.Active);
            }
        }
    }
}