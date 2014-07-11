using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Authentication;
using NSubstitute;
using RestSharp;
using Xunit;
using Xunit.Extensions;

namespace TwitterKiller.UnitTests
{
    public class UserManagerTestCase
    {
        public static UserManager CreateSystemUnderTest(
            IRestClient restClient = null, 
            ISerializer serializer = null)
        {
            return new UserManager(
                restClient ?? Substitute.For<IRestClient>(),
                serializer ?? Substitute.For<ISerializer>());
        }


        private static IRestClient CreateRestClientStubWithResponse(HttpStatusCode code)
        {
            var responseStub = Substitute.For<IRestResponse>();
            responseStub.StatusCode.Returns(code);

            var restClientStub = Substitute.For<IRestClient>();
            restClientStub.Execute(Arg.Any<IRestRequest>()).Returns(responseStub);

            return restClientStub;
        }

        [Fact]
        public void Login_WhenServerConnectionSuccessful_ShouldReturnSession()
        {
            // Fixture setup
            const string expectedLogin = "login";
            const string expectedPassword = "password";

            var expected = new Session("foo", "bar") {User = new User(expectedLogin)};

            var serializerStub = Substitute.For<ISerializer>();
            serializerStub.Deserialize<Session>(Arg.Any<string>()).Returns(expected);

            var restClientStub = CreateRestClientStubWithResponse(HttpStatusCode.OK);

            var sut = CreateSystemUnderTest(restClientStub, serializerStub);

            // Excercise
            var actual = sut.Login(expectedLogin, expectedPassword);

            // Verify
            Assert.Equal(expected.Token, actual.Token);
            Assert.Equal(expected.Expiration, actual.Expiration);
            Assert.Equal(expectedLogin, actual.User.Login);
        }

        public static IEnumerable<object[]> NullStringTestData
        {
            get
            {
                yield return new object[] {null, ""}; 
                yield return new object[] {"", null};
            }
        }

        [Theory, PropertyData("NullStringTestData")]
        public void Login_WhenArgumentsAreNull_ShouldThrowArgumentNullException(string first, string second)
        {
            var sut = CreateSystemUnderTest();

            Assert.Throws<ArgumentNullException>(() => sut.Login(first, second));
        }

        public static IEnumerable<object[]> StatusCodeData
        {
            get
            {
                yield return new object[] {HttpStatusCode.Unauthorized}; 
                yield return new object[] {HttpStatusCode.BadRequest};
            }    
        }

        [Theory, PropertyData("StatusCodeData")]
        public void Login_WhenAuthenticationFails_ShouldThrowAuthenticationException(HttpStatusCode statusCode)
        {
            var restClientStub = CreateRestClientStubWithResponse(statusCode);
            var sut = CreateSystemUnderTest(restClientStub);
            Assert.Throws<AuthenticationException>(() => sut.Login("", ""));
        }

        [Fact]
        public void Register_WhenServerConnectionSuccessful_ShouldSendRequestToServerAndSuccessfullyRegister()
        {
            var restClientMock = CreateRestClientStubWithResponse(HttpStatusCode.OK);
            var sut = CreateSystemUnderTest(restClientMock);
         
            sut.Register("foo", "bar");

            restClientMock.Received(1).Execute(Arg.Any<IRestRequest>());
        }

        [Theory, PropertyData("NullStringTestData")]
        public void Register_WhenArgumentsAreNull_ShouldThrowArgumentNullException(string first, string second)
        {
            var sut = CreateSystemUnderTest();

            Assert.Throws<ArgumentNullException>(() => sut.Register(first, second));
        }

        [Theory,
        PropertyData("StatusCodeData")]
        public void Register_WhenAuthenticationFails_ShouldThrowAuthenticatorException(HttpStatusCode statusCode)
        {
            var restClientStub = CreateRestClientStubWithResponse(statusCode);
            var sut = CreateSystemUnderTest(restClientStub);
            Assert.Throws<AuthenticationException>(() => sut.Register("", ""));
        }

        [Fact]
        public void SendTweet_WhenServerConnectionSuccessful_ShouldSendRequestToServerAndSuccessfullyPostTweet()
        {
            var restClientMock = CreateRestClientStubWithResponse(HttpStatusCode.OK);
            var sut = CreateSystemUnderTest(restClientMock);

            sut.SendTweet(new Session("foo", "bar"), "foo");

            restClientMock.Received(1).Execute(Arg.Any<IRestRequest>());
        }

        public static IEnumerable<object[]> NullSessionStringTestData
        {
            get
            {
                yield return new object[] {null, ""};
                yield return new object[] {new Session("", ""), null};
            }
        }

        [Theory,
        PropertyData("NullSessionStringTestData")]
        public void SendTweet_WhenArgumentsAreNull_ShouldThrowArgumentNullException(Session first, string second)
        {
            var sut = CreateSystemUnderTest();

            Assert.Throws<ArgumentNullException>(() => sut.SendTweet(first, second));
        }

        [Theory,
        PropertyData("StatusCodeData")]
        public void SendTweet_WhenAuthenticationFails_ShouldThrowAuthenticatorException(HttpStatusCode statusCode)
        {
            var restClientStub = CreateRestClientStubWithResponse(statusCode);
            var sut = CreateSystemUnderTest(restClientStub);
            Assert.Throws<AuthenticationException>(() => sut.SendTweet(new Session("", ""), "foo"));
        }

        public static IEnumerable<object[]> TweetData
        {
            get
            {
                yield return new object[] {""};
                yield return new object[] {new string(' ', 141)};
            }
        }

        [Theory,
         PropertyData("TweetData")]
        public void SendTweet_Always_ShouldValidateTweet(string tweet)
        {
            var sut = CreateSystemUnderTest(CreateRestClientStubWithResponse(HttpStatusCode.OK));
            
            Assert.Throws<ArgumentException>(() => sut.SendTweet(new Session("", ""), tweet));
        }
    }
}
