using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NBS.CRE.Orchestrator.Actions
{
    public class ParallelActionsGroup : IActionWrapper
    {
        private List<AbstractAction> _actions = new List<AbstractAction>();

        public ParallelActionsGroup()
        {
            ActionError = false;
            ErrorMessage = "";
        }
        public bool ActionError { get; private set; }

        public string ErrorMessage { get; private set; }

        public void AddAction(AbstractAction action)
        {
            if(action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var exists = _actions.Find(a => a.Id == action.Id);
            if(exists != null)
            {
                throw new InvalidOperationException("Action already added to the group.");
            }

            _actions.Add(action);
            
        }
        public async Task Execute(IServiceProvider serviceProvider, ISchedulerContext context)
        {
            List<Task> actionTasks = new List<Task>();
            foreach (var action in _actions)
            {
                actionTasks.Add(action.Execute(serviceProvider, context)); 
            }

            await Task.WhenAll(actionTasks);

            if (_actions.Any(a => a.ActionError))
            {
                ActionError = true;
                StringBuilder sb = new StringBuilder();
                
                foreach(var errorMessage in _actions.Where(a => a.ActionError).Select(a => a.ErrorMessage))
                {
                    sb.AppendLine(errorMessage);
                }

                ErrorMessage = sb.ToString();
            }
        }

        public IReadOnlyCollection<AbstractAction> GetWrappedActions()
        {
            return _actions.AsReadOnly();
        }
    }
}
