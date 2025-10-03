using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaDemo.ViewModels;

namespace AvaloniaDemo.Views;

public partial class StateSerializationView : UserControl
{
    private StateSerializationViewModel? ViewModel => DataContext as StateSerializationViewModel;

    public StateSerializationView()
    {
        InitializeComponent();

        DataContext = new StateSerializationViewModel();

        Loaded += (s, e) =>
        {
            if (ViewModel != null)
            {
                ViewModel.ZoomBorder = ZoomBorder;
            }
        };
    }

    private void Export_Click(object? sender, RoutedEventArgs e) => ViewModel?.ExportState();
    private void Import_Click(object? sender, RoutedEventArgs e) => ViewModel?.ImportState();
    private void Reset_Click(object? sender, RoutedEventArgs e) => ViewModel?.ResetState();
}
