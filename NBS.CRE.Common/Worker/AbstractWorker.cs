using NBS.CRE.Common.Messaging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Threading;
using System.Xml.Linq;

namespace NBS.CRE.Common.Worker
{
    /// <summary>
    /// Abstract base class for creating Remote Workers
    /// </summary>
    public abstract class AbstractWorker
    {
        public AbstractWorker(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        /// <value>Gets dependency injection container.</value>
        protected IServiceProvider ServiceProvider { get; private set; }

        /// <value>Gets the message routing key.</value>
        protected abstract string ActionCode { get; }

        /// <summary>
        /// Process the request message.
        /// </summary>
        /// <param name="request">Message request details.</param>
        /// <returns>XML response data.</returns>
        protected abstract XElement Process(RequestMessage request);

        /// <summary>
        /// Run the worker process. Creates a connection to the RabbitMQ broker and processes the received messaged.
        /// </summary>
        /// <param name="autoResetEvent">Reset event to signals the method to exit.</param>
        /// <exception cref="InvalidOperationException">Thrown if the RabbitMQ connection cannot be resolved.</exception>
        public void Run(AutoResetEvent autoResetEvent)
        {
            IConnection connection = ServiceProvider.GetService(typeof(IConnection)) as IConnection;
            if (connection == null)
            {
                throw new InvalidOperationException("RabbitMQ IConnection service not configured");
            }

            using (var channel = connection.CreateModel())
            {
                ChannelHelper.EnsureQueue(ActionCode, channel);
                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: true);

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {
                    var props = ea.BasicProperties;
                    var replyProps = channel.CreateBasicProperties();

                    RequestMessage request = SerializationHelper.FromBytes<RequestMessage>(ea.Body);
                    ResponseMessage response = null;

                    replyProps.CorrelationId = props.CorrelationId;

                    try
                    {
                        XElement data = Process(request);
                        response = ResponseMessage.CreateResponseOK(data);
                    }
                    catch (Exception ex)
                    {
                        response = ResponseMessage.CreateResponseERROR(ex);
                    }
                    finally
                    {
                        if (response != null)
                        {
                            var responseBytes = SerializationHelper.ToBytes(response);
                            channel.BasicPublish(exchange: "",
                                routingKey: props.ReplyTo,
                                basicProperties: replyProps,
                                body: responseBytes);
                        }

                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                };

                channel.BasicConsume(queue: ActionCode,
                                     autoAck: false,
                                     consumer: consumer);

                autoResetEvent.WaitOne();
            }
        }
    }
}
