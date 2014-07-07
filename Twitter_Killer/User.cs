namespace Twitter_Killer
{
    class User : IUser
    {
        public User(string login, string sessionToken)
        {
            Login = login;
            SessionToken = sessionToken;
        }

        public string Login { get; set; }
        public string SessionToken { get; set; }
    }
}
