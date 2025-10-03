using Avalonia.Controls;

namespace AvaloniaDemo.Views;

public partial class InertiaGesturesView : UserControl
{
    public InertiaGesturesView()
    {
        InitializeComponent();
        DataContext = ZoomBorder;
    }
}
