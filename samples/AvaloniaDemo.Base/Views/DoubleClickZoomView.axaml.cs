using Avalonia.Controls;

namespace AvaloniaDemo.Views;

public partial class DoubleClickZoomView : UserControl
{
    public DoubleClickZoomView()
    {
        InitializeComponent();
        DataContext = ZoomBorder;
    }
}
