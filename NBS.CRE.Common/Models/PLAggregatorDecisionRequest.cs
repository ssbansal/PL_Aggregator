using System;
using System.Collections.Generic;
using System.Text;

namespace NBS.CRE.Common.Models
{
    /// <summary>
    /// PL Aggregator Decision Request.
    /// </summary>
    public class PLAggregatorDecisionRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DOB { get; set; }
        public string Postcode { get; set; }
    }
}
