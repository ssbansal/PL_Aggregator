using RabbitMQ.Client;
using System;

namespace NBS.CRE.Common.Messaging
{
    public static class ChannelHelper
    {
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
