using System;

namespace Radoslav.TaskProcessor.Model
{
    [Serializable]
    public sealed class DemoTaskJobSettings : ITaskJobSettings
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }
}