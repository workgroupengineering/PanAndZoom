using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaDemo.ViewModels;

namespace AvaloniaDemo.Views;

public partial class BoundsCallbacksView : UserControl
{
    private BoundsCallbacksViewModel? ViewModel => DataContext as BoundsCallbacksViewModel;

    public BoundsCallbacksView()
    {
        InitializeComponent();

        DataContext = new BoundsCallbacksViewModel();

        Loaded += (s, e) =>
        {
            if (ViewModel != null)
            {
                ViewModel.ZoomBorder = ZoomBorder;
                ViewModel.UpdateBounds();

                // Subscribe to events
                ZoomBorder.ZoomChanged += (sender, args) =>
                {
                    ViewModel.LogEvent($"ZoomChanged: Zoom=({args.ZoomX:F2}, {args.ZoomY:F2})");
                    ViewModel.UpdateBounds();
                };

                ZoomBorder.PanStarted += (sender, args) =>
                    ViewModel.LogEvent($"PanStarted: Offset=({args.OffsetX:F2}, {args.OffsetY:F2})");

                ZoomBorder.PanEnded += (sender, args) =>
                    ViewModel.LogEvent($"PanEnded: Offset=({args.OffsetX:F2}, {args.OffsetY:F2})");

                ZoomBorder.ZoomStarted += (sender, args) =>
                    ViewModel.LogEvent($"ZoomStarted: Zoom=({args.ZoomX:F2}, {args.ZoomY:F2})");

                ZoomBorder.ZoomEnded += (sender, args) =>
                    ViewModel.LogEvent($"ZoomEnded: Zoom=({args.ZoomX:F2}, {args.ZoomY:F2})");
            }
        };
    }

    private void RefreshBounds_Click(object? sender, RoutedEventArgs e)
    {
        ViewModel?.UpdateBounds();
    }
}
