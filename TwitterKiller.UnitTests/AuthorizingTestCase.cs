using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using RestSharp;
using RestSharp.Serializers;
using Twitter_Killer;
using Xunit;

namespace TwitterKiller.UnitTests
{
    public class AuthorizingTestCase
    {
        public static readonly string TestingLogin = "CorrectLogin";
        public static readonly string TestingPassword = "CorrectPassword";
        public static readonly string TestingToken = "token";

        private readonly LoginViewModel _viewModel;

        public AuthorizingTestCase()
        {
            _viewModel = new LoginViewModel(
                    new Authenticator(
                        new RestClientMock(), 
                        new JsonMock(), 
                        new RestRequestDummy()))
            {
                Login = TestingLogin,
                Password = TestingPassword
            };
        }

        [Fact]
        public void TestMethod_AlwaysSuccessfulLogin_ProperLoginAndPassword()
        {
            _viewModel.LoginCommand.Execute(null);
            Assert.NotNull(_viewModel.User);
            const string expectedError = "";
            Assert.Equal(expectedError, _viewModel.ErrorMessage);
            Assert.Equal(TestingToken, _viewModel.User.SessionToken);
        }

        [Fact]
        public void TestMethod_WrongPassword_AlwaysUnsuccessfulLogin()
        {
            _viewModel.Password = "WrongPassword";
            _viewModel.LoginCommand.Execute(null);
            Assert.Null(_viewModel.User);
            const string expectedError = "Unable to log in";
            Assert.Equal(expectedError, _viewModel.ErrorMessage);
        }

        [Fact]
        public void TestMethod_WrongLogin_AlwaysUnsuccessfulLogin()
        {
            _viewModel.Login = "NotExistingLogin";
            _viewModel.LoginCommand.Execute(null);
            Assert.Null(_viewModel.User);
            const string expectedError = "Unable to log in";
            Assert.Equal(expectedError, _viewModel.ErrorMessage);
        }
    }

    internal class RestRequestDummy : IRestRequestWrapper
    {
        public IRestRequest GetRequest(string source, Method method)
        {
            if (method != Method.POST) throw new ArgumentException("Expected to see Method.POST");
            return new RestRequestMock(source); // expected to test only POST method
        }
    }

    internal class RestRequestMock : IRestRequest
    {
        public RestRequestMock(string source)
        {
            Resource = source;
            Parameters = new List<Parameter>();
        }

        public IRestRequest AddHeader(string name, string value)
        {
            return AddParameter(name, value, ParameterType.HttpHeader); // implementation from the original code
        }

        public IRestRequest AddParameter(string name, object value, ParameterType type)
        {
            var par = new Parameter { Name = name, Value = value, Type = type };
            Parameters.Add(par);
            return this;
        }

        #region unused

        public Action<IRestResponse> OnBeforeDeserialization { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public int Attempts { get; private set; }
        public int Timeout { get; set; }
        public ICredentials Credentials { get; set; }
        public string XmlNamespace { get; set; }
        public string DateFormat { get; set; }
        public string RootElement { get; set; }
        public DataFormat RequestFormat { get; set; }
        public string Resource { get; set; }
        public Method Method { get; set; }
        public List<FileParameter> Files { get; private set; }
        public List<Parameter> Parameters { get; private set; }
        public Action<Stream> ResponseWriter { get; set; }
        public ISerializer XmlSerializer { get; set; }
        public ISerializer JsonSerializer { get; set; }
        public bool AlwaysMultipartFormData { get; set; }
        public void IncreaseNumAttempts()
        {
            throw new NotImplementedException();
        }

        public IRestRequest AddUrlSegment(string name, string value)
        {
            throw new NotImplementedException();
        }

        public IRestRequest AddCookie(string name, string value)
        {
            throw new NotImplementedException();
        }

        public IRestRequest AddParameter(string name, object value)
        {
            throw new NotImplementedException();
        }

        public IRestRequest AddParameter(Parameter p)
        {
            throw new NotImplementedException();
        }

        public IRestRequest AddObject(object obj)
        {
            throw new NotImplementedException();
        }

        public IRestRequest AddObject(object obj, params string[] whitelist)
        {
            throw new NotImplementedException();
        }

        public IRestRequest AddBody(object obj)
        {
            throw new NotImplementedException();
        }

        public IRestRequest AddBody(object obj, string xmlNamespace)
        {
            throw new NotImplementedException();
        }

        public IRestRequest AddFile(string name, byte[] bytes, string fileName, string contentType)
        {
            throw new NotImplementedException();
        }

        public IRestRequest AddFile(string name, byte[] bytes, string fileName)
        {
            throw new NotImplementedException();
        }

        public IRestRequest AddFile(string name, string path)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    internal class JsonMock : IJsonWrapper
    {
        public string GetValueByKey(string getContent, string key)
        {
            return getContent;
        }
    }

    internal class RestClientMock : RestClient
    {
        public override IRestResponse Execute(IRestRequest request)
        {
            return new RestResponseMock(request);
        }
    }

    internal class RestResponseMock : RestResponse
    {
        public RestResponseMock(IRestRequest request)
        {
            if (request == null) throw new ArgumentNullException("request");
            
            if (request.Parameters.Count != 2)
                throw new ArgumentException("Request parameters count not equals to 2");

            var login = (string)request.Parameters[0].Value;
            var password = (string) request.Parameters[1].Value;

            if (login == AuthorizingTestCase.TestingLogin && password == AuthorizingTestCase.TestingPassword)
            {
                StatusCode = HttpStatusCode.OK;
                Content = AuthorizingTestCase.TestingToken;
            }
            else
            {
                StatusCode = HttpStatusCode.Unauthorized;
            }
        }


    }
}
