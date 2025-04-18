using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace BlogAutoWriter
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void ShowMainView()
        {
            // 1. 창 크기 부드럽게 확장
            var widthAnim = new DoubleAnimation
            {
                From = this.Width,
                To = 1000,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };
            var heightAnim = new DoubleAnimation
            {
                From = this.Height,
                To = 600,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };

            this.BeginAnimation(Window.WidthProperty, widthAnim);
            this.BeginAnimation(Window.HeightProperty, heightAnim);

            // 2. 메인 뷰 표시 + 애니메이션 시작
            MainViewContainer.Visibility = Visibility.Visible;

            var storyboard = (Storyboard)this.Resources["ExpandMainView"];
            storyboard.Begin(this);

            // 3. 로그인 뷰 숨김
            LoginView.Visibility = Visibility.Collapsed;
        }
    }
}
