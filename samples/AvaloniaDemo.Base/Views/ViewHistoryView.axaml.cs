using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using AvaloniaDemo.ViewModels;

namespace AvaloniaDemo.Views;

public partial class ViewHistoryView : UserControl
{
    private ViewHistoryViewModel? ViewModel => DataContext as ViewHistoryViewModel;

    public ViewHistoryView()
    {
        InitializeComponent();

        DataContext = new ViewHistoryViewModel();

        Loaded += (s, e) =>
        {
            if (ViewModel != null)
            {
                ViewModel.ZoomBorder = ZoomBorder;
                ViewModel.UpdateHistoryState();

                ZoomBorder.ZoomChanged += (sender, args) => ViewModel.UpdateHistoryState();
                ZoomBorder.MatrixChanged += (sender, args) => ViewModel.UpdateHistoryState();
            }
        };

        // Add keyboard shortcuts
        ZoomBorder.KeyDown += (s, e) =>
        {
            if (e.KeyModifiers == KeyModifiers.Control)
            {
                if (e.Key == Key.Z)
                {
                    ViewModel?.Undo();
                    e.Handled = true;
                }
                else if (e.Key == Key.Y)
                {
                    ViewModel?.Redo();
                    e.Handled = true;
                }
            }
        };
    }

    private void Undo_Click(object? sender, RoutedEventArgs e) => ViewModel?.Undo();
    private void Redo_Click(object? sender, RoutedEventArgs e) => ViewModel?.Redo();
    private void ClearHistory_Click(object? sender, RoutedEventArgs e) => ViewModel?.ClearHistory();
}
