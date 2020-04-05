using NBS.CRE.Common.Messaging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Threading;
using System.Xml.Linq;

namespace NBS.CRE.Common.Worker
{
    public abstract class AbstractWorker
    {
        public AbstractWorker(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
        protected IServiceProvider ServiceProvider { get; private set; }
        protected abstract string ActionCode { get; }
        protected abstract XElement Process(RequestMessage request);
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
