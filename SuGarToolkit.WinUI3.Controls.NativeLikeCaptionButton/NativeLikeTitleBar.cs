using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

using SuGarToolkit.WinUI3.Controls.NativeLikeCaptionButton.Helpers;
using SuGarToolkit.WinUI3.SourceGenerators;

using System.ComponentModel;

using Windows.Foundation;

namespace SuGarToolkit.WinUI3.Controls.NativeLikeCaptionButton;

[TemplatePart(Name = nameof(BackButton), Type = typeof(Button))]
[TemplatePart(Name = nameof(PaneToggleButton), Type = typeof(Button))]
[TemplatePart(Name = nameof(CaptionButtonBar), Type = typeof(CaptionButtonBar))]
[TemplatePart(Name = nameof(HeaderArea), Type = typeof(FrameworkElement))]
[TemplatePart(Name = nameof(CenterArea), Type = typeof(FrameworkElement))]
[TemplatePart(Name = nameof(FooterArea), Type = typeof(FrameworkElement))]
public partial class NativeLikeTitleBar : ContentControl
{
    public NativeLikeTitleBar()
    {
        DefaultStyleKey = typeof(NativeLikeTitleBar);
        Loaded += OnLoaded;
        SizeChanged += OnSizeChanged;
    }

    [DependencyProperty(DefaultValue = Visibility.Visible)]
    public partial Visibility BackButtonVisibility { get; set; }

    [DependencyProperty(DefaultValue = Visibility.Visible)]
    public partial Visibility PaneToggleButtonVisibility { get; set; }

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

    [DependencyProperty(PropertyChanged = nameof(OnOwnerWindowDependencyPropertyChanged))]
    public partial Window? OwnerWindow { get; set; }

    [DependencyProperty]
    public partial object? CustomHeader { get; set; }

    [DependencyProperty]
    public partial object? CustomFooter { get; set; }

    public static bool GetIsHitTestVisibleInTitleBar(DependencyObject d) => (bool) d.GetValue(IsHitTestVisibleInTitleBarProperty);

    public static void SetIsHitTestVisibleInTitleBar(DependencyObject d, bool value) => d.SetValue(IsHitTestVisibleInTitleBarProperty, value);

    public static readonly DependencyProperty IsHitTestVisibleInTitleBarProperty = DependencyProperty.RegisterAttached(
        "IsHitTestVisibleInTitleBar",
        typeof(bool),
        typeof(NativeLikeTitleBar),
        new PropertyMetadata(false)
    );

    public event TypedEventHandler<CaptionButtonBar, CancelEventArgs>? MinimizeButtonClick;
    public event TypedEventHandler<CaptionButtonBar, CancelEventArgs>? MaximizeButtonClick;
    public event TypedEventHandler<CaptionButtonBar, CancelEventArgs>? RestoreButtonClick;
    public event TypedEventHandler<CaptionButtonBar, CancelEventArgs>? CloseButtonClick;

    private CaptionButtonBar CaptionButtonBar { get; set; }
    private Button BackButton { get; set; }
    private Button PaneToggleButton { get; set; }

    private FrameworkElement HeaderArea { get; set; }
    private FrameworkElement CenterArea { get; set; }
    private FrameworkElement FooterArea { get; set; }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        BackButton = (Button) GetTemplateChild(nameof(BackButton));
        PaneToggleButton = (Button) GetTemplateChild(nameof(PaneToggleButton));
        CaptionButtonBar = (CaptionButtonBar) GetTemplateChild(nameof(CaptionButtonBar));

        HeaderArea = (FrameworkElement) GetTemplateChild(nameof(HeaderArea));
        CenterArea = (FrameworkElement) GetTemplateChild(nameof(CenterArea));
        FooterArea = (FrameworkElement) GetTemplateChild(nameof(FooterArea));
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (OwnerWindow is null)
            return;

        nonClientRegionHelper = new TitleBarNonClientRegionHelper(OwnerWindow);
        OwnerWindow!.SetTitleBar(this);
        CollectNonClientRegionPassthroughHitTestVisibleElements(this);
        nonClientRegionHelper
            .Add(NonClientRegionKind.Minimize, CaptionButtonBar.MinimizeButton)
            .Add(NonClientRegionKind.Maximize, CaptionButtonBar.MaximizeAndRestoreButtonArea)
            .Add(NonClientRegionKind.Close, CaptionButtonBar.CloseButton)
            .Add(NonClientRegionKind.Passthrough, CaptionButtonBar, BackButton, PaneToggleButton)
            .Refresh(NonClientRegionKind.Minimize, NonClientRegionKind.Maximize, NonClientRegionKind.Close, NonClientRegionKind.Passthrough)
            .Apply(NonClientRegionKind.Minimize, NonClientRegionKind.Maximize, NonClientRegionKind.Close, NonClientRegionKind.Passthrough);
    }

    private void CollectNonClientRegionPassthroughHitTestVisibleElements(DependencyObject parent)
    {
        if (parent is UIElement element && GetIsHitTestVisibleInTitleBar(parent))
        {
            nonClientRegionHelper.Add(NonClientRegionKind.Passthrough, element);
            return;
        }
        int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < childrenCount; i++)
        {
            DependencyObject dependencyObject = VisualTreeHelper.GetChild(parent, i);
            if (dependencyObject is UIElement child && GetIsHitTestVisibleInTitleBar(child))
            {
                nonClientRegionHelper.Add(NonClientRegionKind.Passthrough, child);
                return;
            }
            CollectNonClientRegionPassthroughHitTestVisibleElements(dependencyObject);
        }
    }

    private bool hasPartSizeChanged;

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (hasPartSizeChanged || nonClientRegionHelper is null)
        {
            hasPartSizeChanged = false;
            return;
        }

        nonClientRegionHelper
            .Refresh(NonClientRegionKind.Minimize, NonClientRegionKind.Maximize, NonClientRegionKind.Close, NonClientRegionKind.Passthrough)
            .Apply(NonClientRegionKind.Minimize, NonClientRegionKind.Maximize, NonClientRegionKind.Close, NonClientRegionKind.Passthrough);
    }

    private void OnPartSizeChanged(object sender, SizeChangedEventArgs e)
    {
        hasPartSizeChanged = true;

        if (nonClientRegionHelper is null)
            return;

        nonClientRegionHelper
            .Refresh(NonClientRegionKind.Minimize, NonClientRegionKind.Maximize, NonClientRegionKind.Close, NonClientRegionKind.Passthrough)
            .Apply(NonClientRegionKind.Minimize, NonClientRegionKind.Maximize, NonClientRegionKind.Close, NonClientRegionKind.Passthrough);
    }

    private void OnOwnerWindowChanging(Window oldValue, Window newValue)
    {
        oldValue?.Activated -= OnOwnerWindowActivated;
        if (newValue is null)
            return;
        newValue.Activated += OnOwnerWindowActivated;
    }

    private void OnOwnerWindowActivated(object sender, WindowActivatedEventArgs args)
    {
        if (args.WindowActivationState is WindowActivationState.Deactivated)
        {
            VisualStateManager.GoToState(this, "Inactive", false);
        }
        else
        {
            VisualStateManager.GoToState(this, "Active", false);
        }
    }

    private static void OnOwnerWindowDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        NativeLikeTitleBar self = (NativeLikeTitleBar) d;
        self.OnOwnerWindowChanging((Window) e.OldValue, (Window) e.NewValue);
    }

    private TitleBarNonClientRegionHelper nonClientRegionHelper;
}
