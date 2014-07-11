using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using RestSharp;

namespace TwitterKiller
{
    public class UserManager : IUserManager
    {
        private readonly IRestClient _restClient;
        private readonly ISerializer _jsonSerializer;

        public UserManager(IRestClient restClient, ISerializer jsonSerializer)
        {
            if (restClient == null)
                throw new ArgumentNullException("restClient");
            if (jsonSerializer == null)
                throw new ArgumentNullException("jsonSerializer");

            _restClient = restClient;
            _jsonSerializer = jsonSerializer;
        }

        public Session Login(string login, string password)
        {
            if (login == null)
                throw new ArgumentNullException("login");
            if (password == null)
                throw new ArgumentNullException("password");

            var response = _restClient.Execute(Request("/user/login", new[] { "login", "password" },
                new[] { login, password }, Method.POST));

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    return _jsonSerializer.Deserialize<Session>(response.Content);
                case HttpStatusCode.Unauthorized:
                    throw new AuthenticationException("Wrong username or password");
                case HttpStatusCode.BadRequest:
                    throw new AuthenticationException("Bad request server response");
                default:
                    throw new AuthenticationException("Server error. Retry in a few seconds");
            }
        }

        public void Register(string login, string password)
        {
            if (login == null)
                throw new ArgumentNullException("login");
            if (password == null)
                throw new ArgumentNullException("password");

            var response = _restClient.Execute(Request("/user/register", new[] { "login", "password" },
                new[] { login, password }, Method.POST));

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    return;
                case HttpStatusCode.Unauthorized:
                    throw new AuthenticationException("Login already exists");
                case HttpStatusCode.BadRequest:
                    throw new AuthenticationException("Bad request server response");
                default:
                    throw new AuthenticationException("Server error. Retry in a few seconds");
            }
        }

        public void SendTweet(Session session, string tweetText)
        {
            if (session == null)
                throw new ArgumentNullException("session");
            if (tweetText == null)
                throw new ArgumentNullException("tweetText");

            if (tweetText.Length == 0 || tweetText.Length > 140) 
                throw new ArgumentException("Wrong size of the tweet");

            var request = Request("/tweet/add", new[] {"token"}, new[] {session.Token}, Method.POST).
                AddParameter("text/plain", tweetText, ParameterType.RequestBody);
            var response = _restClient.Execute(request);

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    break;
                case HttpStatusCode.Unauthorized:
                    throw new AuthenticationException("You are not authorized");
                case HttpStatusCode.BadRequest:
                    throw new AuthenticationException("Bad request from server");
                default:
                    throw new AuthenticationException("Server error. Retry in a few seconds");
            }
        }

        private static IRestRequest Request(string source, IList<string> names, IList<string> values, Method method)
        {
            if (names.Count != values.Count)
                throw new ArgumentException("names.Length != values.Length");

            var request = new RestRequest(source, method);
            for (var i = 0; i < names.Count; i++)
                request.AddHeader(names[i], values[i]);

            return request;
        }

        public IEnumerable<Tweet> GetTweets(Session session)
        {
            if (session == null) 
                throw new ArgumentNullException("session");

            var response = _restClient.Execute(Request("/tweet/user/" + session.User.Login, new[] {"token"}, new[] {session.Token}, Method.GET));
            var tweetTypeLoader = new { tweets = new Tweet[] {} };

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    return _jsonSerializer.DeserializeAnonymous(response.Content, tweetTypeLoader).tweets.Reverse();
                case HttpStatusCode.Unauthorized:
                    throw new AuthenticationException("You are not authorized");
                default:
                    throw new AuthenticationException("Server error. Retry in a few seconds");
            }
        }
    }
}
