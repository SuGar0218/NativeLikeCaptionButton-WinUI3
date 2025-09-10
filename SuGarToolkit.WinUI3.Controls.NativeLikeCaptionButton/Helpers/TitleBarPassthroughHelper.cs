using Microsoft.UI.Input;
using Microsoft.UI.Xaml;

using System.Collections.Generic;

using Windows.Graphics;

namespace SuGarToolkit.WinUI3.Controls.NativeLikeCaptionButton.Helpers;

internal partial class TitleBarPassthroughHelper
{
    public TitleBarPassthroughHelper(Window window)
    {
        _nonClientPointerSource = InputNonClientPointerSource.GetForWindowId(window.AppWindow.Id);
        _passthroughRegions = [];
    }

    public TitleBarPassthroughHelper AddDragRegion(IEnumerable<UIElement> elements)
    {
        return AddDragRegion([.. elements]);
    }

    public TitleBarPassthroughHelper Add(params UIElement[] elements)
    {
        foreach (UIElement element in elements)
        {
            _passthroughRegions.Add(element, default);
        }
        return this;
    }

    public TitleBarPassthroughHelper Add(NonClientRegionKind kind, IEnumerable<UIElement> elements)
    {
        return Add(kind, [.. elements]);
    }

    public TitleBarPassthroughHelper Remove(params UIElement[] elements)
    {
        foreach (UIElement element in elements)
        {
            _passthroughRegions.Remove(element);
        }
        return this;
    }

    public TitleBarPassthroughHelper Refresh()
    {
        foreach (UIElement element in _passthroughRegions.Keys)
        {
            if (element.Visibility is not Visibility.Visible)
            {
                _passthroughRegions[element] = new RectInt32(0, 0, 0, 0);
            }
            else
            {
                GeneralTransformHelper generalTransformHelper = new(element);
                _passthroughRegions[element] = generalTransformHelper.GetPixelRect(generalTransformHelper.GetRegionRect());
            }
        }
        return this;
    }

    public TitleBarPassthroughHelper Clear()
    {
        _passthroughRegions.Clear();
        return this;
    }

    public void Apply()
    {
        _nonClientPointerSource.SetRegionRects(NonClientRegionKind.Passthrough, [.. _passthroughRegions.Values]);
    }

    private readonly Dictionary<UIElement, RectInt32> _passthroughRegions;

    private InputNonClientPointerSource _nonClientPointerSource;
}
