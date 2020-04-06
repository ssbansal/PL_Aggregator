using NBS.CRE.Common.Messaging;
using NBS.CRE.Common.RPC;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NBS.CRE.Orchestrator.Actions
{
    /// <summary>
    /// Abstract class for all actions.
    /// </summary>
    public abstract class AbstractAction : IActionWrapper
    {
        /// <value>Unique id of the action instance.</value>
        public string Id { get; private set; }
        
        /// <value>User friendly action name.</value>
        public abstract string Name { get; }

        /// <value>Action code for message routing used by <see cref="RpcClient"/>.</value>
        public abstract string ActionCode { get; }

        /// <value>Number of seconds the orchestrator waits for the action to complete.</value>
        public virtual int SecondsTimeout
        {
            get { return 5; }
        }

        /// <value>Returns <c>true</c> when the action execution completes without an error.</value>
        public bool ActionProcessed { get; private set; }

        /// <value>Returns <c>true</c> when the action execution is skipped.</value>
        public bool ActionSkip { get; private set; }

        /// <value>Returns <c>true</c> when the action execution completes with an error.</value>
        public bool ActionError { get; private set; }

        /// <value>Error message that resulted in action execution failure.</value>
        public string ErrorMessage { get; private set; }

        /// <value>Action execution start time.</value>
        public DateTime ActionStartTS { get; private set; }

        /// <value>Action execution end time.</value>
        public DateTime ActionEndTS { get; private set; }

        /// <value>Action execution parmeters.</value>
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

        /// <summary>
        /// Gets wrapped actions. Also see <see cref="ParallelActionsGroup"/>.
        /// </summary>
        /// <returns>Readonly collection of <see cref="AbstractAction"/> instances.</returns>
        public IReadOnlyCollection<AbstractAction> GetWrappedActions()
        {
            return _actions.AsReadOnly();
        }

        /// <summary>
        /// Set a parameters resolver function delegate.
        /// </summary>
        /// <param name="parameterResolver">Function that returns a list of parameters.</param>
        /// <returns>Instance of self (<see cref="AbstractAction"/>) for chaining methods.</returns>
        public AbstractAction WithParameters(Func<ISchedulerContext, Dictionary<string, object>> parameterResolver = null)
        {
            if (parameterResolver != null)
            {
                _parameterResolver = parameterResolver;
            }

            return this;
        }

        /// <summary>
        /// Set a skip predicate function delegate.
        /// </summary>
        /// <param name="canSkipPredicate">Function delegate that returns <c>true</c> if the action execution should be skipped.</param>
        /// <returns>Instance of self (<see cref="AbstractAction"/>) for chaining methods.</returns>
        public AbstractAction WithSkipCondition(Func<ISchedulerContext, bool> canSkipPredicate = null)
        {
            if (canSkipPredicate != null)
            {
                _canSkipPredicate = canSkipPredicate;
            }

            return this;
        }

        /// <summary>
        /// Set a <see cref="Action"/> deledate for post processing.
        /// </summary>
        /// <param name="postProcessing">Action delegate that will be called when the action execution completes successfully.</param>
        /// <returns>Instance of self (<see cref="AbstractAction"/>) for chaining methods.</returns>
        public AbstractAction WithPostProcessing(Action<ISchedulerContext> postProcessing)
        {
            if (postProcessing != null)
            {
                _postProcessing = postProcessing;
            }

            return this;
        }

        /// <summary>
        /// Set a function delegate for generating action execution data. 
        /// </summary>
        /// <param name="messageDataResolver">Function delegate that returns action execution data.</param>
        /// <returns>Instance of self (<see cref="AbstractAction"/>) for chaining methods.</returns>
        public AbstractAction WithMessageData(Func<ISchedulerContext, XElement> messageDataResolver)
        {
            if (messageDataResolver != null)
            {
                _messageDataResolver = messageDataResolver;
            }

            return this;
        }

        /// <summary>
        /// Asynchronously executes the action using <see cref="RpcClient"/>.
        /// </summary>
        /// <param name="serviceProvider">Dependency injection container for resolving services.</param>
        /// <param name="context">Scheduler context for executing actions.</param>
        /// <returns><c>Task</c> object executing the action.</returns>
        /// <exception cref="InvalidOperationException"><see cref="RpcClient"/> object is not configured.</exception>
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
