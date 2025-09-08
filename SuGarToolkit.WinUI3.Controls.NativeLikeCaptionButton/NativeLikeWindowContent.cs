using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using SuGarToolkit.WinUI3.SourceGenerators;

using System.ComponentModel;

using Windows.Foundation;

namespace SuGarToolkit.WinUI3.Controls.NativeLikeCaptionButton;

public partial class NativeLikeWindowContent : ContentControl
{
    public NativeLikeWindowContent()
    {
        DefaultStyleKey = typeof(NativeLikeWindowContent);
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
    public partial Window OwnerWindow { get; set; }

    [DependencyProperty]
    public partial UIElement TitleBarArea { get; set; }

    public event TypedEventHandler<NativeLikeWindowContent, CancelEventArgs>? MinimizeButtonClick;
    public event TypedEventHandler<NativeLikeWindowContent, CancelEventArgs>? MaximizeButtonClick;
    public event TypedEventHandler<NativeLikeWindowContent, CancelEventArgs>? RestoreButtonClick;
    public event TypedEventHandler<NativeLikeWindowContent, CancelEventArgs>? CloseButtonClick;

    private CaptionButtonBar CaptionButtonBar { get; set; }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        CaptionButtonBar = (CaptionButtonBar) GetTemplateChild(nameof(CaptionButtonBar));
        CaptionButtonBar.MinimizeButtonClick += (sender, args) => MinimizeButtonClick?.Invoke(this, args);
        CaptionButtonBar.MaximizeButtonClick += (sender, args) => MaximizeButtonClick?.Invoke(this, args);
        CaptionButtonBar.RestoreButtonClick += (sender, args) => RestoreButtonClick?.Invoke(this, args);
        CaptionButtonBar.CloseButtonClick += (sender, args) => CloseButtonClick?.Invoke(this, args);
    }

    private void SetTitleBarArea()
    {
        if (OwnerWindow is null)
            return;

    }

    private static void OnTitleBarDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        NativeLikeWindowContent self = (NativeLikeWindowContent) d;
        self.SetTitleBarArea();
    }
}
