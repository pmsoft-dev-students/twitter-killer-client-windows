using System;
using System.Net;
using System.Security.Authentication;
using RestSharp;

namespace Twitter_Killer
{
    public class Authenticator : IAuthenticator
    {
        /// <summary>
        /// Names of the errors, occured
        /// </summary>
        public static readonly string ErrorBadRequest = "Bad request server response";      // 400
        public static readonly string ErrorUnauthorized = "Unable to log in";               // 401 
        public static readonly string ErrorUnexpected = "Unexpected error occurred";        // else

        private IRestClient RestClient { get; set; } 
        private IJsonWrapper JsonWrapper { get; set; }
        private IRestRequestWrapper RequestWrapper { get; set; }

        public Authenticator(IRestClient restClient, IJsonWrapper jsonWrapper, IRestRequestWrapper requestWrapper)
        {
            if (restClient == null) throw new ArgumentNullException("restClient");
            if (jsonWrapper == null) throw new ArgumentNullException("jsonWrapper");
            if (requestWrapper == null) throw new ArgumentNullException("requestWrapper");

            RestClient = restClient;
            JsonWrapper = jsonWrapper;
            RequestWrapper = requestWrapper;
        }

        public IUser Login(string login, string password)
        {
            var request = RequestWrapper.GetRequest("/user/login", Method.POST);
            request.AddHeader("login", login).AddHeader("password", password);
            var response = RestClient.Execute(request);
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    var token = JsonWrapper.GetValueByKey(response.Content, "token");
                    return new User(login, token);
                    break;
                case HttpStatusCode.Unauthorized:
                    throw new AuthenticationException(ErrorUnauthorized);
                    break;
                case HttpStatusCode.BadRequest:
                    throw new AuthenticationException(ErrorBadRequest);
                    break;
                default:
                    throw new AuthenticationException(ErrorUnexpected);
                    break;
            }
        }
    }
}
