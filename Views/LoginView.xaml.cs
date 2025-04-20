using System.Windows;
using System.Windows.Media.Animation;
using BlogAutoWriter.Services;

namespace BlogAutoWriter.Views
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            StatusText.Opacity = 0;
            LoginProgressBar.Visibility = Visibility.Visible;
            ((UIElement)sender).IsEnabled = false;

            string userid = UserIdTextBox.Text.Trim();
            string password = PasswordBox.Password.Trim();
            string passwordHash = password; // TODO: 해시 적용 예정

            var result = await LoginService.LoginAsync(userid, passwordHash);

            LoginProgressBar.Visibility = Visibility.Collapsed;
            ((UIElement)sender).IsEnabled = true;

            if (result.Success)
            {
                StatusText.Foreground = System.Windows.Media.Brushes.LightGreen;
                StatusText.Text = $"Login successful! Grade: {result.Grade}";
                StatusText.Opacity = 1;

                // TODO: MainView로 전환 구현 필요
            }
            else
            {
                var shake = (Storyboard)this.Resources["ShakeAnimation"];
                shake.Begin();

                StatusText.Foreground = System.Windows.Media.Brushes.OrangeRed;
                StatusText.Text = result.Reason == "expired"
                    ? "기간이 만료된 계정입니다."
                    : result.Reason == "not_found"
                        ? "아이디 또는 비밀번호가 잘못되었습니다."
                        : $"로그인 실패: {result.Reason}";
                StatusText.Opacity = 1;
            }
        }
    }
}