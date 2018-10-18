using System;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.UnitTests
{
    [Serializable]
    public sealed class FakeTaskJobSettings : ITaskJobSettings
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }
}