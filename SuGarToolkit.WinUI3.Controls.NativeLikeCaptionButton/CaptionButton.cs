using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

using SuGarToolkit.WinUI3.SourceGenerators;

namespace SuGarToolkit.WinUI3.Controls.NativeLikeCaptionButton;

[TemplateVisualState(GroupName = "CommonStates", Name = "PointerExit")]
[TemplateVisualState(GroupName = "CommonStates", Name = "PointerOver")]
[TemplateVisualState(GroupName = "CommonStates", Name = "PointerPressed")]
public partial class CaptionButton : Button
{
    public CaptionButton()
    {
        DefaultStyleKey = typeof(CaptionButton);
    }

    [DependencyProperty(DefaultValue = true, PropertyChanged = nameof(OnIsActiveDependencyPropertyChanged))]
    public partial bool IsActive { get; set; }

    private CaptionButtonBar CaptionButtonBar { get; set; }

    protected override void OnPointerEntered(PointerRoutedEventArgs e)
    {
        base.OnPointerEntered(e);
        VisualStateManager.GoToState(this, "PointerOver", false);
    }

    protected override void OnPointerExited(PointerRoutedEventArgs e)
    {
        base.OnPointerExited(e);
        VisualStateManager.GoToState(this, "PointerExit", false);
    }

    protected override void OnPointerPressed(PointerRoutedEventArgs e)
    {
        base.OnPointerPressed(e);
        VisualStateManager.GoToState(this, "PointerPressed", false);
    }

    private static void OnIsActiveDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if ((bool) e.NewValue)
        {
            VisualStateManager.GoToState((CaptionButton) d, "Active", false);
        }
        else
        {
            VisualStateManager.GoToState((CaptionButton) d, "Inactive", false);
        }
    }
}
