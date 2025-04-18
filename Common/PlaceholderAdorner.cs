using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Controls;


namespace BlogAutoWriter.Common
{
    public class PlaceholderAdorner : Adorner
    {
        private readonly TextBlock _textBlock;

        public PlaceholderAdorner(UIElement element, string placeholder)
            : base(element)
        {
            IsHitTestVisible = false;

            _textBlock = new TextBlock
            {
                Text = placeholder,
                Foreground = Brushes.Gray,
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(6, 0, 0, 0) // ← 텍스트박스 내부 여백과 맞춤
            };


            AddVisualChild(_textBlock);
        }

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index) => _textBlock;

        protected override Size MeasureOverride(Size constraint)
        {
            _textBlock.Measure(constraint);
            return constraint;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _textBlock.Arrange(new Rect(finalSize));
            return finalSize;
        }
    }
}