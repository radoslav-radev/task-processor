using System;

namespace Radoslav.Redis
{
    /// <summary>
    /// Event arguments for a message received from a channel via Redis.
    /// </summary>
    public sealed class RedisMessageEventArgs : EventArgs
    {
        private readonly string channel;
        private readonly string message;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisMessageEventArgs"/> class.
        /// </summary>
        /// <param name="channel">The channel on which the message was received.</param>
        /// <param name="message">The message that was received.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="channel"/> is null or empty string, or <paramref name="message"/> is null.</exception>
        public RedisMessageEventArgs(string channel, string message)
        {
            if (string.IsNullOrEmpty(channel))
            {
                throw new ArgumentNullException("channel");
            }

            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            this.channel = channel;
            this.message = message;
        }

        /// <summary>
        /// Gets the channel on which the message was received.
        /// </summary>
        /// <value>The channel on which the message was received.</value>
        public string Channel
        {
            get { return this.channel; }
        }

        /// <summary>
        /// Gets the message that was received.
        /// </summary>
        /// <value>The message that was received.</value>
        public string Message
        {
            get { return this.message; }
        }
    }
}