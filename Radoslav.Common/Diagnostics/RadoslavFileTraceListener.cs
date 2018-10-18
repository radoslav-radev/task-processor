using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Radoslav.Diagnostics
{
    /// <summary>
    /// Logs RTA application <see cref="Debug"/> or <see cref="Trace"/> output to a log file in specific format.</summary>
    /// <remarks>
    /// <see cref="RadoslavFileTraceListener"/> writes to a log file in the following format:
    /// <code> {Date:yyyy/MM/dd hh:mm:ss.fffffff}|{Category}|{Class}|{Method}|{Thread}|{Message}</code>
    /// </remarks>
    public sealed class RadoslavFileTraceListener : TextWriterTraceListener
    {
        private static readonly string DefaultCategory = TraceLevel.Verbose.ToString();

        private readonly string fileName;

        /// <inheritdoc />
        public RadoslavFileTraceListener()
        {
            this.fileName = Path.GetTempFileName();
        }

        /// <inheritdoc />
        public RadoslavFileTraceListener(string fileName)
            : base(fileName)
        {
            this.fileName = fileName;
        }

        /// <summary>
        /// Gets the log file path.
        /// </summary>
        /// <value>The full path to the log file.</value>
        public string FilePath => this.fileName;

        /// <summary>
        /// Creates a new instance of the <see cref="RadoslavFileTraceListener"/> class with a temp file name and
        /// adds it to the <see cref="Trace"/>.<see cref="Trace.Listeners"/> collection.
        /// </summary>
        /// <returns>The newly created <see cref="RadoslavFileTraceListener"/> instance.</returns>
        public static RadoslavFileTraceListener Add()
        {
            RadoslavFileTraceListener result = new RadoslavFileTraceListener();

            Trace.Listeners.Add(result);

            return result;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="RadoslavFileTraceListener"/> class with a specified file name and
        /// adds it to the <see cref="Trace"/>.<see cref="Trace.Listeners"/> collection.
        /// </summary>
        /// <param name="fileName">The log file name.</param>
        /// <returns>The newly created <see cref="RadoslavFileTraceListener"/> instance.</returns>
        public static RadoslavFileTraceListener Add(string fileName)
        {
            RadoslavFileTraceListener result = new RadoslavFileTraceListener(fileName);

            Trace.Listeners.Add(result);

            return result;
        }

        /// <inheritdoc />
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            this.WriteLineInternal(message, eventType.ToString());
        }

        /// <inheritdoc />
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            this.WriteLineInternal(format.FormatInvariant(args), eventType.ToString());
        }

        /// <inheritdoc />
        public override void WriteLine(object o)
        {
            if (o == null)
            {
                throw new ArgumentNullException("o");
            }

            string message, category;

            Exception exception = o as Exception;

            if (exception != null)
            {
                message = exception.ToString();

                category = TraceLevel.Error.ToString();
            }
            else
            {
                message = o.ToString();

                category = RadoslavFileTraceListener.DefaultCategory;
            }

            this.WriteLineInternal(message, category);
        }

        /// <inheritdoc />
        public override void WriteLine(object o, string category)
        {
            if (o == null)
            {
                throw new ArgumentNullException("o");
            }

            if (string.IsNullOrEmpty(category))
            {
                throw new ArgumentNullException("category");
            }

            Exception exception = o as Exception;

            if (exception != null)
            {
                this.WriteLineInternal(exception.ToString(), category);
            }
            else
            {
                this.WriteLineInternal(o.ToString(), category);
            }
        }

        /// <summary>
        /// Writes a message to the listener, followed by a line terminator.
        /// </summary>
        /// <param name="message">The message to write.</param>
        public override void WriteLine(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException("message");
            }

            this.WriteLineInternal(message, RadoslavFileTraceListener.DefaultCategory);
        }

        /// <inheritdoc />
        public override void WriteLine(string message, string category)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException("message");
            }

            if (string.IsNullOrEmpty(category))
            {
                throw new ArgumentNullException("category");
            }

            this.WriteLineInternal(message, category);
        }

        /// <summary>
        /// Changes the underlying log file name.
        /// </summary>
        /// <remarks>This method removes the current <see cref="RadoslavFileTraceListener"/> instance from the
        /// <see cref="Trace"/>.<see cref="Trace.Listeners"/> collection, renames the old file to <paramref name="newFileName"/>,
        /// and creates a new <see cref="RadoslavFileTraceListener"/> instance logging to the new file name.</remarks>
        /// <param name="newFileName">The new file name.</param>
        /// <returns>A new <see cref="RadoslavFileTraceListener"/> instance with the specified file name.</returns>
        public RadoslavFileTraceListener ChangeFileName(string newFileName)
        {
            this.Flush();

            Trace.Listeners.Remove(this);

            this.Close();

            string logFileFolder = Path.GetDirectoryName(newFileName);

            if (!Directory.Exists(logFileFolder))
            {
                Directory.CreateDirectory(logFileFolder);
            }

            File.Move(this.fileName, newFileName);

            RadoslavFileTraceListener result = new RadoslavFileTraceListener(newFileName);

            Trace.Listeners.Add(result);

            return result;
        }

        private static MethodBase GetCallingMethod()
        {
            int framesToSkip = 5;

            MethodBase result = null;

            while (framesToSkip > 0)
            {
                result = new StackFrame(framesToSkip).GetMethod();

                if (result != null)
                {
                    break;
                }

                framesToSkip--;
            }

            return result;
        }

        // Must be a separated method that is called by all other WriteLine methods otherwise calling method cannot be determined.
        private void WriteLineInternal(string message, string category)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException("message");
            }

            if (string.IsNullOrEmpty(category))
            {
                throw new ArgumentNullException("category");
            }

            MethodBase currentMethod = RadoslavFileTraceListener.GetCallingMethod();

            message = string.Join(
                "|",
                 DateTime.UtcNow.ToString("yyyy\\/MM\\/dd hh:mm:ss.fffffff", CultureInfo.InvariantCulture), // Sortable format with milliseconds.
                 category.ToUpperInvariant(),
                 currentMethod.DeclaringType.Name,
                 currentMethod.Name,
                 Thread.CurrentThread.ManagedThreadId,
                 message);

            base.WriteLine(message);
        }
    }
}