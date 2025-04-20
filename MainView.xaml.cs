using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using BlogAutoWriter;

namespace BlogAutoWriter
{
    public partial class MainView : UserControl
    {
        private readonly GptService _gptService = new();
        private bool isSettingsOpen = false;

        public MainView()
        {
            InitializeComponent();

            ReserveCheck.Checked += ReserveCheck_Checked;
            ReserveCheck.Unchecked += ReserveCheck_Unchecked;

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

        private void ReserveCheck_Checked(object sender, RoutedEventArgs e)
        {
            ReserveTimePanel.Visibility = Visibility.Visible;

            // Placeholder가 정확히 재적용되도록 렌더링 재요청
            Dispatcher.InvokeAsync(() =>
            {
                ReserveTimeInput.InvalidateVisual();
                ReserveTimeInput.UpdateLayout();
            });
        }

        private void ReserveCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            ReserveTimePanel.Visibility = Visibility.Collapsed;
            // ✅ 텍스트 초기화
            ReserveTimeInput.Text = "";
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

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
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
