using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaDemo.ViewModels;

namespace AvaloniaDemo.Views;

public partial class RotationView : UserControl
{
    private RotationViewModel? ViewModel => DataContext as RotationViewModel;

    public RotationView()
    {
        InitializeComponent();

        DataContext = new RotationViewModel();

        Loaded += (s, e) =>
        {
            if (ViewModel != null)
            {
                ViewModel.ZoomBorder = ZoomBorder;
                ViewModel.UpdateRotation();

                ZoomBorder.PropertyChanged += (sender, args) =>
                {
                    if (args.Property.Name == nameof(ZoomBorder.Rotation))
                    {
                        ViewModel.UpdateRotation();
                    }
                };
            }
        };
    }

    private void Rotate15_Click(object? sender, RoutedEventArgs e) => ViewModel?.RotateBy(15);
    private void RotateMinus15_Click(object? sender, RoutedEventArgs e) => ViewModel?.RotateBy(-15);
    private void Rotate45_Click(object? sender, RoutedEventArgs e) => ViewModel?.RotateBy(45);
    private void RotateMinus45_Click(object? sender, RoutedEventArgs e) => ViewModel?.RotateBy(-45);
    private void Snap_Click(object? sender, RoutedEventArgs e) => ViewModel?.Snap();
    private void Reset_Click(object? sender, RoutedEventArgs e) => ViewModel?.Reset();
    private void UpdateRotation_Click(object? sender, RoutedEventArgs e) => ViewModel?.UpdateRotation();
}
