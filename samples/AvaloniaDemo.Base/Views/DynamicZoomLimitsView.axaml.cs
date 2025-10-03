using Avalonia.Controls;

namespace AvaloniaDemo.Views;

public partial class DynamicZoomLimitsView : UserControl
{
    public DynamicZoomLimitsView()
    {
        InitializeComponent();
        DataContext = ZoomBorder;
    }
}
