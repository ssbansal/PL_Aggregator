using System;
using System.Collections.Generic;
using System.Text;

namespace NBS.CRE.PolicyChecker.Services
{
    public interface ILookupService
    {
        bool CustomerExists(string firstName, string lastName, DateTime dob, string postcode);
    }
}
