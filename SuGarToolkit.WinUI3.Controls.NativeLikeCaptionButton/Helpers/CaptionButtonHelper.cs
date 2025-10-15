using Microsoft.UI.Input;
using Microsoft.UI.Xaml;

using System.Collections.Generic;

using Windows.Foundation;
using Windows.Graphics;

namespace SuGarToolkit.WinUI3.Controls.NativeLikeCaptionButton.Helpers;

public class CaptionButtonHelper
{
    public CaptionButtonHelper(Window targetWindow)
    {
        _targetWindow = targetWindow;
        _inputNonClientPointerSource = InputNonClientPointerSource.GetForWindowId(_targetWindow.AppWindow.Id);
        _inputNonClientPointerSource.RegionsChanged += OnRegionsChanged;
        if (_targetWindow.Content is FrameworkElement content)
        {
            content.Loaded += OnWindowContentLoaded;
            content.Unloaded += OnWindowContentUnloaded;
            if (content.IsLoaded)
            {
                OnWindowContentLoaded(content, new RoutedEventArgs());
            }
        }
    }

    public void Add(CaptionButtonKind kind, UIElement element)
    {
        if (_nonClientElements.TryGetValue(kind, out List<UIElement>? nonClientElement))
        {
            nonClientElement ??= [];
            nonClientElement.Add(element);
        }
        else
        {
            _nonClientElements.Add(kind, [element]);
        }
        _elementKinds.Add(element, kind);
    }

    public void Refresh(UIElement element)
    {
        if ((element.ActualSize.X == 0 && element.ActualSize.Y == 0) || element.Visibility is Visibility.Collapsed)
            return;

        double scale = element.XamlRoot.RasterizationScale;

        Rect elementDipRect = element.TransformToVisual(null).TransformBounds(new Rect(
            0,
            0,
            element.ActualSize.X,
            element.ActualSize.Y));

        RectInt32 elementPixelRect = new(
            (int) (elementDipRect.X * scale),
            (int) (elementDipRect.Y * scale),
            (int) (elementDipRect.Width * scale),
            (int) (elementDipRect.Height * scale));

        _elementRects[element] = elementPixelRect;
    }

    public void Refresh(CaptionButtonKind kind)
    {
        for (int i = 0; i < _nonClientElements[kind].Count; i++)
        {
            Refresh(_nonClientElements[kind][i]);
        }
    }

    public void Apply(CaptionButtonKind kind)
    {
        RectInt32[] rects = new RectInt32[_nonClientElements[kind].Count];
        for (int i = 0; i < rects.Length; i++)
        {
            rects[i] = _elementRects[_nonClientElements[kind][i]];
        }
        _inputNonClientPointerSource.SetRegionRects((NonClientRegionKind) kind, rects);
    }

    /// <summary>
    /// 防止变成 0 尺寸
    /// </summary>
    private void OnRegionsChanged(InputNonClientPointerSource sender, NonClientRegionsChangedEventArgs args)
    {
        for (int i = 0; i < args.ChangedRegions.Length; i++)
        {
            if (!_nonClientElements.TryGetValue((CaptionButtonKind) args.ChangedRegions[i], out List<UIElement>? elements) || elements is null)
                continue;

            RectInt32[]? rects = sender.GetRegionRects(args.ChangedRegions[i]);

            if (rects is null)
                continue;

            bool abnormal = false;
            for (int j = 0; j < rects.Length; j++)
            {
                if (!abnormal && rects[j].Width == 0 && rects[j].Height == 0)
                {
                    abnormal = true;
                }
            }
            if (abnormal)
            {
                Refresh((CaptionButtonKind) args.ChangedRegions[i]);
                Apply((CaptionButtonKind) args.ChangedRegions[i]);
            }
        }
    }

    private void OnWindowContentLoaded(object sender, RoutedEventArgs e)
    {
        FrameworkElement content = (FrameworkElement) sender;
        content.SizeChanged += OnWindowContentSizeChanged;
    }

    private void OnWindowContentUnloaded(object sender, RoutedEventArgs e)
    {
        FrameworkElement content = (FrameworkElement) sender;
        content.SizeChanged -= OnWindowContentSizeChanged;
    }

    private void OnWindowContentSizeChanged(object sender, SizeChangedEventArgs args)
    {
        RefreshAndApplyCaptionButtonRegions();
    }

    private void RefreshAndApplyCaptionButtonRegions()
    {
        Refresh(CaptionButtonKind.Minimize);
        Refresh(CaptionButtonKind.Maximize);
        Refresh(CaptionButtonKind.Close);
        Apply(CaptionButtonKind.Minimize);
        Apply(CaptionButtonKind.Maximize);
        Apply(CaptionButtonKind.Close);
    }

    private readonly Window _targetWindow;
    private readonly InputNonClientPointerSource _inputNonClientPointerSource;
    private readonly Dictionary<CaptionButtonKind, List<UIElement>> _nonClientElements = [];
    private readonly Dictionary<UIElement, RectInt32> _elementRects = [];
    private readonly Dictionary<UIElement, CaptionButtonKind> _elementKinds = [];
}
