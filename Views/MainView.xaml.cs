using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BlogAutoWriter.Services;
using BlogAutoWriter.Views;

namespace BlogAutoWriter.Views
{
    public partial class MainView : Window
    {
        private Timer? membershipTimer;
        private DateTime? startDate;
        private int validDays;
        private string grade = "Free";

        public MainView()
        {
            InitializeComponent();
            Loaded += MainView_Loaded;
        }

        private void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            // 테스트용: 실제 로그인 후 받아온 값으로 치환해야 함
            startDate = App.Current.Properties["StartDate"] as DateTime? ?? DateTime.Now;
            validDays = (int)(App.Current.Properties["ValidDays"] ?? 0);
            grade = App.Current.Properties["Grade"] as string ?? "Free";

            ShowMembershipInfo();
            StartMembershipTimer();
        }

        private void ShowMembershipInfo()
        {
            var remainDays = (int)(validDays - (DateTime.Now - startDate!.Value).TotalDays);
            MessageBox.Show($"남은 기간: {remainDays}일 | 등급: {grade}", "멤버십 정보", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void StartMembershipTimer()
        {
            membershipTimer = new Timer(10000); // 10초마다 체크
            membershipTimer.Elapsed += CheckMembership;
            membershipTimer.Start();
        }

        private void CheckMembership(object? sender, ElapsedEventArgs e)
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
            new SettingsWindow().ShowDialog();
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