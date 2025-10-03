using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AvaloniaDemo.Views;

public partial class CenterControlsView : UserControl
{
    public CenterControlsView()
    {
        InitializeComponent();
        DataContext = ZoomBorder;
    }

    private void CenterPoint1_Click(object? sender, RoutedEventArgs e)
    {
        ZoomBorder.CenterOn(new Point(100, 100));
    }

    private void CenterPoint2_Click(object? sender, RoutedEventArgs e)
    {
        ZoomBorder.CenterOn(new Point(300, 200));
    }

    private void CenterPoint3_Click(object? sender, RoutedEventArgs e)
    {
        ZoomBorder.CenterOn(new Point(500, 300));
    }

    private void CenterPointZoom_Click(object? sender, RoutedEventArgs e)
    {
        if (double.TryParse(CenterZoomBox.Text, out var zoom))
        {
            ZoomBorder.CenterOn(new Point(100, 100), zoom);
        }
    }

    private void CenterRect1_Click(object? sender, RoutedEventArgs e)
    {
        ZoomBorder.CenterOn(new Rect(50, 350, 250, 150));
    }

    private void CenterRect2_Click(object? sender, RoutedEventArgs e)
    {
        ZoomBorder.CenterOn(new Rect(450, 350, 300, 200));
    }
}
