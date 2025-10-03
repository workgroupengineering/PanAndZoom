using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaDemo.ViewModels;

namespace AvaloniaDemo.Views;

public partial class GridSnapView : UserControl
{
    private GridSnapViewModel? ViewModel => DataContext as GridSnapViewModel;

    public GridSnapView()
    {
        InitializeComponent();

        DataContext = new GridSnapViewModel();

        Loaded += (s, e) =>
        {
            if (ViewModel != null)
            {
                ViewModel.ZoomBorder = ZoomBorder;
                ViewModel.UpdateSnapping();
            }
        };
    }

    private void UpdateSnapping_Click(object? sender, RoutedEventArgs e)
    {
        ViewModel?.UpdateSnapping();
    }

    private void UpdateSnapping_TextChanged(object? sender, TextChangedEventArgs e)
    {
        ViewModel?.UpdateSnapping();
    }
}
