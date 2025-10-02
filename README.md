# PanAndZoom

[![Gitter](https://badges.gitter.im/wieslawsoltes/PanAndZoom.svg)](https://gitter.im/wieslawsoltes/PanAndZoom?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)

[![Build Status](https://dev.azure.com/wieslawsoltes/GitHub/_apis/build/status/wieslawsoltes.PanAndZoom?branchName=master)](https://dev.azure.com/wieslawsoltes/GitHub/_build/latest?definitionId=98&branchName=master)
[![CI](https://github.com/wieslawsoltes/PanAndZoom/actions/workflows/build.yml/badge.svg)](https://github.com/wieslawsoltes/PanAndZoom/actions/workflows/build.yml)

[![NuGet](https://img.shields.io/nuget/v/PanAndZoom.svg)](https://www.nuget.org/packages/PanAndZoom)
[![NuGet](https://img.shields.io/nuget/dt/PanAndZoom.svg)](https://www.nuget.org/packages/PanAndZoom)
[![MyGet](https://img.shields.io/myget/panandzoom-nightly/vpre/PanAndZoom.svg?label=myget)](https://www.myget.org/gallery/panandzoom-nightly) 

PanAndZoom control for Avalonia

<a href='https://youtu.be/BFLF1WPZWCQ' target='_blank'>![](images/PanAndZoom.png)<a/>

## NuGet

PanAndZoom is delivered as a NuGet package.

You can find the NuGet packages here for [Avalonia](https://www.nuget.org/packages/PanAndZoom/) or by using nightly build feed:
* Add `https://www.myget.org/F/panandzoom-nightly/api/v2` to your package sources
* Alternative nightly build feed `https://pkgs.dev.azure.com/wieslawsoltes/GitHub/_packaging/Nightly/nuget/v3/index.json`
* Update your package using `PanAndZoom` feed

You can install the package for `Avalonia` based projects like this:

`Install-Package PanAndZoom -Pre`

### Package Sources

* https://api.nuget.org/v3/index.json
* https://www.myget.org/F/panandzoom-nightly/api/v2

## Resources

* [GitHub source code repository.](https://github.com/wieslawsoltes/PanAndZoom)

## Using PanAndZoom

`MainWindow.xaml`
```XAML
<Window x:Class="AvaloniaDemo.MainWindow"
        xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:paz="using:Avalonia.Controls.PanAndZoom"
        WindowStartupLocation="CenterScreen" UseLayoutRounding="True"
        Title="PanAndZoom" Height="640" Width="640">
    <Grid RowDefinitions="Auto,12,Auto,12,*,12" ColumnDefinitions="50,*,50">
        <StackPanel Orientation="Vertical"
                    HorizontalAlignment="Center" Grid.Row="0" Grid.Column="1">
            <TextBlock Text="F - Fill"/>
            <TextBlock Text="U - Uniform"/>
            <TextBlock Text="R - Reset"/>
            <TextBlock Text="T - Toggle Stretch Mode"/>
            <TextBlock Text="Mouse Wheel - Zoom to Point"/>
            <TextBlock Text="Mouse Left Button Down - Pan"/>
        </StackPanel>
        <StackPanel Orientation="Horizontal"
                    HorizontalAlignment="Center" Grid.Row="2" Grid.Column="1">
            <TextBlock Text="PanButton:" VerticalAlignment="Center"/>
            <ComboBox Items="{x:Static paz:ZoomBorder.ButtonNames}"
                      SelectedItem="{Binding #ZoomBorder.PanButton, Mode=TwoWay}"
                      Margin="2">
            </ComboBox>
            <TextBlock Text="Stretch:" VerticalAlignment="Center"/>
            <ComboBox Items="{x:Static paz:ZoomBorder.StretchModes}"
                      SelectedItem="{Binding #ZoomBorder.Stretch, Mode=TwoWay}"
                      Margin="2">
            </ComboBox>
            <TextBlock Text="ZoomSpeed:" VerticalAlignment="Center"/>
            <TextBox Text="{Binding #ZoomBorder.ZoomSpeed, Mode=TwoWay}"
                     TextAlignment="Center" Width="50" Margin="2"/>
            <CheckBox IsChecked="{Binding #ZoomBorder.EnablePan}"
                      Content="EnablePan" VerticalAlignment="Center"/>
            <CheckBox IsChecked="{Binding #ZoomBorder.EnableZoom}"
                      Content="EnableZoom" VerticalAlignment="Center"/>
        </StackPanel>
        <ScrollViewer Grid.Row="4" Grid.Column="1"
                      VerticalScrollBarVisibility="Auto"
                      HorizontalScrollBarVisibility="Auto">
            <paz:ZoomBorder Name="ZoomBorder" Stretch="None" ZoomSpeed="1.2"
                            Background="SlateBlue" ClipToBounds="True" Focusable="True"
                            VerticalAlignment="Stretch" HorizontalAlignment="Stretch">
                <Canvas Background="LightGray" Width="300" Height="300">
                    <Rectangle Canvas.Left="100" Canvas.Top="100" Width="50" Height="50" Fill="Red"/>
                    <StackPanel Canvas.Left="100" Canvas.Top="200">
                        <TextBlock Text="Text1" Width="100" Background="Red" Foreground="WhiteSmoke"/>
                        <TextBlock Text="Text2" Width="100" Background="Red" Foreground="WhiteSmoke"/>
                    </StackPanel>
                </Canvas>
            </paz:ZoomBorder>  
        </ScrollViewer>
    </Grid> 
</Window>
```

`MainWindow.xaml.cs`
```C#
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace AvaloniaDemo
{
    public class MainWindow : Window
    {
        private readonly ZoomBorder? _zoomBorder;

        public MainWindow()
        {
            this.InitializeComponent();
            this.AttachDevTools();

            _zoomBorder = this.Find<ZoomBorder>("ZoomBorder");
            if (_zoomBorder != null)
            {
                _zoomBorder.KeyDown += ZoomBorder_KeyDown;
                
                _zoomBorder.ZoomChanged += ZoomBorder_ZoomChanged;
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void ZoomBorder_KeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F:
                    _zoomBorder?.Fill();
                    break;
                case Key.U:
                    _zoomBorder?.Uniform();
                    break;
                case Key.R:
                    _zoomBorder?.ResetMatrix();
                    break;
                case Key.T:
                    _zoomBorder?.ToggleStretchMode();
                    _zoomBorder?.AutoFit();
                    break;
            }
        }

        private void ZoomBorder_ZoomChanged(object sender, ZoomChangedEventArgs e)
        {
            Debug.WriteLine($"[ZoomChanged] {e.ZoomX} {e.ZoomY} {e.OffsetX} {e.OffsetY}");
        }
    }
}
```

### Getting zoom ratio

To get current zoom ratio use `ZoomX` and `ZoomY` properties. 

### Getting pan offset

To get current pan offset use `OffsetX` and `OffsetY` properties. 

### Constrain zoom ratio

To constrain zoom ratio use `MinZoomX`, `MaxZoomX`, `MinZoomY` and `MaxZoomY` properties. 

### Constrain pan offset

To constrain pan offset use `MinOffsetX`, `MaxOffsetX`, `MinOffsetY` and `MaxOffsetY` properties. 

### Enable or disable constrains

To enable or disable constrains use `EnableConstrains` flag.

## Advanced Features

### Animation Support

Enable smooth animations for zoom and pan operations:

```csharp
zoomBorder.EnableAnimations = true;
zoomBorder.AnimationDuration = TimeSpan.FromMilliseconds(300);
```

### Double-Click to Zoom

Configure double-click zoom behavior:

```csharp
zoomBorder.EnableDoubleClickZoom = true;
zoomBorder.DoubleClickZoomMode = DoubleClickZoomMode.ZoomInOut; // ZoomIn, ZoomOut, ZoomInOut, ZoomToFit, None
zoomBorder.DoubleClickZoomFactor = 2.0;
```

### Content Bounds Restriction

Prevent panning beyond content boundaries:

```csharp
zoomBorder.BoundsMode = ContentBoundsMode.KeepContentVisible; // Unrestricted, KeepContentVisible, FillViewport, KeepCentered, Custom
zoomBorder.BoundsPadding = new Thickness(10);
zoomBorder.MinimumVisibleContentPercentage = 0.1; // 10% of content must remain visible
```

**Bounds Modes:**
- **Unrestricted**: No bounds checking (default)
- **KeepContentVisible**: Ensures minimum percentage of content stays visible
- **FillViewport**: Centers small content and prevents empty space for large content
- **KeepCentered**: Always keeps content centered
- **Custom**: Override `GetContentBounds()` and `ValidateTransform()` for custom logic

### Resize Behavior

Control how the view adjusts when the control is resized:

```csharp
zoomBorder.ResizeBehavior = ResizeBehaviorMode.MaintainCenter; // None, MaintainCenter, MaintainTopLeft, MaintainZoom, ReapplyStretch, Custom
```

**Resize Modes:**
- **None**: No special handling (default)
- **MaintainCenter**: Keeps the center point stable during resize
- **MaintainTopLeft**: Keeps the top-left position stable
- **MaintainZoom**: Maintains zoom level and adjusts position proportionally
- **ReapplyStretch**: Reapplies the current stretch mode (calls AutoFit)
- **Custom**: Override `OnResized(Size oldSize, Size newSize)` for custom logic

### Configurable Wheel Behavior

Customize mouse wheel behavior with modifier key support:

```csharp
zoomBorder.WheelBehavior = WheelBehaviorMode.Zoom; // Zoom, PanVertical, PanHorizontal, None
zoomBorder.WheelWithCtrl = WheelBehaviorMode.Zoom;
zoomBorder.WheelWithShift = WheelBehaviorMode.PanHorizontal;
zoomBorder.WheelZoomSensitivity = 1.0;
zoomBorder.WheelPanSensitivity = 1.0;
```

**Wheel Modes:**
- **Zoom**: Zoom in/out (default)
- **PanVertical**: Pan up/down
- **PanHorizontal**: Pan left/right
- **None**: Disable wheel

**Example:** By default, `Shift`+`Wheel` pans horizontally while regular wheel zooms.

### Virtual Methods for Custom Behavior

Override these methods to implement custom logic:

```csharp
public class CustomZoomBorder : ZoomBorder
{
    protected override Rect GetContentBounds()
    {
        // Return custom content bounds for Custom BoundsMode
        return base.GetContentBounds();
    }

    protected override bool ValidateTransform(Matrix newMatrix)
    {
        // Validate proposed transform matrix
        return base.ValidateTransform(newMatrix);
    }

    protected override void OnResized(Size oldSize, Size newSize)
    {
        // Custom resize handling for Custom ResizeBehavior
        base.OnResized(oldSize, newSize);
    }
}
```

## License

PanAndZoom is licensed under the [MIT license](LICENSE.TXT).
