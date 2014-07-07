using System.Windows;
using RestSharp;

namespace Twitter_Killer
{   
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            const string serverName = "";
            // initializing client view with data context
            var clientView = new LoginView {DataContext = new LoginViewModel(
                new Authenticator(
                    new RestClient("http://" + serverName + "/rest"), 
                    new JsonWrapper(), 
                    new RestRequestWrapper()))};
            clientView.Show();

        }
    }
}
