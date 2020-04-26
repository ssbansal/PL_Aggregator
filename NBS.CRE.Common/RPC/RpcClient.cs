using NBS.CRE.Common.Messaging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NBS.CRE.Common.RPC
{
    /// <summary>
    /// RpcClient implements Request-Reply pattern over RabbitMQ.
    /// </summary>
    public class RpcClient : IDisposable
    {
        private bool disposed = false;
        private string clientID;
        private List<string> actionCodes;

        private IConnection connection;
        private IModel channel;
        private EventingBasicConsumer consumer;
        private ConcurrentDictionary<string, TaskCompletionSource<ResponseMessage>> tasksDict;
        public RpcClient(IConnection rmqConnection)
        {
            clientID = Guid.NewGuid().ToString("N");
            actionCodes = new List<string>();
            tasksDict = new ConcurrentDictionary<string, TaskCompletionSource<ResponseMessage>>();

            connection = rmqConnection;

            channel = connection.CreateModel();
            ChannelHelper.EnsureQueue(ReplyQueueName, channel, true);
            consumer = new EventingBasicConsumer(channel);
            consumer.Received += Consumer_Received;

            channel.BasicConsume(queue: ReplyQueueName,
                autoAck: true,
                consumer: consumer);
        }

        /// <summary>
        /// Callback method for receiving message.
        /// </summary>
        private void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var correlationId = e.BasicProperties.CorrelationId;
            if (tasksDict.TryRemove(correlationId, out TaskCompletionSource<ResponseMessage> completionSource))
            {
                try
                {
                    ResponseMessage response = SerializationHelper.FromBytes<ResponseMessage>(e.Body);
                    if (response.IsError)
                    {
                        completionSource.SetException(response.Error);
                    }
                    else
                    {
                        completionSource.SetResult(response);
                    }
                }
                catch (Exception ex)
                {
                    completionSource.SetException(ex);
                }
            }
        }

        /// <summary>
        /// Publish message to the message broker.
        /// </summary>
        /// <param name="request">Request object instance.</param>
        /// <param name="secondsTimeout">Number of seconds to wait for action completion.</param>
        /// <returns>Response generated from remote action execution.</returns>
        /// <exception cref="TimeoutException">Exception raised when the action does not completes before the timeout period.</exception>
        public Task<ResponseMessage> Execute(RequestMessage request, int secondsTimeout = 15)
        {
            var correlationId = Guid.NewGuid().ToString("N");
            var completionSource = tasksDict.GetOrAdd(correlationId, new TaskCompletionSource<ResponseMessage>());

            var bytes = SerializationHelper.ToBytes(request);
            var props = channel.CreateBasicProperties();
            props.DeliveryMode = 2;
            props.CorrelationId = correlationId;
            props.ReplyTo = ReplyQueueName;

            if (!actionCodes.Contains(request.ActionCode))
            {
                ChannelHelper.EnsureQueue(request.ActionCode, channel);
                actionCodes.Add(request.ActionCode);
            }

            channel.BasicPublish(
                exchange: "",
                routingKey: request.ActionCode,
                basicProperties: props,
                body: bytes);

            Task.WhenAny(completionSource.Task, Task.Delay(secondsTimeout * 1000)).ContinueWith(t =>
            {
                if (!completionSource.Task.IsCompleted)
                {
                    if (tasksDict.TryRemove(correlationId, out TaskCompletionSource<ResponseMessage> source))
                    {
                        source.SetException(new TimeoutException());
                    }
                }
            });

            return completionSource.Task;
        }

        /// <value>Reply-To queue name.</value>
        private string ReplyQueueName
        {
            get
            {
                return "QUEUE_" + clientID;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Managed Objects
            }

            if (consumer != null)
            {
                consumer.Received -= Consumer_Received;
            }

            if (channel != null)
            {
                if (channel.IsOpen)
                {
                    channel.Close();
                }

                channel.Dispose();
            }

            disposed = true;
        }

        ~RpcClient()
        {
            Dispose(false);
        }
    }
}
