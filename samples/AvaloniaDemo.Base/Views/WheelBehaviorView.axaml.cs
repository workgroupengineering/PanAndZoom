using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Interactivity;

namespace AvaloniaDemo.Views;

public partial class WheelBehaviorView : UserControl
{
    public WheelBehaviorView()
    {
        InitializeComponent();
        DataContext = ZoomBorder;
    }

    private void WheelBehavior_Changed(object? sender, SelectionChangedEventArgs e)
    {
        if (ZoomBorder == null) return;

        if (sender is ComboBox cb && cb.SelectedItem is ComboBoxItem item && item.Tag is string tag)
        {
            ZoomBorder.WheelBehavior = tag switch
            {
                "Zoom" => WheelBehaviorMode.Zoom,
                "PanVertical" => WheelBehaviorMode.PanVertical,
                "PanHorizontal" => WheelBehaviorMode.PanHorizontal,
                "None" => WheelBehaviorMode.None,
                _ => WheelBehaviorMode.Zoom
            };
        }
    }

    private void WheelCtrl_Changed(object? sender, SelectionChangedEventArgs e)
    {
        if (ZoomBorder == null) return;

        if (sender is ComboBox cb && cb.SelectedItem is ComboBoxItem item && item.Tag is string tag)
        {
            ZoomBorder.WheelWithCtrl = tag switch
            {
                "Zoom" => WheelBehaviorMode.Zoom,
                "PanVertical" => WheelBehaviorMode.PanVertical,
                "PanHorizontal" => WheelBehaviorMode.PanHorizontal,
                _ => WheelBehaviorMode.Zoom
            };
        }
    }

    private void WheelShift_Changed(object? sender, SelectionChangedEventArgs e)
    {
        if (ZoomBorder == null) return;

        if (sender is ComboBox cb && cb.SelectedItem is ComboBoxItem item && item.Tag is string tag)
        {
            ZoomBorder.WheelWithShift = tag switch
            {
                "Zoom" => WheelBehaviorMode.Zoom,
                "PanVertical" => WheelBehaviorMode.PanVertical,
                "PanHorizontal" => WheelBehaviorMode.PanHorizontal,
                _ => WheelBehaviorMode.PanHorizontal
            };
        }
    }
}
