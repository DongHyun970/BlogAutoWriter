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

        private static readonly DependencyProperty PlaceholderAdornerProperty =
            DependencyProperty.RegisterAttached(
                "PlaceholderAdorner",
                typeof(PlaceholderAdorner),
                typeof(PlaceholderBehavior),
                new PropertyMetadata(null));

        public static string GetPlaceholder(DependencyObject obj) =>
            (string)obj.GetValue(PlaceholderProperty);

        public static void SetPlaceholder(DependencyObject obj, string value) =>
            obj.SetValue(PlaceholderProperty, value);

        private static PlaceholderAdorner GetPlaceholderAdorner(DependencyObject obj) =>
            (PlaceholderAdorner)obj.GetValue(PlaceholderAdornerProperty);

        private static void SetPlaceholderAdorner(DependencyObject obj, PlaceholderAdorner adorner) =>
            obj.SetValue(PlaceholderAdornerProperty, adorner);

        private static void OnPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox tb)
            {
                tb.Loaded -= TextBoxOnLoaded;
                tb.Unloaded -= TextBoxOnUnloaded;
                tb.TextChanged -= TextBoxOnTextChanged;

                tb.Loaded += TextBoxOnLoaded;
                tb.Unloaded += TextBoxOnUnloaded;
                tb.TextChanged += TextBoxOnTextChanged;
            }
            else if (d is PasswordBox pb)
            {
                pb.Loaded -= PasswordBoxOnLoaded;
                pb.Unloaded -= PasswordBoxOnUnloaded;
                pb.PasswordChanged -= PasswordBoxOnPasswordChanged;

                pb.Loaded += PasswordBoxOnLoaded;
                pb.Unloaded += PasswordBoxOnUnloaded;
                pb.PasswordChanged += PasswordBoxOnPasswordChanged;
            }
        }

        private static void TextBoxOnLoaded(object sender, RoutedEventArgs e)
        {
            var tb = (TextBox)sender;
            UpdatePlaceholder(tb);
            tb.Dispatcher.InvokeAsync(() =>
            {
                tb.InvalidateVisual();
                tb.UpdateLayout();
            });
        }

        private static void TextBoxOnUnloaded(object sender, RoutedEventArgs e)
        {
            var tb = (TextBox)sender;
            tb.Loaded -= TextBoxOnLoaded;
            tb.Unloaded -= TextBoxOnUnloaded;
            tb.TextChanged -= TextBoxOnTextChanged;
            RemoveAdorner(tb);
        }

        private static void TextBoxOnTextChanged(object sender, TextChangedEventArgs e) =>
            UpdatePlaceholder((TextBox)sender);

        private static void PasswordBoxOnLoaded(object sender, RoutedEventArgs e)
        {
            var pb = (PasswordBox)sender;
            UpdatePlaceholder(pb);
            pb.Dispatcher.InvokeAsync(() =>
            {
                pb.InvalidateVisual();
                pb.UpdateLayout();
            });
        }

        private static void PasswordBoxOnUnloaded(object sender, RoutedEventArgs e)
        {
            var pb = (PasswordBox)sender;
            pb.Loaded -= PasswordBoxOnLoaded;
            pb.Unloaded -= PasswordBoxOnUnloaded;
            pb.PasswordChanged -= PasswordBoxOnPasswordChanged;
            RemoveAdorner(pb);
        }

        private static void PasswordBoxOnPasswordChanged(object sender, RoutedEventArgs e) =>
            UpdatePlaceholder((PasswordBox)sender);

        private static void UpdatePlaceholder(Control ctrl)
        {
            // ✅ 완전히 보이는 상태가 아니라면 그리지 않는다
            if (ctrl.Visibility != Visibility.Visible || !ctrl.IsVisible || ctrl.ActualWidth == 0 || ctrl.ActualHeight == 0)
                return;

            var layer = AdornerLayer.GetAdornerLayer(ctrl);
            if (layer == null) return;

            var placeholder = GetPlaceholder(ctrl);
            var existing = GetPlaceholderAdorner(ctrl);

            bool isEmpty = ctrl switch
            {
                TextBox tb => string.IsNullOrWhiteSpace(tb.Text),
                PasswordBox pb => string.IsNullOrWhiteSpace(pb.Password),
                _ => false
            };

            if (isEmpty && existing == null && !string.IsNullOrEmpty(placeholder))
            {
                var adorner = new PlaceholderAdorner(ctrl, placeholder);
                layer.Add(adorner);
                SetPlaceholderAdorner(ctrl, adorner);
            }
            else if (!isEmpty && existing != null)
            {
                layer.Remove(existing);
                SetPlaceholderAdorner(ctrl, null);
            }
        }


        private static void RemoveAdorner(Control ctrl)
        {
            var existing = GetPlaceholderAdorner(ctrl);
            if (existing != null)
            {
                AdornerLayer.GetAdornerLayer(ctrl)?.Remove(existing);
                SetPlaceholderAdorner(ctrl, null);
            }
        }

    }
}