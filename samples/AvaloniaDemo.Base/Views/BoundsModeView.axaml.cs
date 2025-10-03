using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Interactivity;

namespace AvaloniaDemo.Views;

public partial class BoundsModeView : UserControl
{
    public BoundsModeView()
    {
        InitializeComponent();
        DataContext = ZoomBorder;
    }

    private void BoundsMode_Changed(object? sender, SelectionChangedEventArgs e)
    {
        if (ZoomBorder == null) return;

        if (sender is ComboBox cb && cb.SelectedItem is ComboBoxItem item && item.Tag is string tag)
        {
            ZoomBorder.BoundsMode = tag switch
            {
                "Unrestricted" => ContentBoundsMode.Unrestricted,
                "KeepContentVisible" => ContentBoundsMode.KeepContentVisible,
                "FillViewport" => ContentBoundsMode.FillViewport,
                "KeepCentered" => ContentBoundsMode.KeepCentered,
                _ => ContentBoundsMode.Unrestricted
            };
        }
    }
}
