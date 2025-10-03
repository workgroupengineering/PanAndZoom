using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Interactivity;

namespace AvaloniaDemo.Views;

public partial class ResizeBehaviorView : UserControl
{
    public ResizeBehaviorView()
    {
        InitializeComponent();
        DataContext = ZoomBorder;
    }

    private void ResizeBehavior_Changed(object? sender, SelectionChangedEventArgs e)
    {
        if (ZoomBorder == null) return;

        if (sender is not ComboBox comboBox || comboBox.SelectedItem is not ComboBoxItem item)
            return;

        var tag = item.Tag?.ToString();
        ZoomBorder.ResizeBehavior = tag switch
        {
            "None" => ResizeBehaviorMode.None,
            "MaintainCenter" => ResizeBehaviorMode.MaintainCenter,
            "MaintainTopLeft" => ResizeBehaviorMode.MaintainTopLeft,
            "MaintainZoom" => ResizeBehaviorMode.MaintainZoom,
            "ReapplyStretch" => ResizeBehaviorMode.ReapplyStretch,
            _ => ResizeBehaviorMode.MaintainZoom
        };
    }
}
