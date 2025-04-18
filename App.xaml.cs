using System.Windows;
using BlogAutoWriter.Models;

namespace BlogAutoWriter
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AppSettings.Load(); // 자동 설정 불러오기
        }
    }
}
