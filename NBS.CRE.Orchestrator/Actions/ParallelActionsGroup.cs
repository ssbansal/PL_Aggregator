using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NBS.CRE.Orchestrator.Actions
{
    /// <summary>
    /// Executes a group of actions in parallel.
    /// </summary>
    public class ParallelActionsGroup : IActionWrapper
    {
        private List<AbstractAction> _actions = new List<AbstractAction>();

        public ParallelActionsGroup()
        {
            ActionError = false;
            ErrorMessage = "";
        }

        /// <value>Returns <c>true</c> if any of the actions fails to execute successfully.</value>
        public bool ActionError { get; private set; }

        /// <value>Returns consolidated error messages from all failed actions.</value>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// Adds an action to the group for parallel execution.
        /// </summary>
        /// <param name="action"><see cref="AbstractAction"/> to add to the group.</param>
        /// <exception cref="InvalidOperationException">Thrown when the action being added already exists in the group.</exception>
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

        /// <summary>
        /// Executes the actions in parallel.
        /// </summary>
        /// <param name="serviceProvider">Dependency injection container for resolving services.</param>
        /// <param name="context">Scheduler context for executing actions.</param>
        /// <returns>A single <c>Task</c> object that completes when all the action finish execution.</returns>
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

        /// <summary>
        /// Gets wrapped actions instances.
        /// </summary>
        /// <returns>Readonly collection of <see cref="AbstractAction"/> instances.</returns>
        public IReadOnlyCollection<AbstractAction> GetWrappedActions()
        {
            return _actions.AsReadOnly();
        }
    }
}
