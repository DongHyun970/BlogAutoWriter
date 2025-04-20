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
            // 창 크기 애니메이션
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

            // MainView 표시 및 애니메이션
            MainViewContainer.Visibility = Visibility.Visible;
            var storyboard = (Storyboard)this.Resources["ExpandMainView"];
            storyboard.Begin(this);

            // LoginView 숨기기
            LoginView.Visibility = Visibility.Collapsed;
        }
    }
}
