using NBS.CRE.Orchestrator.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NBS.CRE.Orchestrator
{
    public interface ISchedulerContext
    {
        void LogStartTime();
        void LogEndTime();
        void LogAction(IActionWrapper action);
        Task ExecuteActions();
        void AppendAction(IActionWrapper action);
        void PrependAction(IActionWrapper action);
        string Status { get; }
        string ErrorMessage { get; }
        string ToString();
        void AppendContextData(AbstractAction action, XElement responseData);
        XElement GetContextData();
        XElement GetActionDataById(string actionId);
    }
    public class SchedulerContext : ISchedulerContext
    {
        private static XNamespace ns = XNamespace.Get("http://cre.nationwide.co.uk/context");
        private XElement _contextNode = new XElement(ns + "Context");
        private XElement _dataNode = new XElement(ns + "Data");
        private XElement _orchestrationNode = new XElement(ns + "Orchestration");
        private XElement _startTimeNode = new XElement(ns + "StartTime");
        private XElement _endTimeNode = new XElement(ns + "EndTime");
        private XElement _countNode = new XElement(ns + "Count");
        private XElement _actionsNode = new XElement(ns + "Actions");
        private XElement _statusNode = new XElement(ns + "Status");
        private XElement _errorMessageNode = new XElement(ns + "ErrorMessage");

        private int _actionLogCount = 0;
        private IServiceProvider _serviceProvider;
        private LinkedList<IActionWrapper> _pendingActions = new LinkedList<IActionWrapper>();
        public SchedulerContext(IServiceProvider serviceProvider)
        {
            _contextNode.Add(_dataNode);
            _contextNode.Add(_statusNode);
            _contextNode.Add(_errorMessageNode);
            _contextNode.Add(_orchestrationNode);
            _orchestrationNode.Add(_startTimeNode);
            _orchestrationNode.Add(_endTimeNode);
            _orchestrationNode.Add(_actionsNode);
            _actionsNode.Add(_countNode);

            _serviceProvider = serviceProvider;
        }

        public override string ToString()
        {
            return _contextNode.ToString();
        }
        public void AppendAction(IActionWrapper action)
        {
            _pendingActions.AddLast(action);
        }

        public void PrependAction(IActionWrapper action)
        {
            _pendingActions.AddFirst(action);
        }

        public string Status
        {
            get { return _statusNode.Value; }
        }

        public string ErrorMessage
        {
            get { return _errorMessageNode.Value; }
        }
        public void LogStartTime()
        {
            _startTimeNode.Value = DateTime.Now.ToString();
        }

        public void LogEndTime()
        {
            _endTimeNode.Value = DateTime.Now.ToString();
        }

        public void AppendContextData(AbstractAction action, XElement responseData)
        {
            if (responseData != null)
            {
                var responseDataNode = new XElement(action.ActionCode, new XAttribute("Id", action.Id));
                
                foreach (var key in action.Parameters.Keys)
                {
                    responseDataNode.Add(new XAttribute(key, String.Format("{0}", action.Parameters[key])));
                }

                responseDataNode.Add(responseData);
                _dataNode.Add(responseDataNode);
            }
        }

        public XElement GetContextData()
        {
            return XElement.Parse(_dataNode.ToString());
        }

        public XElement GetActionDataById(string actionId)
        {
            var actionDataNode = _dataNode.Elements().Where(x => x.Attribute("Id").Value == actionId).FirstOrDefault();
            if (actionDataNode == null)
            {
                return null;
            }
            else
            {
                return XElement.Parse(actionDataNode.ToString());
            }
        }
        public void LogAction(IActionWrapper actionWrapper)
        {
            foreach (var action in actionWrapper.GetWrappedActions())
            {
                var actionNode = new XElement(ns + "Action", new XAttribute("Id", action.Id));
                actionNode.Add(new XElement(ns + "Name", action.Name));
                actionNode.Add(new XElement(ns + "ActionCode", action.ActionCode));
                actionNode.Add(new XElement(ns + "ActionProcessed", action.ActionProcessed.ToString()));
                actionNode.Add(new XElement(ns + "ActionSkip", action.ActionSkip.ToString()));
                actionNode.Add(new XElement(ns + "ActionError", action.ActionError.ToString()));
                actionNode.Add(new XElement(ns + "ErrorMessage", action.ErrorMessage));
                actionNode.Add(new XElement(ns + "ActionStartTS", action.ActionStartTS.ToString()));
                actionNode.Add(new XElement(ns + "ActionEndTS", action.ActionEndTS.ToString()));

                if (action.Parameters != null)
                {
                    var parametersNode = new XElement(ns + "Parameters");
                    foreach (var key in action.Parameters.Keys)
                    {
                        var parameterNode = new XElement(ns + "Parameter",
                            new XAttribute("Name", key),
                            new XAttribute("Value", String.Format("{0}", action.Parameters[key])));

                        parametersNode.Add(parameterNode);
                    }
                    actionNode.Add(parametersNode);
                }

                _actionLogCount++;
                _countNode.Value = _actionLogCount.ToString();
                _actionsNode.Add(actionNode);
            }

        }
        public async Task ExecuteActions()
        {
            _statusNode.Value = "COMPLETE";
            while (_pendingActions.Count > 0)
            {
                IActionWrapper nextAction = _pendingActions.First.Value;
                _pendingActions.RemoveFirst();

                await nextAction.Execute(_serviceProvider, this);
                LogAction(nextAction);

                if (nextAction.ActionError)
                {
                    _errorMessageNode.Value = nextAction.ErrorMessage;
                    _statusNode.Value = "ERROR";
                    break;
                }
            }
        }
    }
}
