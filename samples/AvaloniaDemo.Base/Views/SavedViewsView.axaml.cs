using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaDemo.ViewModels;

namespace AvaloniaDemo.Views;

public partial class SavedViewsView : UserControl
{
    private SavedViewsViewModel? ViewModel => DataContext as SavedViewsViewModel;

    public SavedViewsView()
    {
        InitializeComponent();

        DataContext = new SavedViewsViewModel();

        Loaded += (s, e) =>
        {
            if (ViewModel != null)
            {
                ViewModel.ZoomBorder = ZoomBorder;
                ViewModel.RefreshViewList();
            }
        };
    }

    private void SaveView_Click(object? sender, RoutedEventArgs e) => ViewModel?.SaveView();
    private void RestoreView_Click(object? sender, RoutedEventArgs e) => ViewModel?.RestoreView();
    private void DeleteView_Click(object? sender, RoutedEventArgs e) => ViewModel?.DeleteView();
    private void ClearAllViews_Click(object? sender, RoutedEventArgs e) => ViewModel?.ClearAllViews();
}
