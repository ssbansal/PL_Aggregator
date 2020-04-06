using System;
using System.Threading.Tasks;

namespace NBS.CRE.Orchestrator.Mappers
{
    /// <summary>
    /// Abstract base class for Request-Response Mappers.
    /// </summary>
    /// <typeparam name="REQUEST">Request model.</typeparam>
    /// <typeparam name="RESPONSE">Response model.</typeparam>
    public abstract class AbstractMapper<REQUEST, RESPONSE>
    {
        public AbstractMapper()
        {
        }

        /// <summary>
        /// Called before the <see cref="Execute(REQUEST, ISchedulerContext)"/> method. Use this method to setup the scheduler context.
        /// </summary>
        /// <param name="request">Instance of the request object.</param>
        /// <param name="context">Scheduler execution context.</param>
        public abstract void OnMapRequest(REQUEST request, ISchedulerContext context);

        /// <summary>
        /// Called after the <see cref="Execute(REQUEST, ISchedulerContext)"/> method. Use this method to generate the response object.
        /// </summary>
        /// <param name="context">Scheduler execution context.</param>
        /// <returns>Generated response object.</returns>
        public abstract RESPONSE OnMapResponse(ISchedulerContext context);

        /// <summary>
        /// Executes the actions configured in the scheduler context.
        /// </summary>
        /// <param name="request">Instance of the request object.</param>
        /// <param name="context">Scheduler execution context.</param>
        /// <returns><c>Task</c> object that completes when all the actions in the scheduler context finishes.</returns>
        public async Task<RESPONSE> Execute(REQUEST request, ISchedulerContext context)
        {
            context.LogStartTime();

            OnMapRequest(request, context);
            await context.ExecuteActions();
            context.LogEndTime();

            if (context.Status == "ERROR")
            {
                throw new Exception(context.ErrorMessage);
            }
            else
            {
                RESPONSE response = OnMapResponse(context);
                return response;
            }
        }
    }
}
