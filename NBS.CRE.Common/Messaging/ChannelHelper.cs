using RabbitMQ.Client;
using System;

namespace NBS.CRE.Common.Messaging
{
    /// <summary>
    /// Channel Helper 
    /// </summary>
    public static class ChannelHelper
    {
        /// <summary>
        /// Ensure the queue exists in the RabbitMQ broker.
        /// </summary>
        /// <param name="queueName">Queue name to check.</param>
        /// <param name="channel">RabbitMQ channel.</param>
        /// <param name="exclusive">When <c>true</c> creates an exclusive channel.</param>
        public static void EnsureQueue(string queueName, IModel channel, bool exclusive = false)
        {
            if (String.IsNullOrEmpty(queueName))
                throw new ArgumentNullException(nameof(queueName));

            channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: exclusive,
                                 autoDelete: true,
                                 arguments: null);
        }
    }
}
