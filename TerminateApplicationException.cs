using System;

namespace PostgresToCouchDB
{
    public class TerminateApplicationException : ApplicationException
    {
        public TerminateApplicationException(string message) :
            base(message)
        {
        }
    }
}
