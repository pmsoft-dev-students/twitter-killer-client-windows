using System.Security.Authentication;
using GalaSoft.MvvmLight.Messaging;
using NSubstitute;
using Xunit;

namespace TwitterKiller.UnitTests
{
    public class LoginViewModelTestCase
    {
        private static LoginViewModel CreateSystemUnderTest(
            IUserManager userManager = null, 
            IMessenger messenger = null,
            TypeOfView type = TypeOfView.Login)
        {
            return new LoginViewModel(
                userManager ?? Substitute.For<IUserManager>(), 
                messenger ?? Substitute.For<IMessenger>(),
                type);
        }

        [Fact]
        public void LoginCommand_WhenLoginIsSuccessfull_ShouldReturnSession()
        {
            var userManagerStub = Substitute.For<IUserManager>();
            var expected = new Session("foo", "01.01.1970");
            userManagerStub.Login(Arg.Any<string>(), Arg.Any<string>()).Returns(expected);
            var sut = CreateSystemUnderTest(userManager: userManagerStub);
            sut.Login = "foo";

            sut.ActionCommand.Execute(null);

            Assert.Equal(expected, sut.Session);
        }

        [Fact]
        public void LoginCommand_WhenLoginIsUnsuccessfull_ShouldFillErrorMessage()
        {
            const string expected = "foo";
            var authenticationStub = Substitute.For<IUserManager>();
            authenticationStub.When(a => a.Login(Arg.Any<string>(), Arg.Any<string>()))
                .Do(ci => { throw new AuthenticationException(expected); });
            var sut = CreateSystemUnderTest(userManager: authenticationStub);

            sut.ActionCommand.Execute(null);

            Assert.Equal(expected, sut.ErrorMessage);
        }
    }
}