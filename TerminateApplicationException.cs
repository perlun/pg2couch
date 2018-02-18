using System;

namespace Pg2Couch
{
    public class TerminateApplicationException : ApplicationException
    {
        public TerminateApplicationException(string message) :
            base(message)
        {
        }
    }
}
