using Microsoft.AspNetCore.Mvc;
using NBS.CRE.Common.Models;
using NBS.CRE.Orchestrator;
using System;
using System.Net;
using System.Threading.Tasks;

namespace NBS.CRE.APIGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PLAggregateDecisionController : ControllerBase
    {
        private IScheduler _scheduler;

        public PLAggregateDecisionController(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PLAggregatorDecisionRequest request)
        {
            try
            {
                return Ok(await _scheduler.Execute<PLAggregatorDecisionRequest, PLAggregatorDecisionResponse>(request));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
