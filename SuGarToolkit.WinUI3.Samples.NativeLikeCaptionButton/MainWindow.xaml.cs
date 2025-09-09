using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using SuGarToolkit.WinUI3.Samples.NativeLikeCaptionButton.ViewModels;
using SuGarToolkit.WinUI3.Samples.NativeLikeCaptionButton.Views;

namespace SuGarToolkit.WinUI3.Samples.NativeLikeCaptionButton;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Collapsed;
        ViewModel = new TitleBarElementViewModel();
    }

    private void Frame_Loaded(object sender, RoutedEventArgs e)
    {
        Frame frame = (Frame) sender;
        frame.Navigate(typeof(MainPage), new MainPage.NavigationParameter
        {
            ViewModel = ViewModel,
            OwnerWindow = this
        });
    }

    private TitleBarElementViewModel ViewModel { get; init; }
}
