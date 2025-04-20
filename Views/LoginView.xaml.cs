using System.Windows;
using System.Windows.Controls;
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
            // UI 초기화
            StatusText.Opacity = 0;
            LoginProgressBar.Visibility = Visibility.Visible;
            LoginButton.IsEnabled = false;

            string userid = UserIdTextBox.Text.Trim();
            string password = PasswordBox.Password.Trim();
            string passwordHash = password; // TODO: 실제 해시 적용 필요

            var result = await LoginService.LoginAsync(userid, passwordHash);

            LoginProgressBar.Visibility = Visibility.Collapsed;
            LoginButton.IsEnabled = true;

            if (result.Success)
            {
                // 로그인 성공: 메인 화면으로 전환 (임시 메시지)
                StatusText.Foreground = System.Windows.Media.Brushes.LightGreen;
                StatusText.Text = $"Login successful! Grade: {result.Grade}";
                StatusText.Opacity = 1;

                // TODO: MainView로 전환
            }
            else
            {
                // 로그인 실패: 흔들림 애니메이션 실행 + 메시지 표시
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