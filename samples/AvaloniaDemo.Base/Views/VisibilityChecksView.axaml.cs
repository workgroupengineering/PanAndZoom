using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaDemo.ViewModels;

namespace AvaloniaDemo.Views;

public partial class VisibilityChecksView : UserControl
{
    private VisibilityChecksViewModel? ViewModel => DataContext as VisibilityChecksViewModel;

    public VisibilityChecksView()
    {
        InitializeComponent();
        DataContext = new VisibilityChecksViewModel();

        Loaded += (s, e) =>
        {
            if (ViewModel != null)
            {
                ViewModel.ZoomBorder = ZoomBorder;
            }
        };
    }

    private void CheckPoint_Click(object? sender, RoutedEventArgs e) => ViewModel?.CheckPointVisibility();
    private void CheckRect_Click(object? sender, RoutedEventArgs e) => ViewModel?.CheckRectVisibility();
    private void GetPortion_Click(object? sender, RoutedEventArgs e) => ViewModel?.GetVisiblePortion();
}
