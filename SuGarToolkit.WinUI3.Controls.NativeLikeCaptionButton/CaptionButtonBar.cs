using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using SuGarToolkit.WinUI3.SourceGenerators;

using System.ComponentModel;

using Windows.Foundation;

namespace SuGarToolkit.WinUI3.Controls.NativeLikeCaptionButton;

[TemplatePart(Name = nameof(MinimizeButton), Type = typeof(CaptionMinimizeButton))]
[TemplatePart(Name = nameof(MaximizeButton), Type = typeof(CaptionMaximizeButton))]
[TemplatePart(Name = nameof(RestoreButton), Type = typeof(CaptionRestoreButton))]
[TemplatePart(Name = nameof(CloseButton), Type = typeof(CaptionCloseButton))]
[TemplatePart(Name = nameof(MaximizeAndRestoreButtonArea), Type = typeof(UIElement))]
public partial class CaptionButtonBar : Control
{
    public CaptionButtonBar()
    {
        DefaultStyleKey = typeof(CaptionButtonBar);
    }

    [DependencyProperty(DefaultValue = Visibility.Visible)]
    public partial Visibility MinimizeButtonVisibility { get; set; }

    [DependencyProperty(DefaultValue = Visibility.Visible)]
    public partial Visibility MaximizeButtonVisibility { get; set; }

    [DependencyProperty(DefaultValue = Visibility.Visible)]
    public partial Visibility CloseButtonVisibility { get; set; }

    [DependencyProperty(DefaultValue = true)]
    public partial bool IsMinimizeButtonEnabled { get; set; }

    [DependencyProperty(DefaultValue = true)]
    public partial bool IsMaximizeButtonEnabled { get; set; }

    [DependencyProperty(DefaultValue = true)]
    public partial bool IsCloseButtonEnabled { get; set; }

    [DependencyProperty]
    public partial double Spacing { get; set; }

    [DependencyProperty(PropertyChanged = nameof(OnOwnerWindowDependencyPropertyChanged))]
    public partial Window OwnerWindow { get; set; }

    private OverlappedPresenter? OwnerWindowPresenter { get; set; }

    public event TypedEventHandler<CaptionButtonBar, CancelEventArgs>? MinimizeButtonClick;
    public event TypedEventHandler<CaptionButtonBar, CancelEventArgs>? MaximizeButtonClick;
    public event TypedEventHandler<CaptionButtonBar, CancelEventArgs>? RestoreButtonClick;
    public event TypedEventHandler<CaptionButtonBar, CancelEventArgs>? CloseButtonClick;

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
        CancelEventArgs args = new();
        MinimizeButtonClick?.Invoke(this, args);
        if (args.Cancel)
            return;

        OwnerWindowPresenter?.Minimize();
    }

    private void OnMaximizeButtonClicked(object sender, RoutedEventArgs e)
    {
        CancelEventArgs args = new();
        MinimizeButtonClick?.Invoke(this, args);
        if (args.Cancel)
            return;

        OwnerWindowPresenter?.Maximize();
    }

    private void OnRestoreButtonClicked(object sender, RoutedEventArgs e)
    {
        CancelEventArgs args = new();
        MinimizeButtonClick?.Invoke(this, args);
        if (args.Cancel)
            return;

        OwnerWindowPresenter?.Restore();
    }

    private void OnCloseButtonClicked(object sender, RoutedEventArgs e)
    {
        CancelEventArgs args = new();
        MinimizeButtonClick?.Invoke(this, args);
        if (args.Cancel)
            return;

        OwnerWindow?.Close();
    }

    private void OnOwnerWindowChanging(Window oldValue, Window newValue)
    {
        OwnerWindowPresenter = null;
        if (oldValue is not null)
        {
            oldValue.AppWindow.Changed -= OnOwnerAppWindowStateChanged;
            oldValue.Activated -= OnOwnerWindowActivated;
        }

        if (newValue is null || newValue.AppWindow.Presenter.Kind is not AppWindowPresenterKind.Overlapped)
            return;

        OwnerWindowPresenter = (OverlappedPresenter) newValue.AppWindow.Presenter;
        newValue.AppWindow.Changed += OnOwnerAppWindowStateChanged;
        newValue.Activated += OnOwnerWindowActivated;
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
        if (args.DidSizeChange)
        {
            switch (OwnerWindowPresenter!.State)
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

    private static void OnOwnerWindowDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((CaptionButtonBar) d).OnOwnerWindowChanging((Window) e.OldValue, (Window) e.NewValue);
    }
}
