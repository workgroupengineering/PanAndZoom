using Avalonia.Controls.PanAndZoom;

namespace AvaloniaDemo.ViewModels;

public class ViewHistoryViewModel : ViewModelBase
{
    private bool _canUndo;
    private bool _canRedo;
    private string _historyInfo = "";

    public bool CanUndo
    {
        get => _canUndo;
        set => SetProperty(ref _canUndo, value);
    }

    public bool CanRedo
    {
        get => _canRedo;
        set => SetProperty(ref _canRedo, value);
    }

    public string HistoryInfo
    {
        get => _historyInfo;
        set => SetProperty(ref _historyInfo, value);
    }

    public ZoomBorder? ZoomBorder { get; set; }

    public void UpdateHistoryState()
    {
        if (ZoomBorder == null) return;

        CanUndo = ZoomBorder.CanNavigateBack;
        CanRedo = ZoomBorder.CanNavigateForward;

        HistoryInfo = $"History Enabled: {ZoomBorder.EnableViewHistory}\n" +
                     $"Can Navigate Back: {CanUndo}\n" +
                     $"Can Navigate Forward: {CanRedo}";
    }

    public void Undo()
    {
        ZoomBorder?.NavigateBack();
        UpdateHistoryState();
    }

    public void Redo()
    {
        ZoomBorder?.NavigateForward();
        UpdateHistoryState();
    }

    public void ClearHistory()
    {
        ZoomBorder?.ClearViewHistory();
        UpdateHistoryState();
    }
}
