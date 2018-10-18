using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Radoslav
{
    /// <summary>
    /// Static class for extensions of common classes like <see cref="System.String"/>, <see cref="System.Exception"/>, <see cref="System.DateTime"/>.
    /// </summary>
    public static partial class Helpers
    {
        private static readonly Random Randomizer = new Random();

        private static readonly IDictionary<Type, Func<TypeConverter>> ConvertersFactory = new Dictionary<Type, Func<TypeConverter>>()
        {
            { typeof(string), () => new StringConverter() },
            { typeof(TimeSpan), () => new TimeSpanConverter() },
            { typeof(DateTime), () => new DateTimeConverter() },
            { typeof(int), () => new Int32Converter() },
            { typeof(bool), () => new BooleanConverter() }
        };

        /// <summary>
        /// Registers a default converter for type.
        /// </summary>
        /// <typeparam name="TFromType">The type of the objects to convert from.</typeparam>
        /// <typeparam name="TConverter">The type of the converter to use.</typeparam>
        public static void RegisterDefaultConverter<TFromType, TConverter>()
            where TConverter : TypeConverter, new()
        {
            Helpers.ConvertersFactory.Add(typeof(TFromType), () => new TConverter());
        }

        /// <summary>
        /// Retrieves the value of the current <see cref="Nullable{T}"/> object, or the specified default value returned by a callback.
        /// </summary>
        /// <typeparam name="T">The underlying value type of the <see cref="Nullable{T}"/> generic type.</typeparam>
        /// <param name="value">The <see cref="Nullable{T}"/> value to extend.</param>
        /// <param name="defaultValueCallback">The callback to provide the default value in case that
        /// <see cref="Nullable{T}"/>.<see cref="Nullable{T}.HasValue"/> property is false.</param>
        /// <returns> The value of the <see cref="Nullable{T}"/>.<see cref="Nullable{T}.Value"/> property if the
        /// <see cref="Nullable{T}"/>.<see cref="Nullable{T}.HasValue"/> property is true; otherwise,
        /// the value returned by the <paramref name="defaultValueCallback"/> parameter.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="defaultValueCallback"/> is null.</exception>
        public static T GetValueOrDefault<T>(this T? value, Func<T> defaultValueCallback)
            where T : struct
        {
            if (defaultValueCallback == null)
            {
                throw new ArgumentNullException("defaultValueCallback");
            }

            return value.HasValue ? value.Value : defaultValueCallback();
        }

        /// <summary>
        /// Gets a default <see cref="TypeConverter"/> for a specified type.
        /// </summary>
        /// <param name="forType">The type to get a converter for.</param>
        /// <returns>The default <see cref="TypeConverter"/> for the specified type.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="forType"/> is null.</exception>
        /// <exception cref="NotSupportedException">The specified type is not supported.</exception>
        public static TypeConverter GetDefaultTypeConverter(Type forType)
        {
            if (forType == null)
            {
                throw new ArgumentNullException("forType");
            }

            Func<TypeConverter> factoryMethod;

            if (Helpers.ConvertersFactory.TryGetValue(forType, out factoryMethod))
            {
                return factoryMethod();
            }
            else
            {
                throw new NotSupportedException<Type>(forType);
            }
        }

        /// <summary>
        /// Tries to execute a method and returns a <see cref="Boolean"/> value whether it completed successfully (without exception).
        /// </summary>
        /// <param name="callback">The method to try to execute.</param>
        /// <param name="traceOnFail">Whether to trace the error.</param>
        /// <returns><c>true</c> if method completed without throwing an exception; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="callback"/> is null.</exception>
        [DebuggerStepThrough]
        public static bool TryToExecute(Action callback, bool traceOnFail)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            try
            {
                callback();

                return true;
            }
            catch (Exception ex)
            {
                if (traceOnFail)
                {
                    Trace.WriteLine(ex);
                }

                if (ex.IsCritical())
                {
                    throw;
                }

                return false;
            }
        }

        /// <summary>
        /// Tries to execute a method and returns a <see cref="Boolean"/> value whether it completed successfully (without exception).
        /// </summary>
        /// <param name="callback">The method to try to execute.</param>
        /// <returns><c>true</c> if method completed without throwing an exception; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="callback"/> is null.</exception>
        [DebuggerStepThrough]
        public static bool TryToExecute(Action callback)
        {
            return Helpers.TryToExecute(callback, true);
        }

        /// <summary>
        /// Waits for an event to be raised, or raises a <see cref="TimeoutException"/> after a specified timeout.
        /// </summary>
        /// <param name="timeout">The timeout to wait for the event to be raised.</param>
        /// <param name="subscribeForEventCallback">Callback to subscribe for the event.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="subscribeForEventCallback"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Parameter <paramref name="timeout"/> is less than <see cref="TimeSpan.Zero"/>.</exception>
        /// <exception cref="TimeoutException">Event was not raised within the specified timeout.</exception>
        public static void WaitForEvent(TimeSpan timeout, Action<EventHandler> subscribeForEventCallback)
        {
            Helpers.WaitForEvent(timeout, subscribeForEventCallback, () => { });
        }

        /// <summary>
        /// Waits for an event to be raised.
        /// </summary>
        /// <param name="subscribeForEventCallback">Callback to subscribe for the event.</param>
        /// <param name="raiseEventCallback">Callback that should raise the event.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="subscribeForEventCallback"/> or <paramref name="raiseEventCallback"/> is null.</exception>
        public static void WaitForEvent(Action<EventHandler> subscribeForEventCallback, Action raiseEventCallback)
        {
            if (subscribeForEventCallback == null)
            {
                throw new ArgumentNullException("subscribeForEventCallback");
            }

            if (raiseEventCallback == null)
            {
                throw new ArgumentNullException("raiseEventCallback");
            }

            ManualResetEventSlim waitHandler = new ManualResetEventSlim();

            subscribeForEventCallback(new EventHandler((sender, e) =>
            {
                waitHandler.Set();
            }));

            if (raiseEventCallback != null)
            {
                raiseEventCallback();
            }

            waitHandler.Wait();
        }

        /// <summary>
        /// Waits for an event to be raised, or raises a <see cref="TimeoutException"/> after a specified timeout.
        /// </summary>
        /// <param name="timeout">The timeout to wait for the event to be raised.</param>
        /// <param name="subscribeForEventCallback">Callback to subscribe for the event.</param>
        /// <param name="raiseEventCallback">Callback that should raise the event.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="subscribeForEventCallback"/> or <paramref name="raiseEventCallback"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Parameter <paramref name="timeout"/> is less than <see cref="TimeSpan.Zero"/>.</exception>
        /// <exception cref="TimeoutException">Event was not raised within the specified timeout.</exception>
        public static void WaitForEvent(TimeSpan timeout, Action<EventHandler> subscribeForEventCallback, Action raiseEventCallback)
        {
            if (timeout < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("timeout", timeout, "Value must not be negative.");
            }

            if (subscribeForEventCallback == null)
            {
                throw new ArgumentNullException("subscribeForEventCallback");
            }

            if (raiseEventCallback == null)
            {
                throw new ArgumentNullException("raiseEventCallback");
            }

            ManualResetEventSlim waitHandler = new ManualResetEventSlim();

            subscribeForEventCallback(new EventHandler((sender, e) =>
            {
                waitHandler.Set();
            }));

            if (raiseEventCallback != null)
            {
                raiseEventCallback();
            }

            if (!waitHandler.Wait(timeout))
            {
                throw new TimeoutException("Event not raised after '{0}' timeout.".FormatInvariant(timeout));
            }
        }

        /// <summary>
        /// Waits for an event to be raised and returns its <see cref="EventArgs"/>, or raises a <see cref="TimeoutException"/> after a specified timeout.
        /// </summary>
        /// <typeparam name="TEventArgs">The type of the event arguments.</typeparam>
        /// <param name="timeout">The timeout to wait for the event to be raised.</param>
        /// <param name="subscribeForEventCallback">Callback to subscribe for the event.</param>
        /// <returns>The event arguments of the raised event.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="subscribeForEventCallback"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Parameter <paramref name="timeout"/> is less than <see cref="TimeSpan.Zero"/>.</exception>
        /// <exception cref="TimeoutException">Event was not raised within the specified timeout.</exception>
        public static TEventArgs WaitForEvent<TEventArgs>(TimeSpan timeout, Action<EventHandler<TEventArgs>> subscribeForEventCallback)
            where TEventArgs : EventArgs
        {
            return Helpers.WaitForEvent(timeout, subscribeForEventCallback, () => { });
        }

        /// <summary>
        /// Waits for an event to be raised and returns its <see cref="EventArgs"/>, or raises a <see cref="TimeoutException"/> after a specified timeout.
        /// </summary>
        /// <typeparam name="TEventArgs">The type of the event arguments.</typeparam>
        /// <param name="timeout">The timeout to wait for the event to be raised.</param>
        /// <param name="subscribeForEventCallback">Callback to subscribe for the event.</param>
        /// <param name="raiseEventCallback">Callback that should raise the event.</param>
        /// <returns>The event arguments of the raised event.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="subscribeForEventCallback"/> or <paramref name="raiseEventCallback"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Parameter <paramref name="timeout"/> is less than <see cref="TimeSpan.Zero"/>.</exception>
        /// <exception cref="TimeoutException">Event was not raised within the specified timeout.</exception>
        public static TEventArgs WaitForEvent<TEventArgs>(TimeSpan timeout, Action<EventHandler<TEventArgs>> subscribeForEventCallback, Action raiseEventCallback)
            where TEventArgs : EventArgs
        {
            if (timeout < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("timeout", timeout, "Value must not be negative.");
            }

            if (subscribeForEventCallback == null)
            {
                throw new ArgumentNullException("subscribeForEventCallback");
            }

            if (raiseEventCallback == null)
            {
                throw new ArgumentNullException("raiseEventCallback");
            }

            TEventArgs result = null;

            ManualResetEventSlim waitHandler = new ManualResetEventSlim();

            subscribeForEventCallback(new EventHandler<TEventArgs>((sender, e) =>
            {
                result = e;

                waitHandler.Set();
            }));

            if (raiseEventCallback != null)
            {
                raiseEventCallback();
            }

            if (waitHandler.Wait(timeout))
            {
                return result;
            }
            else
            {
                throw new TimeoutException("Event not raised after '{0}' timeout.".FormatInvariant(timeout));
            }
        }

        /// <summary>
        /// Gets an unique process instance name by a process ID.
        /// </summary>
        /// <remarks>
        /// <para>Process instance name is used by performance counters.</para>
        /// <para>More info at: <a href="http://stackoverflow.com/questions/9115436/performance-counter-by-process-id-instead-of-name">Stack Overflow</a>.</para>
        /// </remarks>
        /// <param name="processId">The unique ID of the process.</param>
        /// <returns>The unique process instance name for the specified process ID, or null if not found.</returns>
        public static string GetProcessInstanceName(int processId)
        {
            PerformanceCounterCategory category = new PerformanceCounterCategory("Process");

            foreach (string instance in category.GetInstanceNames())
            {
                using (PerformanceCounter cnt = new PerformanceCounter("Process", "ID Process", instance, true))
                {
                    if (processId == (int)cnt.RawValue)
                    {
                        return instance;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the parent process ID of a process.
        /// </summary>
        /// <param name="childProcessId">The ID of the child process whose parent process ID must be returned.</param>
        /// <returns>The ID of the parent process.</returns>
        public static int GetParentProcessId(int childProcessId)
        {
            string processInstanceName = Helpers.GetProcessInstanceName(childProcessId);

            using (PerformanceCounter counter = new PerformanceCounter("Process", "Creating Process ID", processInstanceName))
            {
                return Convert.ToInt32(counter.NextValue());
            }
        }

        /// <summary>
        /// Gets the directory of the executable file that is currently executed.
        /// </summary>
        /// <returns>The directory of the executable file that is currently executed.</returns>
        public static string GetCurrentExeDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase.Replace("file:///", string.Empty));
        }
    }
}