using NBS.CRE.Common.Messaging;
using NBS.CRE.Common.Worker;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.Globalization;
using System.Threading;

namespace NBS.CRE.SoftQuote
{
    class SoftQuoteWorker : AbstractWorker
    {
        public SoftQuoteWorker(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override string ActionCode => "GET_SOFT_QUOTE";

        protected override XElement Process(RequestMessage request)
        {
            Console.WriteLine(String.Format("[{0}] Received Message", DateTime.Now));

            var result = new XElement("SoftQuoteDecision");
            var decision = new XElement("Decision");
            var decisionId = new XElement("DecisionId");
            var score = new XElement("Score");

            result.Add(decision, decisionId, score);

            DateTime dob = DateTime.ParseExact(request.Data.Elements("DOB").First().Value,"dd/MM/yyyy", CultureInfo.InvariantCulture);
            var ageDays = (int)TimeSpan.FromTicks(DateTime.Now.Ticks - dob.Ticks).Days / 365;

            if (ageDays >= 25 && ageDays <= 50)
            {
                decision.Value = "ACCEPT";
                decisionId.Value = Guid.NewGuid().ToString("N");
                score.SetValue(100 - (ageDays - 25) * 2);
            }
            else
            {
                decision.Value = "DECLINE";
                decisionId.Value = "";
                score.SetValue(0);
            }

            // Add 1 second delay
            Thread.Sleep(1000);

            return result;
        }
    }
}
