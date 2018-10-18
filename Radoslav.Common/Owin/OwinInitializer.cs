using System;
using System.Diagnostics;
using Radoslav.WfmLive.BL.Messaging;

namespace Radoslav.Owin
{
    /// <summary>
    /// Class for OWIN initialization.
    /// </summary>
    public static class OwinInitializer
    {
        private static readonly Random Random = new Random();

        /// <summary>
        /// Initializes an OWIN host.
        /// </summary>
        /// <param name="removeOwinTraceListeners">Whether to remove the trace listeners added by OWIN.</param>
        /// <returns>A dispose to close the OWIN host.</returns>
        /// <exception cref="OwinFailedToStartException">Starting OWIN failed.</exception>
        public static IDisposable InitializeOwin(bool removeOwinTraceListeners)
        {
            return OwinInitializer.InitializeOwin(true, () => Startup.StartOwinHost());
        }

        /// <summary>
        /// Initializes an OWIN host.
        /// </summary>
        /// <param name="removeOwinTraceListeners">Whether to remove the trace listeners added by OWIN.</param>
        /// <param name="port">The port on which OWIN should listen.</param>
        /// <returns>A dispose to close the OWIN host.</returns>
        /// <exception cref="OwinFailedToStartException">Starting OWIN failed.</exception>
        public static IDisposable InitializeOwin(bool removeOwinTraceListeners, int port)
        {
            return OwinInitializer.InitializeOwin(true, () => Startup.StartOwinHost(port));
        }

        /// <summary>
        /// Initializes an OWIN host with a random port.
        /// </summary>
        /// <param name="removeOwinTraceListeners">Whether to remove the trace listeners added by OWIN.</param>
        /// <param name="minPort">The minimum port that can be used.</param>
        /// <param name="maxPort">The maximum port that can be used.</param>
        /// <param name="maxAttempts">The maximum number of attempts to listen on random port.</param>
        /// <returns>A dispose to close the OWIN host.</returns>
        /// <exception cref="OwinFailedToStartException">Starting OWIN failed every time.</exception>
        public static IDisposable InitializeOwinWithRandomPort(bool removeOwinTraceListeners, int minPort, int maxPort, int maxAttempts)
        {
            return OwinInitializer.InitializeOwin(true, OwinInitializer.InitializeOwinWithRandomPort);
        }

        /// <summary>
        /// Initializes an OWIN host with a random port.
        /// </summary>
        /// <remarks>
        /// Tries to listen on random port in the range 48620-49150
        /// (see <a href="http://stackoverflow.com/questions/2200199/how-do-you-decide-what-port-to-use">How do you decide what port to use</a>
        /// discussion in Stack Overflow.
        /// </remarks>
        /// <param name="removeOwinTraceListeners">Whether to remove the trace listeners added by OWIN.</param>
        /// <param name="maxAttempts">The maximum number of attempts to listen on random port.</param>
        /// <returns>A dispose to close the OWIN host.</returns>
        /// <exception cref="OwinFailedToStartException">Starting OWIN failed every time.</exception>
        public static IDisposable InitializeOwinWithRandomPort(bool removeOwinTraceListeners, int maxAttempts)
        {
            return OwinInitializer.InitializeOwinWithRandomPort(removeOwinTraceListeners, 48620, 49150, maxAttempts);
        }

        /// <summary>
        /// Initializes an OWIN host with a random port.
        /// </summary>
        /// <remarks>
        /// Tries 10 times to listen on random port in the range 48620-49150
        /// (see <a href="http://stackoverflow.com/questions/2200199/how-do-you-decide-what-port-to-use">How do you decide what port to use</a>
        /// discussion in Stack Overflow.
        /// </remarks>
        /// <param name="removeOwinTraceListeners">Whether to remove the trace listeners added by OWIN.</param>
        /// <returns>A dispose to close the OWIN host.</returns>
        /// <exception cref="OwinFailedToStartException">Starting OWIN failed every time.</exception>
        public static IDisposable InitializeOwinWithRandomPort(bool removeOwinTraceListeners)
        {
            return OwinInitializer.InitializeOwinWithRandomPort(removeOwinTraceListeners, 10);
        }

        private static IDisposable InitializeOwinWithRandomPort()
        {
            for (int i = 0; i < 10; i++)
            {
                int port = OwinInitializer.Random.Next(4000, 5000);

                try
                {
                    return Startup.StartOwinHost(port);
                }
                catch
                {
                }
            }

            throw new OwinFailedToStartException("Cannot initialize OWIN: all listenings to random ports failed.");
        }

        private static IDisposable InitializeOwin(bool removeOwinTraceListeners, Func<IDisposable> initializationCallback)
        {
            Trace.WriteLine("ENTER: Initializing OWIN ...");

            int oldTraceListenersCount = Trace.Listeners.Count;

            Trace.WriteLine("Starting OWIN host ...");

            IDisposable result;

            try
            {
                result = initializationCallback();
            }
            catch (OwinFailedToStartException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new OwinFailedToStartException(ex);
            }

            if (removeOwinTraceListeners)
            {
                lock (Trace.Listeners)
                {
                    for (int i = oldTraceListenersCount; i < Trace.Listeners.Count; i++)
                    {
                        Trace.Listeners.RemoveAt(i);
                    }
                }

                Trace.WriteLine("OWIN trace listeners removed.");
            }

            // Should be after removing OWIN trace listeners.
            Trace.WriteLine("OWIN host started.");

            Trace.WriteLine("EXIT: OWIN initialized.");

            return result;
        }
    }
}