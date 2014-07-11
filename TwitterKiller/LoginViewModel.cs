using System;
using System.Security.Authentication;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace TwitterKiller
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IUserManager _userManager;
        private readonly IMessenger _messenger;

        private string _login;
        private string _password;
        private string _errorMessage = "";
        private string _buttonContent;
        private string _headerViewContent;

        public LoginViewModel(IUserManager userManager, IMessenger messenger, TypeOfView type)
        {
            if (userManager == null)
                throw new ArgumentNullException("userManager");
            if (messenger == null)
                throw new ArgumentNullException("messenger");

            _userManager = userManager;
            _messenger = messenger;

            switch (type)
            {
                case TypeOfView.Login:
                    ActionCommand = new RelayCommand(OnLogin);
                    ButtonContent = "Login";
                    HeaderViewContent = "Enter your login and password to log in";
                    break;
                case TypeOfView.Register:
                    ActionCommand = new RelayCommand(OnRegister);
                    ButtonContent = "Register";
                    HeaderViewContent = "Enter your login and password to register";
                    break;
            }
        }

        public ICommand ActionCommand { get; private set; }

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

        public Session Session { get; set; }

        public string ButtonContent
        {
            get { return _buttonContent; }
            private set { Set(ref _buttonContent, value); }
        }

        public string HeaderViewContent
        {
            get { return _headerViewContent; }
            private set { Set(ref _headerViewContent, value); }
        }

        private void OnLogin()
        {
            try
            {
                Session = _userManager.Login(_login, _password);
                Session.User = new User(_login);

                _messenger.Send((object)Session);
            }
            catch (AuthenticationException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (ArgumentNullException)
            {
                ErrorMessage = "Incorrect login password pair";
            }
        }

        private void OnRegister()
        {
            try
            {
                _userManager.Register(_login, _password);
                _messenger.Send(new object());
            }
            catch (AuthenticationException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (ArgumentNullException)
            {
                ErrorMessage = "Arguments must be non-null strings";
            }
        }
    }
}
