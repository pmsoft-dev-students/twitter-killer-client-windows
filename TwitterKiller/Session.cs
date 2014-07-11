using System;

namespace TwitterKiller
{
    public class Session
    {
        private readonly string _token;
        private readonly string _expiration;

        public Session(string token, string expiration)
        {
            if (token == null) 
                throw new ArgumentNullException("token");
            if (expiration == null) 
                throw new ArgumentNullException("expiration");

            _token = token;
            _expiration = expiration;
        }

        public string Token
        {
            get { return _token; }
        }

        public string Expiration
        {
            get { return _expiration; }
        }

        public User User { get; set; }
    }
}
