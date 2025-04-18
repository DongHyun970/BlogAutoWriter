using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Controls;

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
            Margin = new Thickness(5, 0, 0, 0),
            VerticalAlignment = VerticalAlignment.Center
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
