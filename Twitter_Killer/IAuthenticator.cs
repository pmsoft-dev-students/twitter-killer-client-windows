namespace Twitter_Killer
{
    public interface IAuthenticator
    {
        IUser Login(string login, string password);
    }
}
