using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
                textBox.Loaded += (s, ev) => ShowPlaceholder(textBox);
                textBox.TextChanged += (s, ev) => ShowPlaceholder(textBox);
            }
        }

        private static void ShowPlaceholder(TextBox textBox)
        {
            var layer = AdornerLayer.GetAdornerLayer(textBox);
            if (layer == null) return;

            // 기존 placeholder 제거
            var adorners = layer.GetAdorners(textBox);
            if (adorners != null)
            {
                foreach (var adorner in adorners)
                {
                    if (adorner is PlaceholderAdorner)
                        layer.Remove(adorner);
                }
            }

            if (string.IsNullOrEmpty(textBox.Text))
            {
                layer.Add(new PlaceholderAdorner(textBox, GetPlaceholder(textBox)));
            }
        }
    }
}