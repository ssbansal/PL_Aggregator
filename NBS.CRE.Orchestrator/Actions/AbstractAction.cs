using NBS.CRE.Common.Messaging;
using NBS.CRE.Common.RPC;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NBS.CRE.Orchestrator.Actions
{
    public abstract class AbstractAction : IActionWrapper
    {
        public string Id { get; private set; }
        public abstract string Name { get; }
        public abstract string ActionCode { get; }

        public virtual int SecondsTimeout
        {
            get { return 5; }
        }
        public bool ActionProcessed { get; private set; }
        public bool ActionSkip { get; private set; }
        public bool ActionError { get; private set; }
        public string ErrorMessage { get; private set; }
        public DateTime ActionStartTS { get; private set; }
        public DateTime ActionEndTS { get; private set; }
        public IReadOnlyDictionary<string, object> Parameters
        {
            get
            {
                return _parameters;
            }
        }

        private List<AbstractAction> _actions = new List<AbstractAction>();
        private Dictionary<string, object> _parameters = new Dictionary<string, object>();
        private Func<ISchedulerContext, Dictionary<string, object>> _parameterResolver = (ISchedulerContext context) => new Dictionary<string, object>();
        private Func<ISchedulerContext, XElement> _messageDataResolver = (ISchedulerContext context) => null;
        private Func<ISchedulerContext, bool> _canSkipPredicate = (ISchedulerContext context) => false;
        private Action<ISchedulerContext> _postProcessing = (ISchedulerContext context) => { };
        public AbstractAction()
        {
            Id = Guid.NewGuid().ToString("N");
            _actions.Add(this);
        }

        public IReadOnlyCollection<AbstractAction> GetWrappedActions()
        {
            return _actions.AsReadOnly();
        }
        public AbstractAction WithParameters(Func<ISchedulerContext, Dictionary<string, object>> parameterResolver = null)
        {
            if (parameterResolver != null)
            {
                _parameterResolver = parameterResolver;
            }

            return this;
        }

        public AbstractAction WithSkipCondition(Func<ISchedulerContext, bool> canSkipPredicate = null)
        {
            if (canSkipPredicate != null)
            {
                _canSkipPredicate = canSkipPredicate;
            }

            return this;
        }

        public AbstractAction WithPostProcessing(Action<ISchedulerContext> postProcessing)
        {
            if (postProcessing != null)
            {
                _postProcessing = postProcessing;
            }

            return this;
        }

        public AbstractAction WithMessageData(Func<ISchedulerContext, XElement> messageDataResolver)
        {
            if (messageDataResolver != null)
            {
                _messageDataResolver = messageDataResolver;
            }

            return this;
        }

        public async Task Execute(IServiceProvider serviceProvider, ISchedulerContext context)
        {
            var client = serviceProvider.GetService(typeof(RpcClient)) as RpcClient;
            if (client == null)
            {
                throw new InvalidOperationException("RpcClient service not configured");
            }

            _parameters = _parameterResolver(context);

            ActionStartTS = DateTime.Now;
            ActionSkip = _canSkipPredicate(context);
            ActionProcessed = false;
            ActionError = false;

            if (!ActionSkip)
            {
                try
                {
                    XElement requestData = _messageDataResolver(context);
                    List<MessageParameter> requestParameters = new List<MessageParameter>();

                    foreach (var paramKey in _parameters.Keys)
                    {
                        requestParameters.Add(new MessageParameter(paramKey, _parameters[paramKey]));
                    }

                    RequestMessage request = new RequestMessage(ActionCode, requestData, requestParameters);
                    ResponseMessage response = await client.Execute(request, SecondsTimeout);
                    context.AppendContextData(this, response.Data);

                    ActionProcessed = true;
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                    ActionError = true;
                }
            }
            else
            {
                ActionProcessed = true;
            }

            ActionEndTS = DateTime.Now;

            if (ActionProcessed)
            {
                _postProcessing(context);
            }
        }
    }
}
