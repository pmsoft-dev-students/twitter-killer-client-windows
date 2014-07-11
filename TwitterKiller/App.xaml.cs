using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using RestSharp;

namespace TwitterKiller
{   
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            const string serverName = "10.10.10.54:8080";

            var view = new UserView()
            {
                DataContext = new UserViewModel(
                    new UserManager(
                        new RestClient("http://" + serverName + "/rest"),
                        new JsonNetSerializer()), 
                        new ConventionalDialogService(), 
                        Messenger.Default)
            };
            view.Show();
        }
    }
}
