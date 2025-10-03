using System.Text.Json;
using Avalonia.Controls.PanAndZoom;

namespace AvaloniaDemo.ViewModels;

public class StateSerializationViewModel : ViewModelBase
{
    private string _stateJson = "";
    private ZoomBorderState? _savedState;

    public string StateJson
    {
        get => _stateJson;
        set => SetProperty(ref _stateJson, value);
    }

    public ZoomBorder? ZoomBorder { get; set; }

    public void ExportState()
    {
        if (ZoomBorder == null) return;

        _savedState = ZoomBorder.ExportState();

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        StateJson = JsonSerializer.Serialize(_savedState, options);
    }

    public void ImportState()
    {
        if (ZoomBorder == null || _savedState == null) return;

        ZoomBorder.ImportState(_savedState);
    }

    public void ResetState()
    {
        StateJson = "";
        _savedState = null;
    }
}
