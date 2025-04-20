using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BlogAutoWriter;

namespace BlogAutoWriter
{
    public partial class LoginView : UserControl
    {
        private static readonly string GoogleScriptUrl = "https://script.google.com/macros/s/AKfycbwUxlxvupeXpJ3PB-uTzeuX4dqro7DTI6N8IJAyQqnRJitXpcv6KGfoWV7L_FMaECRj/exec";

        public LoginView()
        {
            InitializeComponent();
            PasswordInput.PasswordChanged += (s, e) =>
            {
                PasswordPlaceholder.Visibility = string.IsNullOrEmpty(PasswordInput.Password)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            };
        }

        private async void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            string userId = UserIdInput.Text.Trim();
            string password = PasswordInput.Password.Trim();

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password))
                return;

            string passwordHash = ComputeSha256Hash(password);

            // UI 전환
            LoginBtn.Visibility = Visibility.Collapsed;
            LoadingBar.Visibility = Visibility.Visible;
            ErrorIcon.Visibility = Visibility.Collapsed;

            try
            {
                var result = await ValidateLogin(userId, passwordHash);

                if (result.success)
                {
                    AppState.CurrentUser = new UserInfo { UserId = userId, Grade = result.grade };

                    if (Application.Current.MainWindow is MainWindow window)
                    {
                        // 디버깅용 로그
                        //MessageBox.Show("로그인 성공! MainView 보여주기 시도함.");

                        window.BeginStoryboard(window.Resources["ExpandMainView"] as System.Windows.Media.Animation.Storyboard);

                        // MainView가 null일 경우 대비해서 로그 찍기
                        if (window.MainView == null)
                            MessageBox.Show("MainView가 null이야!");

                        window.LoginView.Visibility = Visibility.Collapsed;
                        window.MainView.Visibility = Visibility.Visible;
                        window.ShowMainView(); // ✅ 이 줄 추가하면 해결!
                    }
                }
                else
                {
                    ShowLoginError();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[예외 발생] {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                ShowLoginError();
            }

            finally
            {
                LoginBtn.Visibility = Visibility.Visible;
                LoadingBar.Visibility = Visibility.Collapsed;
            }
        }

        private void ShowLoginError()
        {
            LoadingBar.Visibility = Visibility.Collapsed;
            LoginBtn.Visibility = Visibility.Visible;
            ErrorIcon.Visibility = Visibility.Visible;
        }

        private static async Task<(bool success, string reason, string grade)> ValidateLogin(string userId, string passwordHash)
        {
            var httpClient = new HttpClient();

            // ✅ 키 이름을 반드시 "password"로 맞춰야 함
            var requestData = new { userid = userId, password = passwordHash };

            var json = JsonSerializer.Serialize(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(GoogleScriptUrl, content);
            var responseText = await response.Content.ReadAsStringAsync();

            var jsonDoc = JsonDocument.Parse(responseText);
            var root = jsonDoc.RootElement;

            bool success = root.GetProperty("success").GetBoolean();
            string reason = root.TryGetProperty("reason", out var r) ? r.GetString() ?? "" : "";
            string grade = root.TryGetProperty("grade", out var g) ? g.GetString() ?? "" : "";


            return (success, reason, grade);
        }


        private static string ComputeSha256Hash(string rawData)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            var builder = new StringBuilder();
            foreach (var b in bytes)
                builder.Append(b.ToString("x2"));
            return builder.ToString();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void RemoveText(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && tb.Text == "아이디")
            {
                tb.Text = "";
                tb.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void AddText(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = "아이디";
                tb.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

    }
}
