using System.Windows;
using BlogAutoWriter.Views;

namespace BlogAutoWriter
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var login = new LoginView();
            login.Show();
        }
    }
}