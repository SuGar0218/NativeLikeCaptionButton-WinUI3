using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

namespace SuGarToolkit.WinUI3.Samples.NativeLikeCaptionButton;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Collapsed;
    }
}
