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

### Keyboard Navigation

Navigate and zoom using keyboard shortcuts:

```csharp
zoomBorder.EnableKeyboardNavigation = true;
zoomBorder.KeyboardPanStep = 50.0;
zoomBorder.KeyboardZoomStep = 1.1;
```

**Built-in Keyboard Shortcuts:**
- **Arrow Keys**: Pan in the respective direction
- **`+` / `=`**: Zoom in
- **`-`**: Zoom out
- **`Ctrl` + `0`**: Reset view to identity matrix
- **`Home`**: Fit content to viewport
- **`Ctrl` + `Left`**: Navigate back in view history
- **`Ctrl` + `Right`**: Navigate forward in view history

### View History (Undo/Redo)

Track navigation history with undo/redo support:

```csharp
zoomBorder.EnableViewHistory = true;
zoomBorder.ViewHistorySize = 50; // Maximum history entries

// Navigate through history
if (zoomBorder.CanNavigateBack)
    zoomBorder.NavigateBack();

if (zoomBorder.CanNavigateForward)
    zoomBorder.NavigateForward();

// Clear history
zoomBorder.ClearViewHistory();

// Listen for history changes
zoomBorder.ViewHistoryChanged += (sender, args) =>
{
    // Update UI state for back/forward buttons
};
```

### Center On Methods

Programmatically center the viewport on specific points, rectangles, or elements:

```csharp
// Center on a point in content coordinates
zoomBorder.CenterOn(new Point(100, 100));

// Center on a point with specific zoom
zoomBorder.CenterOn(new Point(100, 100), zoom: 2.0);

// Center on a rectangle (automatically calculates appropriate zoom)
zoomBorder.CenterOn(new Rect(50, 50, 200, 150));

// Center on a control element
var targetControl = this.FindControl<Control>("MyElement");
zoomBorder.CenterOn(targetControl);

// Add padding when centering
zoomBorder.CenterPadding = new Thickness(20);
```

### Coordinate System Helpers

Convert between viewport and content coordinate systems:

```csharp
// Point conversions
Point contentPoint = zoomBorder.ViewportToContent(viewportPoint);
Point viewportPoint = zoomBorder.ContentToViewport(contentPoint);

// Rectangle conversions
Rect contentRect = zoomBorder.ViewportToContent(viewportRect);
Rect viewportRect = zoomBorder.ContentToViewport(contentRect);

// Vector conversions
Vector contentVector = zoomBorder.ScreenToContent(screenVector);
Vector screenVector = zoomBorder.ContentToScreen(contentVector);

// Size conversions
Size contentSize = zoomBorder.ScreenToContent(screenSize);
Size screenSize = zoomBorder.ContentToScreen(contentSize);

// Get transformation matrices
Matrix contentToScreen = zoomBorder.GetContentToScreenMatrix();
Matrix screenToContent = zoomBorder.GetScreenToContentMatrix();

// Get bounds
Rect visibleContent = zoomBorder.GetVisibleContentBounds();
Rect viewport = zoomBorder.GetViewportBounds();
```

### MVVM Command Support

Use built-in ICommand implementations for MVVM scenarios:

```xml
<paz:ZoomBorder Name="ZoomBorder">
    <!-- Your content -->
</paz:ZoomBorder>

<StackPanel>
    <Button Content="Zoom In" Command="{Binding #ZoomBorder.ZoomInCommand}"/>
    <Button Content="Zoom Out" Command="{Binding #ZoomBorder.ZoomOutCommand}"/>
    <Button Content="Reset" Command="{Binding #ZoomBorder.ResetCommand}"/>
    <Button Content="Fit" Command="{Binding #ZoomBorder.FitCommand}"/>
    <Button Content="Fill" Command="{Binding #ZoomBorder.FillCommand}"/>
    <Button Content="Uniform" Command="{Binding #ZoomBorder.UniformCommand}"/>
    <Button Content="Uniform To Fill" Command="{Binding #ZoomBorder.UniformToFillCommand}"/>
    <Button Content="Back" Command="{Binding #ZoomBorder.NavigateBackCommand}"/>
    <Button Content="Forward" Command="{Binding #ZoomBorder.NavigateForwardCommand}"/>
    <Button Content="Toggle Stretch" Command="{Binding #ZoomBorder.ToggleStretchCommand}"/>
</StackPanel>
```

