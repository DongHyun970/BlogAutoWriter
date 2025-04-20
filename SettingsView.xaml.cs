using System.Windows;
using System.Windows.Controls;
using BlogAutoWriter.Models;

namespace BlogAutoWriter.Views
{
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void LoadBtn_Click(object sender, RoutedEventArgs e)
        {
            AppSettings.Load();
            OpenAiKeyBox.Text = AppSettings.Current.OpenAiApiKey;
            KakaoEmailBox.Text = AppSettings.Current.KakaoEmail;
            KakaoPasswordBox.Password = AppSettings.Current.KakaoPassword;
            TistoryEmailBox.Text = AppSettings.Current.TistoryEmail;
            TistoryPasswordBox.Password = AppSettings.Current.TistoryPassword;
            CategoryBox.Text = string.Join(", ", AppSettings.Current.Categories);
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            AppSettings.Current.OpenAiApiKey = OpenAiKeyBox.Text.Trim();
            AppSettings.Current.KakaoEmail = KakaoEmailBox.Text.Trim();
            AppSettings.Current.KakaoPassword = KakaoPasswordBox.Password.Trim();
            AppSettings.Current.TistoryEmail = TistoryEmailBox.Text.Trim();
            AppSettings.Current.TistoryPassword = TistoryPasswordBox.Password.Trim();
            AppSettings.Current.Categories = CategoryBox.Text.Split(',');

            AppSettings.Save();
        }
    }
}