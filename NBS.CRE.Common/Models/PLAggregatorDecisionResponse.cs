using System;
using System.Collections.Generic;
using System.Text;

namespace NBS.CRE.Common.Models
{
    public class PLAggregatorDecisionResponse
    {
        public string Decision { get; set; }
        public int Score { get; set; }
        public string DecisionId { get; set; }
    }
}
