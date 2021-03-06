﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NBS.CRE.Orchestrator.Actions
{
    /// <summary>
    /// Interface used by the <see cref="Scheduler"/> to build the execution logic.
    /// </summary>
    public interface IActionWrapper
    {
        bool ActionError { get; }
        string ErrorMessage { get; }
        IReadOnlyCollection<AbstractAction> GetWrappedActions();
        Task Execute(IServiceProvider serviceProvider, ISchedulerContext context);
    }
}
