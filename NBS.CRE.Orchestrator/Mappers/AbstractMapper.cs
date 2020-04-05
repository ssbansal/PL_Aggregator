using System;
using System.Threading.Tasks;

namespace NBS.CRE.Orchestrator.Mappers
{
    public abstract class AbstractMapper<REQUEST, RESPONSE>
    {
        public AbstractMapper()
        {
        }
        public abstract void OnMapRequest(REQUEST request, ISchedulerContext context);
        public abstract RESPONSE OnMapResponse(ISchedulerContext context);
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
