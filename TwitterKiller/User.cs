using System;

namespace TwitterKiller
{
    public class User
    {
        private readonly string _login;

        public User(string login)
        {
            if (login == null) 
                throw new ArgumentNullException("login");

            _login = login;
        }

        public string Login
        {
            get { return _login; }
        }
    }
}
