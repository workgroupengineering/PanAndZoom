using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AvaloniaDemo.Views;

public partial class AdvancedZoomView : UserControl
{
    public AdvancedZoomView()
    {
        InitializeComponent();
        DataContext = ZoomBorder;
    }

    private void ZoomIn_Click(object? sender, RoutedEventArgs e)
    {
        ZoomBorder.ZoomIn();
    }

    private void ZoomOut_Click(object? sender, RoutedEventArgs e)
    {
        ZoomBorder.ZoomOut();
    }

    private void SetAbsoluteZoom_Click(object? sender, RoutedEventArgs e)
    {
        var centerX = ZoomBorder.Bounds.Width / 2;
        var centerY = ZoomBorder.Bounds.Height / 2;
        ZoomBorder.Zoom(AbsoluteZoomSlider.Value, centerX, centerY);
    }

    private void ZoomToRatio_Click(object? sender, RoutedEventArgs e)
    {
        if (double.TryParse(ZoomRatioBox.Text, out var ratio))
        {
            var centerX = ZoomBorder.Bounds.Width / 2;
            var centerY = ZoomBorder.Bounds.Height / 2;
            ZoomBorder.ZoomTo(ratio, centerX, centerY);
        }
    }

    private void ZoomDelta_Click(object? sender, RoutedEventArgs e)
    {
        if (double.TryParse(ZoomDeltaBox.Text, out var delta))
        {
            var centerX = ZoomBorder.Bounds.Width / 2;
            var centerY = ZoomBorder.Bounds.Height / 2;
            ZoomBorder.ZoomDeltaTo(delta, centerX, centerY);
        }
    }
}
