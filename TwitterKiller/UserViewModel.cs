using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace TwitterKiller
{
    public class UserViewModel : ViewModelBase
    {
        public static readonly int TweetLength = 140;

        private readonly IUserManager _userManager;
        private readonly IDialogService _dialogService;
        
        private string _errorMessage;
        private string _tweetText;
        private int _charsLeft = TweetLength;
        private string _userName;
        private bool _isEnabledSendTweet;
        private IEnumerable<Tweet> _tweetList;
        private Session _session;

        public UserViewModel(IUserManager userManager, IDialogService dialogService, IMessenger messenger)
        {
            if (userManager == null)
                throw new ArgumentNullException("userManager");
            if (dialogService == null)
                throw new ArgumentNullException("dialogService");
            if (messenger == null)
                throw new ArgumentNullException("messenger");

            _userManager = userManager;
            _dialogService = dialogService;
            MessengerInstance = messenger;

            LoginButtonClickCommand = new RelayCommand(StartLoginView);
            RegisterButtonClickCommand = new RelayCommand(StartRegisterView);
            SendTweet = new RelayCommand(OnSendTweet);
        
            MessengerInstance.Register<object>(this, s =>
            {
                var session = s as Session;
                if (session != null) 
                    Session = session;
            });
        }

        public ICommand LoginButtonClickCommand { get; set; }

        public ICommand RegisterButtonClickCommand { get; set; }

        public ICommand SendTweet { get; set; }

        public string TweetText
        {
            get { return _tweetText; }
            set
            {
                Set(ref _tweetText, value);
                CharsLeft = TweetLength - _tweetText.Length;
            }
        }

        public Session Session
        {
            get { return _session; }
            set
            {
                _session = value;
                UserName = (value == null) ? null : "@" + value.User.Login;
                ErrorMessage = "";
            }
        }

        public string UserName
        {
            get { return _userName ?? "Unauthorized"; }
            private set { Set(ref _userName, value); }
        }

        public int CharsLeft
        {
            get { return _charsLeft; }
            private set
            {
                Set(ref _charsLeft, value);
                IsEnabledSendTweet = _charsLeft >= 0 && _charsLeft < 140;
            }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            private set
            {
                Set(ref _errorMessage, value);    
            }
        }

        public bool IsEnabledSendTweet
        {
            get { return _isEnabledSendTweet; }
            set { Set(ref _isEnabledSendTweet, value); }
        }

        private void StartLoginView()
        {
            _dialogService.ShowDialog(new LoginViewModel(_userManager, MessengerInstance, TypeOfView.Login), WindowMode.Modal);
            // show user tweets
            ShowTweets();
        }

        private void StartRegisterView()
        {
            _dialogService.ShowDialog(new LoginViewModel(_userManager, MessengerInstance, TypeOfView.Register), WindowMode.Modal);
        }

        private void OnSendTweet()
        {
            try
            {
                _userManager.SendTweet(Session, TweetText);
                // passed successfully, try to reload story
                ShowTweets();

                TweetText = "";
                ErrorMessage = "";
            }
            catch (AuthenticationException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (ArgumentNullException ex)
            {
                ErrorMessage = ex.ParamName == "session" ? "You are not authorized" : "Your have to send non-null string";

            }
            catch (ArgumentException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (Exception)
            {
                ErrorMessage = "Unexpected error occurred";
            }
        }

        private void ShowTweets()
        {
            try
            {
                TweetList =_userManager.GetTweets(Session);
            }
            catch (AuthenticationException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (ArgumentNullException)
            {
                ErrorMessage = "You are not authorized";
            }
            catch (Exception)
            {
                ErrorMessage = "Unexpected error occurred";
            }
        }

        public IEnumerable<Tweet> TweetList
        {
            get { return _tweetList; }
            set { Set(ref _tweetList, value); }
        }
    }
}