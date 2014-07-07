using System;
using Microsoft.CSharp.RuntimeBinder;
using Twitter_Killer;
using Xunit;

namespace TwitterKiller.UnitTests
{
    public class JsonWrapperTestCase
    {
        private readonly JsonWrapper _jsonWrapper = new JsonWrapper();

        const string Expected = "unique_token";
        const string Key = "token";
        string _jsonContent = "{\"token\":\"unique_token\"}";

        [Fact]
        public void GetValue_WithRightJsonContent_ShouldReturnTokenValue()
        {
            Assert.Equal(Expected, _jsonWrapper.GetValueByKey(_jsonContent, Key));
        }

        [Fact]
        public void GetValue_WithNonFirstTokenField_ShouldReturnTokenValue()
        {
            _jsonContent = "{\"expires\":\"date\",\"token\":\"unique_token\"}";

            Assert.Equal(Expected, _jsonWrapper.GetValueByKey(_jsonContent, Key));
        }

        [Fact]
        public void GetValue_WithEmptyContent_ShouldThrowRuntimeBinderException()
        {
            Assert.Throws<RuntimeBinderException>(() => _jsonWrapper.GetValueByKey(string.Empty, Key));
        }

        [Fact]
        public void GetValue_WithMoreThanOneTokenField_ShouldThrowArgumentException()
        {
            _jsonContent = "{\"token\":\"first_token\",\"token\":\"second_token\"}";

            Assert.Throws<ArgumentException>(() => _jsonWrapper.GetValueByKey(_jsonContent, Key));
        }
    }
}
