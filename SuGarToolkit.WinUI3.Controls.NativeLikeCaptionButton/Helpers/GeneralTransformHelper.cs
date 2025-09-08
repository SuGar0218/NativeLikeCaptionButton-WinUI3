using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

using Windows.Foundation;
using Windows.Graphics;

namespace SuGarToolkit.WinUI3.Controls.NativeLikeCaptionButton.Helpers;

public class GeneralTransformHelper
{
    public GeneralTransformHelper(UIElement element)
    {
        _element = element;
        _transform = element.TransformToVisual(null);
    }

    public Point GetPositionPoint()
    {
        return _transform.TransformPoint(new Point(0, 0));
    }

    public Rect GetRegionRect()
    {
        return _transform.TransformBounds(new Rect
        {
            Width = _element.ActualSize.X,
            Height = _element.ActualSize.Y
        });
    }

    public Point GetPixelPoint(Point dipPoint) => new()
    {
        X = dipPoint.X * _element.XamlRoot.RasterizationScale,
        Y = dipPoint.Y * _element.XamlRoot.RasterizationScale
    };

    public RectInt32 GetPixelRect(Rect dipRect) => new()
    {
        X = (int) (dipRect.X * _element.XamlRoot.RasterizationScale),
        Y = (int) (dipRect.Y * _element.XamlRoot.RasterizationScale),
        Width = (int) (dipRect.Width * _element.XamlRoot.RasterizationScale),
        Height = (int) (dipRect.Height * _element.XamlRoot.RasterizationScale)
    };

    private readonly UIElement _element;
    private readonly GeneralTransform _transform;
}
