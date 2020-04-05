using NBS.CRE.Common.Models;
using NBS.CRE.Orchestrator.Actions;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Globalization;

namespace NBS.CRE.Orchestrator.Mappers
{
    public class PLAggregatorDecisionMapper : AbstractMapper<PLAggregatorDecisionRequest, PLAggregatorDecisionResponse>
    {
        private AbstractAction policyCheckerAction;
        private AbstractAction getSoftQuoteAction;
        public override void OnMapRequest(PLAggregatorDecisionRequest request, ISchedulerContext context)
        {
            policyCheckerAction = (new PolicyCheckerAction())
                .WithParameters((schedulerContext) =>
                {
                    var parameters = new Dictionary<string, object>();

                    parameters.Add("FIRST_NAME", request.FirstName);
                    parameters.Add("LAST_NAME", request.LastName);
                    parameters.Add("DOB", request.DOB.Date);
                    parameters.Add("POSTCODE", request.Postcode);

                    return parameters;
                });

            getSoftQuoteAction = (new GetSoftQuoteAction())
                .WithMessageData((schedulerContext) =>
                {
                    XElement data = new XElement("ApplicantData");
                    data.Add(new XElement("FIRST_NAME", request.FirstName));
                    data.Add(new XElement("LAST_NAME", request.LastName));
                    data.Add(new XElement("DOB", request.DOB.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)));
                    data.Add(new XElement("POSTCODE", request.Postcode));

                    return data;
                })
                .WithSkipCondition((schedulerContext) =>
                {
                    var policyCheckerResult = schedulerContext.GetActionDataById(policyCheckerAction.Id);
                    var policyRules = policyCheckerResult.Descendants("PolicyRules").FirstOrDefault();

                    if (policyRules != null && policyRules.Attribute("Status").Value == "PASSED")
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                });

            context.AppendAction(policyCheckerAction);
            context.AppendAction(getSoftQuoteAction);
        }

        public override PLAggregatorDecisionResponse OnMapResponse(ISchedulerContext context)
        {
            var result = new PLAggregatorDecisionResponse();
            if(!getSoftQuoteAction.ActionError && !getSoftQuoteAction.ActionSkip)
            {
                var actionData = context.GetActionDataById(getSoftQuoteAction.Id);
                result.Decision = actionData.Descendants("Decision").First().Value;
                if (result.Decision != "DECLINE")
                {
                    result.DecisionId = actionData.Descendants("DecisionId").First().Value;
                    result.Score = int.Parse(actionData.Descendants("Score").First().Value);
                }
            }
            else
            {
                result.Decision = "DECLINE";
                result.DecisionId = "";
                result.Score = 0;
            }

            return result;
        }
    }
}
