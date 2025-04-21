using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BlogAutoWriter.Services;

namespace BlogAutoWriter.Views
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoginButton.Click += async (_, _) =>
            {
                string userid = UserIdBox.Text.Trim();
                string password = PasswordBox.Password.Trim();

                if (string.IsNullOrWhiteSpace(userid) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("아이디와 비밀번호를 입력해주세요.");
                    return;
                }

                LoginButton.IsEnabled = false;

                try
                {
                    var result = await LoginService.TryLoginAsync(userid, password);
                    if (result.Success)
                    {
                        App.Current.Properties["UserId"] = result.UserId;
                        App.Current.Properties["Grade"] = result.Grade;
                        App.Current.Properties["StartDate"] = result.StartDate;
                        App.Current.Properties["ValidDays"] = result.ValidDays;

                        new MainView().Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(result.Reason == "expired"
                            ? "⛔ 사용 기간이 만료되었습니다."
                            : "🚫 로그인 정보가 올바르지 않습니다.",
                            "로그인 실패", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("로그인 도중 오류 발생: " + ex.Message);
                }
                finally
                {
                    LoginButton.IsEnabled = true;
                }
            };
        }
    }
}