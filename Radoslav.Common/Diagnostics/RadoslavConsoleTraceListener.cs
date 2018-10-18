using System;
using System.Diagnostics;

namespace Radoslav.Diagnostics
{
    /// <summary>
    /// Console trace listener for RTA applications.
    /// </summary>
    public sealed class RadoslavConsoleTraceListener : ConsoleTraceListener
    {
        private static readonly object ConsoleForegoundColorLockObject = new object();

        /// <inheritdoc />
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            lock (RadoslavConsoleTraceListener.ConsoleForegoundColorLockObject)
            {
                ConsoleColor oldColor = Console.ForegroundColor;

                switch (eventType)
                {
                    case TraceEventType.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;

                    case TraceEventType.Warning:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;

                    case TraceEventType.Information:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                }

                this.WriteLine(message, eventType);

                Console.ForegroundColor = oldColor;
            }
        }

        /// <inheritdoc />
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            this.TraceEvent(eventCache, source, eventType, id, format.FormatInvariant(args));
        }

        /// <summary>
        /// Writes a message to the listener, followed by a line terminator.
        /// </summary>
        /// <param name="message">The message to write.</param>
        public override void WriteLine(string message)
        {
            this.WriteLine(message, TraceEventType.Verbose);
        }

        /// <inheritdoc />
        public override void WriteLine(string message, string category)
        {
            this.Write(DateTime.UtcNow.TimeOfDay);
            this.Write(": ");

            if (category != null)
            {
                category = category.ToUpperInvariant();

                this.Writer.Write(category);
                this.Writer.Write(": ");
            }

            this.Writer.WriteLine(message);
        }

        private void WriteLine(string message, TraceEventType eventType)
        {
            if ((this.Filter == null) || this.Filter.ShouldTrace(null, null, eventType, 0, null, null, null, null))
            {
                this.WriteLine(message, eventType.ToString());
            }
        }
    }
}