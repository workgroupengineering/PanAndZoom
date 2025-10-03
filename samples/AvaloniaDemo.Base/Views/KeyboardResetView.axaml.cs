using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AvaloniaDemo.Views;

public partial class KeyboardResetView : UserControl
{
    public KeyboardResetView()
    {
        InitializeComponent();
        DataContext = ZoomBorder;
    }

    private void ResetMatrix_Click(object? sender, RoutedEventArgs e) => ZoomBorder.ResetMatrix();
    private void FitToScreen_Click(object? sender, RoutedEventArgs e) => ZoomBorder.Uniform();
    private void FillToScreen_Click(object? sender, RoutedEventArgs e) => ZoomBorder.Fill();
    private void AutoFit_Click(object? sender, RoutedEventArgs e) => ZoomBorder.AutoFit();
    private void ToggleStretch_Click(object? sender, RoutedEventArgs e) => ZoomBorder.ToggleStretchMode();
}
