using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using BlogAutoWriter.Services;
using System.Windows.Controls;


namespace BlogAutoWriter.Views
{
    public partial class LoginView : Window
    {
        private const string SettingsFile = "user.settings";

        public LoginView()
        {
            InitializeComponent();
            LoadSavedUserId();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard fadeIn = (Storyboard)this.Resources["FadeIn"];
            fadeIn.Begin(RootContainer);
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void LoadSavedUserId()
        {
            if (File.Exists(SettingsFile))
            {
                UserIdTextBox.Text = File.ReadAllText(SettingsFile);
                RememberIdCheckBox.IsChecked = true;
            }
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            StatusText.Opacity = 0;
            LoginProgressBar.Visibility = Visibility.Visible;
            LoginButton.IsEnabled = false;

            string userid = UserIdTextBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            try
            {
                var result = await LoginService.LoginAsync(userid, password);

                LoginProgressBar.Visibility = Visibility.Collapsed;
                LoginButton.IsEnabled = true;

                if (result.Success)
                {
                    if (RememberIdCheckBox.IsChecked == true)
                        File.WriteAllText(SettingsFile, userid);
                    else if (File.Exists(SettingsFile))
                        File.Delete(SettingsFile);

                    // ✅ 로그인 정보 App.Properties에 저장
                    App.Current.Properties["StartDate"] = result.StartDate;
                    App.Current.Properties["ValidDays"] = result.ValidDays;
                    App.Current.Properties["Grade"] = result.Grade;

                    var main = new MainView();
                    main.Show();
                    this.Close();
                }
                else
                {
                    StatusText.Foreground = System.Windows.Media.Brushes.OrangeRed;
                    StatusText.Text = result.Reason == "expired" ? "기간이 만료된 계정입니다."
                                       : result.Reason == "not_found" ? "아이디 또는 비밀번호가 잘못되었습니다."
                                       : $"로그인 실패: {result.Reason}";
                    StatusText.Opacity = 1;

                    Storyboard shake = (Storyboard)this.Resources["Shake"];
                    shake.Begin(RootContainer);
                }
            }
            catch (System.Exception ex)
            {
                LoginProgressBar.Visibility = Visibility.Collapsed;
                LoginButton.IsEnabled = true;
                MessageBox.Show($"로그인 중 오류 발생: {ex.Message}", "에러", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}