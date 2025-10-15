using SuGarToolkit.WinUI3.SourceGenerators;

namespace SuGarToolkit.WinUI3.Controls.NativeLikeCaptionButton;

public partial class CaptionMaximizeButton : CaptionButton
{
    public CaptionMaximizeButton()
    {
        InitializeComponent();
    }

    [DependencyProperty]
    public partial bool IsRestoreButton { get; set; }
}
