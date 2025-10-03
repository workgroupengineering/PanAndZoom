using Avalonia.Controls.PanAndZoom;

namespace AvaloniaDemo.ViewModels;

public class RotationViewModel : ViewModelBase
{
    private double _currentRotation;

    public double CurrentRotation
    {
        get => _currentRotation;
        set => SetProperty(ref _currentRotation, value);
    }

    public ZoomBorder? ZoomBorder { get; set; }

    public void UpdateRotation()
    {
        if (ZoomBorder == null) return;
        CurrentRotation = ZoomBorder.Rotation;
    }

    public void RotateBy(double degrees)
    {
        ZoomBorder?.Rotate(degrees);
        UpdateRotation();
    }

    public void Reset()
    {
        ZoomBorder?.ResetRotation();
        UpdateRotation();
    }

    public void Snap()
    {
        ZoomBorder?.SnapRotation();
        UpdateRotation();
    }
}
