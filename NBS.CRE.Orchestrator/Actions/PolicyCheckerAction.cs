using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace NBS.CRE.Orchestrator.Actions
{
    /// <summary>
    /// PolicyCheckerAction
    /// </summary>
    public class PolicyCheckerAction: AbstractAction
    {
        public override string ActionCode => "POLICY_CHECKER";
        public override string Name => "Policy Checker";
        public override int SecondsTimeout => 15;
    }
}
