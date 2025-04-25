using System;
using System.IO;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using BlogAutoWriter.Services;
using Microsoft.Web.WebView2.Core; // 꼭 추가


namespace BlogAutoWriter.Views
{
    public partial class MainView : Window
    {
        private System.Timers.Timer? membershipTimer;

        private DateTime? startDate;
        private int validDays;
        private string grade = "Free";
        private string userId = "";
        private bool settingsOpen = false;

        public MainView()
        {
            InitializeComponent();
            Loaded += MainView_Loaded;

            // ✅ WebView2 자동 초기화
            InitializeWebView();
        }

        private void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            userId = App.Current.Properties["UserId"] as string ?? "unknown";
            startDate = App.Current.Properties["StartDate"] as DateTime? ?? DateTime.Now;
            validDays = (int)(App.Current.Properties["ValidDays"] ?? 0);
            grade = App.Current.Properties["Grade"] as string ?? "Free";

            UpdateMembershipText();
            StartMembershipTimer();
        }

        private async void InitializeWebView()
        {
            try
            {
                await HtmlPreviewBrowser.EnsureCoreWebView2Async();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"WebView2 초기화 실패: {ex.Message}", "WebView2 오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateMembershipText()
        {
            int remainDays = Math.Max(0, validDays - (int)(DateTime.Now - startDate!.Value).TotalDays);
            UserInfoText.Text = $"User: {userId}    등급: {grade}    남은 기간: {remainDays}일";
        }

        private void StartMembershipTimer()
        {
            membershipTimer = new System.Timers.Timer(10000);
            membershipTimer.Elapsed += CheckMembership;
            membershipTimer.Start();
        }

        private void CheckMembership(object? sender, ElapsedEventArgs e)
        {
            if ((DateTime.Now - startDate!.Value).TotalDays > validDays)
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
                DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
            {
                SettingsPanel.Visibility = Visibility.Visible;
                HtmlPreviewBrowser.Visibility = Visibility.Collapsed; // 👉 WebView2 숨김
            }
            else
            {
                animation.Completed += (_, _) =>
                {
                    SettingsPanel.Visibility = Visibility.Collapsed;
                    HtmlPreviewBrowser.Visibility = Visibility.Visible; // 👉 다시 보이게
                };
            }

            SettingsPanel.BeginAnimation(FrameworkElement.WidthProperty, animation);
        }


        private async void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string markdown = File.ReadAllText("SampleData/sample.txt", Encoding.UTF8);

                string htmlBody = MarkdownToHtmlConverter.Convert(markdown);
                string templateClass = GetTemplateClass();
                string fullHtml = $"<div class=\"{templateClass}\">{htmlBody}</div>";

                Directory.CreateDirectory("SampleData");
                File.WriteAllText("SampleData/result.html", fullHtml, Encoding.UTF8);

                RenderHtml(fullHtml);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"파일 처리 중 오류 발생: {ex.Message}", "에러", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private string GetCssFileNameByTemplate()
        {
            var selected = TemplateSelector.SelectedItem as ComboBoxItem;
            var name = selected?.Content?.ToString() ?? "깨끗한 라이트";

            return name switch
            {
                "깨끗한 라이트" => "clean.css",
                "모던 다크" => "dark.css",
                "컬러풀 비비드" => "vivid.css",
                _ => "clean.css"
            };
        }

        private void RenderHtml(string html)
        {
            string cssFileName = GetCssFileNameByTemplate();
            string cssPath = Path.Combine(Directory.GetCurrentDirectory(), "CSS", cssFileName);
            string css = File.Exists(cssPath) ? File.ReadAllText(cssPath, Encoding.UTF8) : "";

            string fullHtml = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8'>
                <style>{css}</style>
            </head>
            <body>
                {html}
            </body>
            </html>";

            HtmlPreviewBrowser.CoreWebView2?.NavigateToString(fullHtml);
        }
    }
}
