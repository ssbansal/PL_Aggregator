using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace NBS.CRE.Orchestrator.Actions
{
    /// <summary>
    /// GetSoftQuoteAction
    /// </summary>
    public class GetSoftQuoteAction: AbstractAction
    {
        public override string ActionCode => "GET_SOFT_QUOTE";
        public override string Name => "Get Soft Quote";
        public override int SecondsTimeout => 15;
 
    }
}
