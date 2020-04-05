using NBS.CRE.Common.Messaging;
using NBS.CRE.Common.Worker;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.Globalization;
using NBS.CRE.PolicyChecker.Services;

namespace NBS.CRE.PolicyChecker
{
    class PolicyCheckerWorker : AbstractWorker
    {
        public PolicyCheckerWorker(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override string ActionCode => "POLICY_CHECKER";

        protected override XElement Process(RequestMessage request)
        {
            Console.WriteLine(String.Format("[{0}] Received Message", DateTime.Now));

            // Read Message Parameters
            string firstName = (string)request.Parameters.Where(x => x.Name == "FIRST_NAME").First().Value;
            string lastName = (string)request.Parameters.Where(x => x.Name == "LAST_NAME").First().Value;
            string postcode = (string)request.Parameters.Where(x => x.Name == "POSTCODE").First().Value;
            DateTime dob = (DateTime)request.Parameters.Where(x => x.Name == "DOB").First().Value;

            // Perform Lookup
            ILookupService service = ServiceProvider.GetService(typeof(ILookupService)) as ILookupService;
            if(service == null)
            {
                throw new InvalidOperationException("Lookup Service not configured!");
            }

            var match = service.CustomerExists(firstName, lastName, dob, postcode);

            return new XElement("PolicyRules", new XAttribute("Status", match ? "PASSED" : "FAILED"));
        }
    }
}
