using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;

public class DragAdorner : Adorner
{
    private readonly VisualBrush _brush;
    private Point _offset;
    private readonly Size _size;

    public DragAdorner(UIElement adornedElement, UIElement visual) : base(adornedElement)
    {
        _brush = new VisualBrush(visual) { Opacity = 0.5 };
        _size = visual.RenderSize;
        IsHitTestVisible = false;
    }

    public void UpdatePosition(Point location)
    {
        _offset = location;
        // Ép buộc Adorner vẽ lại
        this.InvalidateVisual();
    }

    protected override void OnRender(DrawingContext dc)
    {
        // Vẽ tại vị trí chuột, lệch lên một chút để không bị ngón tay/con trỏ che mất
        var p = new Point(_offset.X - 20, _offset.Y - 15);
        dc.DrawRectangle(_brush, null, new Rect(p, _size));
    }
}