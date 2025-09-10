using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using SuGarToolkit.WinUI3.SourceGenerators;

using System;
using System.ComponentModel;

using Windows.Foundation;

namespace SuGarToolkit.WinUI3.Controls.NativeLikeCaptionButton;

[TemplatePart(Name = nameof(MinimizeButton), Type = typeof(CaptionMinimizeButton))]
[TemplatePart(Name = nameof(MaximizeButton), Type = typeof(CaptionMaximizeButton))]
[TemplatePart(Name = nameof(RestoreButton), Type = typeof(CaptionRestoreButton))]
[TemplatePart(Name = nameof(CloseButton), Type = typeof(CaptionCloseButton))]
[TemplatePart(Name = nameof(MaximizeAndRestoreButtonArea), Type = typeof(UIElement))]
[TemplateVisualState(GroupName = "WindowStates", Name = "WindowRestored")]
[TemplateVisualState(GroupName = "WindowStates", Name = "WindowMaximized")]
public partial class CaptionButtonBar : Control
{
    public CaptionButtonBar()
    {
        DefaultStyleKey = typeof(CaptionButtonBar);
    }

    [DependencyProperty(DefaultValue = Visibility.Visible, PropertyChanged = nameof(OnMinimizableDependencyPropertyChanged))]
    public partial Visibility MinimizeButtonVisibility { get; set; }

    [DependencyProperty(DefaultValue = Visibility.Visible, PropertyChanged = nameof(OnMaximizableDependencyPropertyChanged))]
    public partial Visibility MaximizeButtonVisibility { get; set; }

    [DependencyProperty(DefaultValue = Visibility.Visible)]
    public partial Visibility CloseButtonVisibility { get; set; }

    [DependencyProperty(DefaultValue = true, PropertyChanged = nameof(OnMinimizableDependencyPropertyChanged))]
    public partial bool IsMinimizeButtonEnabled { get; set; }

    [DependencyProperty(DefaultValue = true, PropertyChanged = nameof(OnMaximizableDependencyPropertyChanged))]
    public partial bool IsMaximizeButtonEnabled { get; set; }

    [DependencyProperty(DefaultValue = true)]
    public partial bool IsCloseButtonEnabled { get; set; }

    [DependencyProperty]
    public partial double Spacing { get; set; }

    [DependencyProperty(PropertyChanged = nameof(OnOwnerWindowDependencyPropertyChanged))]
    public partial Window? OwnerWindow { get; set; }

    //private OverlappedPresenter? OwnerWindowPresenter => OwnerWindow?.AppWindow.Presenter as OverlappedPresenter;

    public event TypedEventHandler<CaptionButtonBar, EventArgs>? MinimizeButtonClick;
    public event TypedEventHandler<CaptionButtonBar, EventArgs>? MaximizeButtonClick;
    public event TypedEventHandler<CaptionButtonBar, EventArgs>? RestoreButtonClick;
    public event TypedEventHandler<CaptionButtonBar, EventArgs>? CloseButtonClick;

    public CaptionMinimizeButton MinimizeButton { get; private set; }
    public CaptionMaximizeButton MaximizeButton { get; private set; }
    public CaptionRestoreButton RestoreButton { get; private set; }
    public CaptionCloseButton CloseButton { get; private set; }
    public UIElement MaximizeAndRestoreButtonArea { get; private set; }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        MinimizeButton = (CaptionMinimizeButton) GetTemplateChild(nameof(MinimizeButton));
        MaximizeButton = (CaptionMaximizeButton) GetTemplateChild(nameof(MaximizeButton));
        RestoreButton = (CaptionRestoreButton) GetTemplateChild(nameof(RestoreButton));
        CloseButton = (CaptionCloseButton) GetTemplateChild(nameof(CloseButton));
        MaximizeAndRestoreButtonArea = (UIElement) GetTemplateChild(nameof(MaximizeAndRestoreButtonArea));

