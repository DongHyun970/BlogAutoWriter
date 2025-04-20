using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

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
                Margin = new Thickness(4, 0, 0, 0)
            };

            AddVisualChild(_textBlock);
        }

        protected override int VisualChildrenCount => 1;
        protected override Visual GetVisualChild(int index) => _textBlock;

        protected override Size MeasureOverride(Size constraint)
        {
            _textBlock.Measure(constraint);
            return _textBlock.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            // TextBox 기본 padding + 약간 위로 올리기
            double verticalOffset = (finalSize.Height - _textBlock.DesiredSize.Height) / 2 - 1;

            var location = new Point(6, verticalOffset); // 왼쪽 여백 6px
            _textBlock.Arrange(new Rect(location, _textBlock.DesiredSize));

            return finalSize;
        }

    }
}
