using Microsoft.UI.Input;
using Microsoft.UI.Xaml;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

using Windows.Graphics;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Shell;

using WinRT.Interop;

namespace SuGarToolkit.WinUI3.Controls.NativeLikeCaptionButton.Helpers;

internal partial class TitleBarNonClientRegionHelper// : IDisposable
{
    public TitleBarNonClientRegionHelper(Window window)
    {
        _window = window;
        _hwnd = new HWND(WindowNative.GetWindowHandle(window));
        //_hwndReunionWindowingCaptionControls = PInvoke.FindWindowEx(_hwnd, HWND.Null, "ReunionWindowingCaptionControls", null);
        _nonClientPointerSource = InputNonClientPointerSource.GetForWindowId(window.AppWindow.Id);
        //_regions[NonClientRegionKind.Icon] = [];
        //_regions[NonClientRegionKind.Caption] = [];
        //_regions[NonClientRegionKind.Minimize] = [];
        //_regions[NonClientRegionKind.Maximize] = [];
        //_regions[NonClientRegionKind.Close] = [];
        _regions[NonClientRegionKind.Passthrough] = [];
        //_dragRegions = [];
        //_subclassProc = HitTestProcedure;
        //PInvoke.SetWindowSubclass(_hwnd, _subclassProc, 1, 0);
        //_nonClientPointerSource.RegionsChanged += OnRegionsChanged;
    }

    //private void OnRegionsChanged(InputNonClientPointerSource sender, NonClientRegionsChangedEventArgs args)
    //{
    //    foreach (RectInt32 rect in _nonClientPointerSource.GetRegionRects(NonClientRegionKind.Maximize))
    //    {
    //        if (!_regions[NonClientRegionKind.Maximize].Values.Contains(rect))
    //        {
    //            Apply(NonClientRegionKind.Maximize);
    //            return;
    //        }
    //    }
    //}

    //public TitleBarNonClientRegionHelper AddDragRegion(params UIElement[] elements)
    //{
    //    foreach (UIElement element in elements)
    //    {
    //        _dragRegions.Add(element, default);
    //    }
    //    return this;
    //}

    public TitleBarNonClientRegionHelper AddDragRegion(IEnumerable<UIElement> elements)
    {
        return AddDragRegion([..elements]);
    }

    public TitleBarNonClientRegionHelper Add(NonClientRegionKind kind, params UIElement[] elements)
    {
        foreach (UIElement element in elements)
        {
            _regions[kind].Add(element, default);
        }
        return this;
    }

    public TitleBarNonClientRegionHelper Add(NonClientRegionKind kind, IEnumerable<UIElement> elements)
    {
        return Add(kind, [..elements]);
    }

    public TitleBarNonClientRegionHelper Remove(NonClientRegionKind kind, params UIElement[] elements)
    {
        foreach (UIElement element in elements)
        {
            _regions[kind].Remove(element);
        }
        return this;
    }

    public TitleBarNonClientRegionHelper Remove(NonClientRegionKind kind, IEnumerable<UIElement> elements)
    {
        return Remove(kind, [..elements]);
    }

    //public TitleBarNonClientRegionHelper RefreshDragRegions()
    //{
    //    foreach (UIElement element in _dragRegions.Keys)
    //    {
    //        GeneralTransformHelper generalTransformHelper = new(element);
    //        _dragRegions[element] = generalTransformHelper.GetPixelRect(generalTransformHelper.GetRegionRect());
    //    }
    //    return this;
    //}

    public TitleBarNonClientRegionHelper Refresh(params NonClientRegionKind[] kinds)
    {
        foreach (NonClientRegionKind kind in kinds)
        {
            if (!_regions.TryGetValue(kind, out Dictionary<UIElement, RectInt32> rects))
                continue;

            foreach (UIElement element in rects.Keys)
            {
                GeneralTransformHelper generalTransformHelper = new(element);
                _regions[kind][element] = generalTransformHelper.GetPixelRect(generalTransformHelper.GetRegionRect());
            }
        }
        return this;
    }

    //public TitleBarNonClientRegionHelper ClearDragRegions()
    //{
    //    _dragRegions.Clear();
    //    return this;
    //}

    public TitleBarNonClientRegionHelper Clear(params NonClientRegionKind[] kinds)
    {
        foreach (NonClientRegionKind kind in kinds)
        {
            _regions[kind].Clear();
            _nonClientPointerSource.ClearRegionRects(kind);
        }
        return this;
    }

    public void Apply(params NonClientRegionKind[] kinds)
    {
        //_window.AppWindow.TitleBar.SetDragRectangles([.. _dragRegions.Values]);
        foreach (NonClientRegionKind kind in kinds)
        {
            _nonClientPointerSource.SetRegionRects(kind, [.. _regions[kind].Values]);
        }
    }

    //private LRESULT HitTestProcedure(HWND hWnd, uint uMsg, WPARAM wParam, LPARAM lParam, nuint uIdSubclass, nuint dwRefData)
    //{
    //    if (uMsg == 0x0084/*WM_NCHITTEST*/)
    //    {
    //        Point position = new()
    //        {
    //            X = (int) lParam.Value & 0xFFFF,
    //            Y = (int) (lParam.Value >> 16) & 0xFFFF
    //        };
    //        PInvoke.ScreenToClient(hWnd, ref position);
    //        foreach (RectInt32 rect in _regions[NonClientRegionKind.Maximize].Values)
    //        {
    //            if (position.X >= rect.X && position.X <= rect.X + rect.Width && position.Y >= rect.Y && position.Y <= rect.Y + rect.Height)
    //            {
    //                return new LRESULT(9/*HTMAXBUTTON*/);
    //            }
    //        }
    //    }
    //    return PInvoke.DefSubclassProc(hWnd, uMsg, wParam, lParam);
    //}

    private readonly Window _window;

    private readonly HWND _hwnd;

    //private readonly HWND _hwndReunionWindowingCaptionControls;

    //private readonly Dictionary<UIElement, RectInt32> _dragRegions;

    private readonly Dictionary<NonClientRegionKind, Dictionary<UIElement, RectInt32>> _regions = [];

    private InputNonClientPointerSource _nonClientPointerSource;

    //private readonly SUBCLASSPROC _subclassProc;

    //// 通过释放模式实现 IDisposable

    //private bool disposedValue;

    //protected virtual void Dispose(bool disposing)
    //{
    //    if (!disposedValue)
    //    {
    //        if (disposing)
    //        {
    //            // TODO: 释放托管状态(托管对象)
    //            PInvoke.RemoveWindowSubclass(_hwnd, HitTestProcedure, 1);
    //        }

    //        // TODO: 释放未托管的资源(未托管的对象)并重写终结器
    //        // TODO: 将大型字段设置为 null
    //        disposedValue = true;
    //    }
    //}

    //// // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
    //// ~TitleBarNonClientRegionHelper()
    //// {
    ////     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
    ////     Dispose(disposing: false);
    //// }

    //public void Dispose()
    //{
    //    // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
    //    Dispose(disposing: true);
    //    GC.SuppressFinalize(this);
    //}
}
