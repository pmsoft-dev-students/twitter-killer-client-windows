using System;

namespace Twitter_Killer
{
    class AuthorizingException : Exception
    {
        public AuthorizingException(string reason) : base(reason) { }
    }
}