**Available Commands:**
- `ZoomInCommand` - Zoom in at center
- `ZoomOutCommand` - Zoom out at center
- `ResetCommand` - Reset to identity matrix
- `FitCommand` - Fit content to viewport
- `FillCommand` - Fill viewport
- `UniformCommand` - Apply uniform stretch
- `UniformToFillCommand` - Apply uniform to fill stretch
- `NavigateBackCommand` - Navigate back in history
- `NavigateForwardCommand` - Navigate forward in history
- `ToggleStretchCommand` - Cycle through stretch modes

All commands respect `EnableZoom`, `EnablePan`, and `EnableViewHistory` settings.

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

### Zoom to Rectangle

Zoom to fit or exactly match specific content rectangles:

```csharp
// Zoom to fit a rectangle in the viewport
var contentRect = new Rect(100, 100, 200, 150);
zoomBorder.ZoomToRectangle(contentRect);

// Zoom with padding
zoomBorder.ZoomToRectangle(contentRect, padding: new Thickness(20));

// Zoom to exact viewport dimensions
var viewportRect = new Rect(50, 50, 300, 200);
zoomBorder.ZoomToRectangleExact(contentRect, viewportRect);
```

### Saved Views

Save and restore named view states for quick navigation:

```csharp
// Save the current view
zoomBorder.SaveView("DetailView", "Close-up of the detail section");

// Restore a saved view
bool restored = zoomBorder.RestoreView("DetailView");

// Get saved view information
SavedView? view = zoomBorder.GetSavedView("DetailView");
if (view.HasValue)
{
    Console.WriteLine($"Name: {view.Value.Name}");
    Console.WriteLine($"Description: {view.Value.Description}");
    Console.WriteLine($"Saved at: {view.Value.Timestamp}");
}

// List all saved views
string[] viewNames = zoomBorder.GetSavedViewNames();
var allViews = zoomBorder.GetSavedViews();

// Delete a saved view
zoomBorder.DeleteSavedView("DetailView");

// Clear all saved views
zoomBorder.ClearSavedViews();
```

### Discrete Zoom Levels

Enable predefined zoom levels for consistent, predictable zooming:

```csharp
// Enable discrete zoom levels
zoomBorder.EnableDiscreteZoomLevels = true;

// Use default levels: 0.25, 0.5, 0.75, 1.0, 1.5, 2.0, 3.0, 4.0, 6.0, 8.0
// Or set custom levels
zoomBorder.DiscreteZoomLevels = new[] { 0.5, 1.0, 2.0, 4.0, 8.0 };

// Navigate between levels
double nextLevel = zoomBorder.GetNextDiscreteZoomLevel();
double previousLevel = zoomBorder.GetPreviousDiscreteZoomLevel();
double nearestLevel = zoomBorder.GetNearestDiscreteZoomLevel(1.3); // Returns 1.0

// Zoom to specific level
zoomBorder.ZoomToLevel(2.0, centerX: 100, centerY: 75);
```

When `EnableDiscreteZoomLevels` is enabled, all zoom operations will snap to the nearest discrete level.

### Viewport Culling Support

Query visibility for performance optimization and UI updates:

```csharp
// Check if a rectangle is visible in the viewport
var rect = new Rect(100, 100, 50, 50);
bool isVisible = zoomBorder.IsRectangleVisible(rect);

// Check if a point is visible
var point = new Point(150, 150);
bool isPointVisible = zoomBorder.IsPointVisible(point);

// Get the visible portion of a rectangle
Rect visiblePortion = zoomBorder.GetVisiblePortion(rect);

// Example: Conditionally render items based on visibility
foreach (var item in items)
{
    if (zoomBorder.IsRectangleVisible(item.Bounds))
    {
        RenderItem(item);
    }
}
```

## License

PanAndZoom is licensed under the [MIT license](LICENSE.TXT).
