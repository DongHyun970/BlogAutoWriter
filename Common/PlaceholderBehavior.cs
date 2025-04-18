using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace BlogAutoWriter.Common
{
    public static class PlaceholderBehavior
    {
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.RegisterAttached(
                "Placeholder",
                typeof(string),
                typeof(PlaceholderBehavior),
                new PropertyMetadata(null, OnPlaceholderChanged));

        public static string GetPlaceholder(DependencyObject obj)
        {
            return (string)obj.GetValue(PlaceholderProperty);
        }

        public static void SetPlaceholder(DependencyObject obj, string value)
        {
            obj.SetValue(PlaceholderProperty, value);
        }

        private static void OnPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                textBox.Loaded += (s, ev) => UpdatePlaceholder(textBox);
                textBox.TextChanged += (s, ev) => UpdatePlaceholder(textBox);
            }
        }

        private static void UpdatePlaceholder(TextBox textBox)
        {
            var layer = AdornerLayer.GetAdornerLayer(textBox);
            if (layer == null) return;

            // 기존 Placeholder 제거 (중복 방지)
            var adorners = layer.GetAdorners(textBox);
            if (adorners != null)
            {
                foreach (var adorner in adorners)
                {
                    if (adorner is PlaceholderAdorner)
                        layer.Remove(adorner);
                }
            }

            // 새 Placeholder 추가 (텍스트 비었을 때만)
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                string placeholder = GetPlaceholder(textBox);
                if (!string.IsNullOrEmpty(placeholder))
                {
                    layer.Add(new PlaceholderAdorner(textBox, placeholder));
                }
            }
        }

    }
}
