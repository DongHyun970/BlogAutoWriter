using System.Windows;
using BlogAutoWriter; // Models 제거

namespace BlogAutoWriter
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AppSettings.Load();
        }
    }
}
