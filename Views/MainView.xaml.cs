using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using BlogAutoWriter.Models;
using BlogAutoWriter.Services;

namespace BlogAutoWriter.Views
{
    public partial class MainView : UserControl
    {
        private readonly GptService _gptService = new();
        private bool isSettingsOpen = false;

        public MainView()
        {
            InitializeComponent();

            ReserveCheck.Checked += (_, _) => ReserveTimePanel.Visibility = Visibility.Visible;
            ReserveCheck.Unchecked += (_, _) => ReserveTimePanel.Visibility = Visibility.Collapsed;

            Loaded += MainView_Loaded;
        }

        private void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            string grade = AppState.CurrentUser?.Grade ?? "basic";
        }

        private async void RecommendBtn_Click(object sender, RoutedEventArgs e)
        {
            RecommendBtn.IsEnabled = false;
            RecommendBtn.Content = "불러오는 중...";

            try
            {
                var keywords = await _gptService.GetRecommendedKeywords();
                if (keywords.Count > 0)
                {
                    KeywordInput.Text = keywords[0];
                    MessageBox.Show("추천 키워드:\n" + string.Join("\n", keywords));
                }
                else
                {
                    MessageBox.Show("추천 키워드를 가져올 수 없습니다.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("에러: " + ex.Message);
            }

            RecommendBtn.IsEnabled = true;
            RecommendBtn.Content = "추천 키워드";
        }

        private void RemoveText(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && tb.Text == "HH:mm")
            {
                tb.Text = "";
                tb.Foreground = System.Windows.Media.Brushes.White;
            }
        }

        private void AddText(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = "HH:mm";
                tb.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Window.GetWindow(this)?.DragMove();
            }
        }

        private void SettingsToggleBtn_Click(object sender, RoutedEventArgs e)
        {
            SettingsPanelHost.Visibility = Visibility.Visible;

            var animation = new DoubleAnimation
            {
                From = isSettingsOpen ? 360 : 0,
                To = isSettingsOpen ? 0 : 360,
                Duration = new Duration(TimeSpan.FromMilliseconds(300)),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };

            if (isSettingsOpen)
            {
                animation.Completed += (s, _) => SettingsPanelHost.Visibility = Visibility.Collapsed;
            }

            SettingsPanelHost.BeginAnimation(WidthProperty, animation);
            isSettingsOpen = !isSettingsOpen;
        }
    }
}
