using System;
using System.Collections.Generic;
using System.Security.Authentication;
using GalaSoft.MvvmLight.Messaging;
using NSubstitute;
using Xunit;
using Xunit.Extensions;

namespace TwitterKiller.UnitTests
{
    public class UserViewModelTestCase
    {
        public readonly int TweetLength = 140;

        public static UserViewModel CreateSystemUnderTest(
            IUserManager userManager = null, 
            IDialogService dialogService = null, 
            IMessenger messenger = null)
        {
            return new UserViewModel(
                userManager ?? Substitute.For<IUserManager>(),
                dialogService ?? Substitute.For<IDialogService>(),
                messenger ?? Substitute.For<IMessenger>());
        }

        [Fact]
        public void SendTweet_WhenTweetIsUnsuccessful_ShouldDoNothing()
        {
            const string expected = "foo";
            var userManagerStub = Substitute.For<IUserManager>();
            userManagerStub.When(a => a.SendTweet(Arg.Any<Session>(), Arg.Any<string>()))
                .Do(ci => { throw new AuthenticationException(expected); });
            var sut = CreateSystemUnderTest(userManager: userManagerStub);

            sut.SendTweet.Execute(null);

            Assert.Equal(expected, sut.ErrorMessage);
        }

        public static IEnumerable<object[]> TweetTextExamples
        {
            get
            {
                yield return new object[] { "First" };
                yield return new object[] { "Second" };
                yield return new object[] { "This is a tweet that is a little bit longer than the privous two" };
                yield return new object[] { new String('a', 150) };
            }
        }

        [Theory, PropertyData("TweetTextExamples")]
        public void ChangeCurrentTweet_WhenTweetTextIsChanged_ShouldChangesNumberOfCharsLeft(
            string tweet)
        {
            var expected = TweetLength - tweet.Length;
            var sut = CreateSystemUnderTest();
            sut.TweetText = tweet;
            Assert.Equal(expected, sut.CharsLeft);
        }

        [Theory, PropertyData("TweetTextExamples")]
        public void ChangeCurrentTweet_WhenTweetHasProperNumberOfCharacters_AlwaysChangesIsActiveSendTweetToTrue(
            string tweet)
        {
            var expected = TweetLength >= tweet.Length && tweet.Length > 0;
            var sut = CreateSystemUnderTest();
            sut.TweetText = tweet;
            Assert.Equal(expected, sut.IsEnabledSendTweet);
        }

        //[Fact]
        //public void GetAllTweets_Always_ChangesTweetList()
        //{
        //    var expected = new[] {new Tweet("foo", new DateTime(), 1, 1)};
        //    var userManagerStub = Substitute.For<IUserManager>();
        //    userManagerStub.GetTweets(Arg.Any<Session>()).Returns(expected);
        //    var sut = CreateSystemUnderTest(userManagerStub);
        //    sut.ShowAllTweets.Execute(null);
        //    Assert.Equal(expected, sut.TweetList);       
        //}

        //[Fact]
        //public void GetAllTweets_Always_ErrorMessageChangesIfExceptionIsOccurred()
        //{
        //    const string expected = "foo";
        //    var userManagerStub = Substitute.For<IUserManager>();
        //    userManagerStub.When(a => a.GetTweets(Arg.Any<Session>()))
        //        .Do(ci => { throw new AuthenticationException(expected);});
        //    var sut = CreateSystemUnderTest(userManagerStub);
        //    sut.ShowAllTweets.Execute(null);
        //    Assert.Equal(expected, sut.ErrorMessage);
        //}
    }
}
