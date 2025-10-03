using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AvaloniaDemo.Views;

public partial class DiscreteZoomView : UserControl
{
    public DiscreteZoomView()
    {
        InitializeComponent();
        DataContext = ZoomBorder;
    }

    private void ZoomNext_Click(object? sender, RoutedEventArgs e)
    {
        var nextLevel = ZoomBorder.GetNextDiscreteZoomLevel();
        ZoomBorder.ZoomToLevel(nextLevel, ZoomBorder.Bounds.Width / 2, ZoomBorder.Bounds.Height / 2);
    }

    private void ZoomPrevious_Click(object? sender, RoutedEventArgs e)
    {
        var prevLevel = ZoomBorder.GetPreviousDiscreteZoomLevel();
        ZoomBorder.ZoomToLevel(prevLevel, ZoomBorder.Bounds.Width / 2, ZoomBorder.Bounds.Height / 2);
    }

    private void Zoom25_Click(object? sender, RoutedEventArgs e) => ZoomBorder.ZoomToLevel(0.25, ZoomBorder.Bounds.Width / 2, ZoomBorder.Bounds.Height / 2);
    private void Zoom50_Click(object? sender, RoutedEventArgs e) => ZoomBorder.ZoomToLevel(0.5, ZoomBorder.Bounds.Width / 2, ZoomBorder.Bounds.Height / 2);
    private void Zoom100_Click(object? sender, RoutedEventArgs e) => ZoomBorder.ZoomToLevel(1.0, ZoomBorder.Bounds.Width / 2, ZoomBorder.Bounds.Height / 2);
    private void Zoom200_Click(object? sender, RoutedEventArgs e) => ZoomBorder.ZoomToLevel(2.0, ZoomBorder.Bounds.Width / 2, ZoomBorder.Bounds.Height / 2);
}
