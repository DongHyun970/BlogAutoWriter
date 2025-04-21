using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BlogAutoWriter.Services;
using BlogAutoWriter.Views;
using Timer = System.Timers.Timer;

namespace BlogAutoWriter.Views
{
    public partial class MainView : Window
    {
        private Timer? membershipTimer;
        private DateTime? startDate;
        private int validDays;
        private string grade = "Free";
        private string userId = "";

        public MainView()
        {
            InitializeComponent();
            Loaded += MainView_Loaded;
        }

        private void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            userId = App.Current.Properties["UserId"] as string ?? "unknown";

            object? rawStart = App.Current.Properties["StartDate"];
            if (rawStart is string s)
                startDate = DateTime.TryParse(s, out var parsed) ? parsed : DateTime.Now;
            else
                startDate = rawStart as DateTime? ?? DateTime.Now;

            validDays = (int)(App.Current.Properties["ValidDays"] ?? 0);
            grade = App.Current.Properties["Grade"] as string ?? "Free";

            UpdateMembershipText();
            StartMembershipTimer();
        }

        private void UpdateMembershipText()
        {
            int remainDays = Math.Max(0, validDays - (int)(DateTime.Now - startDate!.Value).TotalDays);
            UserInfoText.Text = $"User: {userId}    등급: {grade}    남은 기간: {remainDays}일";
        }

        private void StartMembershipTimer()
        {
            membershipTimer = new Timer(10000); // 10초마다 체크
            membershipTimer.Elapsed += CheckMembership;
            membershipTimer.Start();
        }

        private void CheckMembership(object? sender, System.Timers.ElapsedEventArgs e)
        {
            var now = DateTime.Now;
            var diff = (now - startDate!.Value).TotalDays;

            if (diff > validDays)
            {
                membershipTimer?.Stop();
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("⚠ 사용 기간이 만료되었습니다. 프로그램이 종료됩니다.", "만료", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Application.Current.Shutdown();
                });
            }
            else
            {
                Dispatcher.Invoke(UpdateMembershipText);
            }
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            new SettingsView().ShowDialog();
        }

        private async void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            string keyword = KeywordBox.Text.Trim();
            var selected = StyleSelector.SelectedItem as ComboBoxItem;
            string style = selected?.Content?.ToString() ?? "";

            var loginItem = BlogLoginMethodComboBox.SelectedItem as ComboBoxItem;
            string loginMethod = loginItem?.Content?.ToString() ?? "카카오 로그인";

            if (string.IsNullOrWhiteSpace(keyword) || string.IsNullOrWhiteSpace(style))
            {
                MessageBox.Show("키워드와 스타일을 모두 입력해주세요.");
                return;
            }

            PreviewTextBlock.Text = "GPT가 글을 생성 중입니다...";

            try
            {
                string prompt = $"'{keyword}' 키워드로 {style} 스타일의 블로그 글을 작성해줘.";
                string result = await GptService.GenerateBlogContentAsync(prompt);
                PreviewTextBlock.Text = result;

                if (loginMethod == "카카오 로그인")
                {
                    MessageBox.Show("카카오 로그인 자동화 경로로 이동합니다. (예정)");
                }
                else if (loginMethod == "티스토리 이메일 로그인")
                {
                    MessageBox.Show("티스토리 로그인 자동화 경로로 이동합니다. (예정)");
                }
            }
            catch (Exception ex)
            {
                PreviewTextBlock.Text = "글 생성 중 오류가 발생했습니다.\n" + ex.Message;
            }
        }
    }
}
