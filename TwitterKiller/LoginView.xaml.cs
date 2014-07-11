using GalaSoft.MvvmLight.Messaging;

namespace TwitterKiller
{
    public partial class LoginView
    {
        public LoginView()
        {
            InitializeComponent();

            Messenger.Default.Register<object>(this, (obj) => 
                Close());
        }
    }
}
