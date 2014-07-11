using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Xunit;
using Xunit.Extensions;

namespace TwitterKiller.UnitTests
{
    public class JsonNetSerializerTestCase
    {
        private static JsonNetSerializer CreateSystemUnderTest()
        {
            return new JsonNetSerializer();
        }

        public static IEnumerable<object[]> SerializationTestData
        {
            get
            {
                yield return new object[] {
                    new Session("foo", "bar"), 
                    JsonConvert.SerializeObject(new Session("foo", "bar"))}; // vanilla strings
            }
        }

        public static IEnumerable<object[]> DeserializationTestData
        {
            get
            {
                var session = new Session("foo", "bar");
                yield return new object[] {
                    JsonConvert.SerializeObject(session), 
                    session};
            }
        }

        [Theory, PropertyData("SerializationTestData")]
        public void Serialize_Always_ShouldReturnExpectedResult(object toSerialize, string expected)
        {
            var sut = CreateSystemUnderTest();
            var actual = sut.Serialize(toSerialize);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Deserialize_Always_ShouldReturnExpectedResult()
        {
            DeserializationTest(
                "{\"token\":\"foo\",\"expiration\":\"bar\"}", 
                new Session("foo", "bar"),
                new SessionEqualityComparer());
        }

        private static void DeserializationTest<T>(string toDeserialize, T expected, IEqualityComparer<T> comparer)
        {
            var sut = CreateSystemUnderTest();
            var actual = sut.Deserialize<T>(toDeserialize);
            Assert.Equal(expected, actual, comparer);
        }

        private class SessionEqualityComparer : IEqualityComparer<Session>
        {
            public bool Equals(Session x, Session y)
            {
                return x.Token == y.Token
                       && x.Expiration == y.Expiration
                       && x.User == y.User;
            }

            public int GetHashCode(Session obj)
            {
                return 0;
            }
        }

        public static IEnumerable<object[]> DeserializationAnonymousData
        {
            get
            {
                var tweets = new { tweets = new [] {new Tweet("This is a tweet text", new DateTime(), 1, 1) }};
                yield return new object[] {JsonConvert.SerializeObject(tweets), tweets.tweets};
            }
        }

        [Theory, PropertyData("DeserializationAnonymousData")]
        public void DeserializationAnonnymous_Always_ShouldDeserializeObjectsAsExpected(string toDeserialize, IEnumerable<Tweet> tweets)
        {
            var sut = CreateSystemUnderTest();
            var anonymousTweets = new {tweets = new Tweet[] {} };
            anonymousTweets = sut.DeserializeAnonymous(toDeserialize, anonymousTweets);
            Assert.True(tweets.SequenceEqual(anonymousTweets.tweets));
        }
    }
}
