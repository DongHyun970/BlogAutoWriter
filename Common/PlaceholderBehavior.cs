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

            // 기존 Placeholder 제거
            var adorners = layer.GetAdorners(textBox);
            if (adorners != null)
            {
                foreach (var adorner in adorners)
                {
                    if (adorner is PlaceholderAdorner)
                        layer.Remove(adorner);
                }
            }

            // 텍스트가 비어 있을 때만 Placeholder 표시
            if (string.IsNullOrEmpty(textBox.Text))
            {
                string placeholder = GetPlaceholder(textBox);
                layer.Add(new PlaceholderAdorner(textBox, placeholder));
            }
        }
    }
}
