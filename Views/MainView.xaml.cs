using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using BlogAutoWriter.Services;
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

        private bool settingsOpen = false;

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
            try
            {
                settingsOpen = !settingsOpen;
                ToggleSettingsPanel(settingsOpen);
            }
            catch (Exception ex)
            {
                MessageBox.Show("설정 패널 열기 오류:\n" + ex.Message, "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void ToggleSettingsPanel(bool open)
        {
            double targetWidth = open ? 360 : 0;

            var animation = new DoubleAnimation
            {
                To = targetWidth,
                Duration = TimeSpan.FromMilliseconds(300),
                AccelerationRatio = 0.2,
                DecelerationRatio = 0.8
            };

            if (open)
                SettingsPanel.Visibility = Visibility.Visible;
            else
                animation.Completed += (_, _) => SettingsPanel.Visibility = Visibility.Collapsed;

            SettingsPanel.BeginAnimation(FrameworkElement.WidthProperty, animation); // ✅ 핵심 변경
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


        private string GetTemplateClass()
        {
            var selected = TemplateSelector.SelectedItem as ComboBoxItem;
            var templateName = selected?.Content?.ToString() ?? "깨끗한 라이트";

            return templateName switch
            {
                "깨끗한 라이트" => "baw-light",
                "모던 다크" => "baw-dark",
                "컬러풀 비비드" => "baw-vibrant",
                _ => "baw-light"
            };
        }

        private void RenderHtml(string html)
        {
            string htmlWrapper = $@"
        <!DOCTYPE html>
        <html>
        <head>
        <meta charset='utf-8'>
        <style>
        body {{ margin: 0; padding: 0; }}
        .baw-light {{ background: white; color: #222; font-family: 'Noto Sans KR'; padding: 20px; }}
        .baw-dark {{ background: #222; color: #eee; font-family: 'Noto Sans KR'; padding: 20px; }}
        .baw-vibrant {{ background: #f0f8ff; color: #333; font-family: 'Nanum Gothic'; padding: 20px; }}
        .baw-section h2 {{ font-size: 20px; margin-top: 30px; }}
        .baw-table {{ width: 100%; border-collapse: collapse; margin: 20px 0; }}
        .baw-table th, .baw-table td {{ border: 1px solid #ccc; padding: 10px; }}
        </style>
        </head>
        <body>
        {html}
        </body>
        </html>";

            HtmlPreviewBrowser.NavigateToString(htmlWrapper);
        }


    }
}
