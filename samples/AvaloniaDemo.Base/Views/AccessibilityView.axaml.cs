using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaDemo.ViewModels;

namespace AvaloniaDemo.Views;

public partial class AccessibilityView : UserControl
{
    private AccessibilityViewModel? ViewModel => DataContext as AccessibilityViewModel;

    public AccessibilityView()
    {
        InitializeComponent();

        DataContext = new AccessibilityViewModel();

        Loaded += (s, e) =>
        {
            if (ViewModel != null)
            {
                ViewModel.ZoomBorder = ZoomBorder;
                ViewModel.UpdateDescriptions();

                ZoomBorder.ZoomChanged += (sender, args) => ViewModel.UpdateDescriptions();
            }
        };
    }

    private void UpdateDescriptions_Click(object? sender, RoutedEventArgs e)
    {
        ViewModel?.UpdateDescriptions();
    }
}
