using System.Security.Authentication;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Twitter_Killer
{
    public class LoginViewModel : ViewModelBase
    {
        private IAuthenticator Authenticator { get; set; }

        private string _login;
        private string _password;
        private string _errorMessage;

        public ICommand LoginCommand { get; private set; }

        public string Login
        {
            get { return _login; }
            set { Set(ref _login, value); }
        }

        public string Password
        {
            get { return _password; }
            set { Set(ref _password, value); }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { Set(ref _errorMessage, value); }
        }

        public IUser User { get; set; }

        public LoginViewModel(IAuthenticator authenticator)
        {
            Authenticator = authenticator;
            LoginCommand = new RelayCommand(OnLogin);
            ErrorMessage = "";
        }

        private void OnLogin()
        {
            try
            {
                User = Authenticator.Login(_login, _password);
            }
            catch (AuthenticationException ex)
            {
                ErrorMessage = ex.Message;
            }
        }
    }
}
