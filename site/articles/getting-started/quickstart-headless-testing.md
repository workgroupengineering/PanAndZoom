---
title: "Quickstart: Headless Testing"
---

# Quickstart: Headless Testing

Use headless tests when you want input and gesture coverage without running a full desktop shell.

## Simple Tap Test

```csharp
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.HeadlessTestingFramework;

[AvaloniaFact]
public void Button_Click_Test()
{
    var window = new Window { Content = new Button { Name = "MyButton" } };
    window.Show();

    var button = window.FindFirst<Button>();
    var simulator = new TouchInputSimulator();

    var clicked = false;
    button.Click += (_, _) => clicked = true;

    simulator.Tap(button, new Point(10, 10));

    Assert.True(clicked);
}
```

## Testing `ZoomBorder` Gestures

Gesture handlers must be registered before `window.Show()` so Avalonia recognizers see the control from the start:

```csharp
[AvaloniaFact]
public void Pinch_Zoom_Changes_Scale()
{
    var zoomBorder = new ZoomBorder();
    double scale = 1.0;

    Gestures.AddPinchHandler(zoomBorder, (_, e) => scale = e.Scale);

    var window = new Window { Content = zoomBorder };
    window.Show();

    MultiTouchTestHelperFactory.SimulatePinchZoomIn(
        zoomBorder,
        center: new Point(200, 200),
        startDistance: 50,
        endDistance: 150);

    Assert.True(scale > 1.0);
}
```

## When To Reach For Each Layer

- `TouchInputSimulator`: direct touch event sequences
- `GestureSimulator`: synthetic high-level gestures
- `AvaloniaDriver`: Selenium/Appium-style tests and waiting
- `HeadlessScreenRecorder`: frame capture for diagnostics

## Next Steps

- [Input Simulators](../headless-testing/input-simulators.md)
- [Tree Helpers](../headless-testing/tree-helpers.md)
- [Recording and Video](../headless-testing/recording-and-video.md)
