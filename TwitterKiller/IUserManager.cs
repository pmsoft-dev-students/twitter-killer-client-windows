using System.Collections.Generic;

namespace TwitterKiller
{
    public interface IUserManager
    {
        Session Login(string login, string password);
        void Register(string login, string password);
        void SendTweet(Session session, string tweetText);
        IEnumerable<Tweet> GetTweets(Session session);
    }
}
