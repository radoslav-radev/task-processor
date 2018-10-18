using System;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.Sample
{
    [Serializable]
    public sealed class SampleTaskJobSettings : ITaskJobSettings
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }
}