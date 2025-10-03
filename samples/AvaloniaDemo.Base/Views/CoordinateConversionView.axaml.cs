using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaDemo.ViewModels;

namespace AvaloniaDemo.Views;

public partial class CoordinateConversionView : UserControl
{
    private CoordinateConversionViewModel? ViewModel => DataContext as CoordinateConversionViewModel;

    public CoordinateConversionView()
    {
        InitializeComponent();

        DataContext = new CoordinateConversionViewModel();

        Loaded += (s, e) =>
        {
            if (ViewModel != null)
            {
                ViewModel.ZoomBorder = ZoomBorder;
                ViewModel.UpdateConversions();

                ZoomBorder.ZoomChanged += (sender, args) => ViewModel.UpdateConversions();
            }
        };
    }

    private void UpdateConversions_Click(object? sender, RoutedEventArgs e)
    {
        ViewModel?.UpdateConversions();
    }
}
