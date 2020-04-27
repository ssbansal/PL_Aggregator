using RabbitMQ.Client;
using System;
using System.Collections.Generic;

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

            var args = new Dictionary<string, object>();
            if (exclusive)
            {
                args.Add("x-expires", 1000 * 60 * 5);
            }

            channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 //exclusive: exclusive,
                                 autoDelete: true,
                                 arguments: args);
        }
    }
}