        MinimizeButton.Click += OnMinimizeButtonClicked;
        MaximizeButton.Click += OnMaximizeButtonClicked;
        RestoreButton.Click += OnRestoreButtonClicked;
        CloseButton.Click += OnCloseButtonClicked;
    }

    private void OnMinimizeButtonClicked(object sender, RoutedEventArgs e)
    {
        MinimizeButtonClick?.Invoke(this, EventArgs.Empty);
        if (OwnerWindow?.AppWindow.Presenter.Kind is AppWindowPresenterKind.Overlapped)
        {
            ((OverlappedPresenter) OwnerWindow.AppWindow.Presenter).Minimize();
        }
    }

    private void OnMaximizeButtonClicked(object sender, RoutedEventArgs e)
    {
        MinimizeButtonClick?.Invoke(this, EventArgs.Empty);
        if (OwnerWindow?.AppWindow.Presenter.Kind is AppWindowPresenterKind.Overlapped)
        {
            ((OverlappedPresenter) OwnerWindow.AppWindow.Presenter).Maximize();
        }
    }

    private void OnRestoreButtonClicked(object sender, RoutedEventArgs e)
    {
        MinimizeButtonClick?.Invoke(this, EventArgs.Empty);

        if (OwnerWindow is null)
            return;

        if (OwnerWindow.AppWindow.Presenter.Kind is AppWindowPresenterKind.Overlapped)
        {
            ((OverlappedPresenter) OwnerWindow.AppWindow.Presenter).Restore();
        }
        else if (OwnerWindow.AppWindow.Presenter.Kind is AppWindowPresenterKind.FullScreen)
        {
            OwnerWindow.AppWindow.SetPresenter(AppWindowPresenterKind.Overlapped);
            ((OverlappedPresenter) OwnerWindow.AppWindow.Presenter).Restore();
        }
    }

    private void OnCloseButtonClicked(object sender, RoutedEventArgs e)
    {
        MinimizeButtonClick?.Invoke(this, EventArgs.Empty);
        OwnerWindow?.Close();
    }

    private void OnOwnerWindowChanging(Window oldValue, Window newValue)
    {
        if (oldValue is not null)
        {
            oldValue.Activated -= OnOwnerWindowActivated;
            oldValue.AppWindow.Changed -= OnOwnerAppWindowStateChanged;
        }

        if (newValue is not null)
        {
            newValue.Activated += OnOwnerWindowActivated;
            newValue.AppWindow.Changed += OnOwnerAppWindowStateChanged;
        }
    }

    private void OnOwnerWindowActivated(object sender, WindowActivatedEventArgs args)
    {
        if (args.WindowActivationState is WindowActivationState.Deactivated)
        {
            MinimizeButton.IsActive = false;
            MaximizeButton.IsActive = false;
            RestoreButton.IsActive = false;
            CloseButton.IsActive = false;
        }
        else
        {
            MinimizeButton.IsActive = true;
            MaximizeButton.IsActive = true;
            RestoreButton.IsActive = true;
            CloseButton.IsActive = true;
        }
    }

    private void OnOwnerAppWindowStateChanged(AppWindow sender, AppWindowChangedEventArgs args)
    {
        if (args.DidPresenterChange)
        {
            switch (sender.Presenter.Kind)
            {
                case AppWindowPresenterKind.Default:
                    break;
                case AppWindowPresenterKind.CompactOverlay:
                    break;
                case AppWindowPresenterKind.FullScreen:
                    VisualStateManager.GoToState(this, "WindowMaximized", false);
                    break;
                case AppWindowPresenterKind.Overlapped:
                    switch (((OverlappedPresenter) sender.Presenter).State)
                    {
                        case OverlappedPresenterState.Maximized:
                            VisualStateManager.GoToState(this, "WindowMaximized", false);
                            break;
                        case OverlappedPresenterState.Minimized:
                            break;
                        case OverlappedPresenterState.Restored:
                            VisualStateManager.GoToState(this, "WindowRestored", false);
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
        else
        {
            if (args.DidSizeChange && sender.Presenter.Kind is AppWindowPresenterKind.Overlapped)
            {
                switch (((OverlappedPresenter) sender.Presenter).State)
                {
                    case OverlappedPresenterState.Maximized:
                        VisualStateManager.GoToState(this, "WindowMaximized", false);
                        break;
                    case OverlappedPresenterState.Restored:
                        VisualStateManager.GoToState(this, "WindowRestored", false);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private static void OnOwnerWindowDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((CaptionButtonBar) d).OnOwnerWindowChanging((Window) e.OldValue, (Window) e.NewValue);
    }

    private static void OnMinimizableDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        CaptionButtonBar self = (CaptionButtonBar) d;
        if (self.OwnerWindow?.AppWindow.Presenter.Kind is not AppWindowPresenterKind.Overlapped)
            return;
        ((OverlappedPresenter) self.OwnerWindow.AppWindow.Presenter).IsMinimizable = self.IsMinimizeButtonEnabled && self.MinimizeButtonVisibility is Visibility.Visible;
    }

    private static void OnMaximizableDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        CaptionButtonBar self = (CaptionButtonBar) d;
        if (self.OwnerWindow?.AppWindow.Presenter.Kind is not AppWindowPresenterKind.Overlapped)
            return;
        ((OverlappedPresenter) self.OwnerWindow.AppWindow.Presenter).IsMaximizable = self.IsMaximizeButtonEnabled && self.MaximizeButtonVisibility is Visibility.Visible;
    }
}
