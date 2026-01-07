# TouchTestingFramework

A comprehensive touch input simulator and gesture recognizer test helpers for headless testing of Avalonia controls.

[![NuGet](https://img.shields.io/nuget/v/TouchTestingFramework.svg)](https://www.nuget.org/packages/TouchTestingFramework)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.TXT)

## Overview

TouchTestingFramework enables testing touch and gesture-based interactions in Avalonia headless tests. It provides:

- **TouchInputSimulator** - Simulate raw touch events and high-level gesture events
- **GestureRecognizerTestHelper** - Trigger actual Avalonia gesture recognizers with proper multi-touch support
- **MultiTouchTestHelperFactory** - Convenient factory for multi-finger gesture simulation

## Installation

```bash
dotnet add package TouchTestingFramework
```

## Quick Start

### Basic Touch Events

```csharp
using Avalonia.TouchTestingFramework;

var simulator = new TouchInputSimulator();

// Touch down (finger press)
int touchId = simulator.TouchDown(control, new Point(100, 100));

// Touch move (finger drag)
simulator.TouchMove(control, touchId, new Point(150, 150));

// Touch up (finger release)
simulator.TouchUp(control, touchId);
```

### Gesture Events (Using RaiseEvent)

```csharp
var simulator = new TouchInputSimulator();

// Pinch gesture
simulator.PinchGesture(control, scale: 1.5, scaleOrigin: new Point(200, 150));
simulator.PinchGestureEnded(control);

// Scroll gesture
simulator.ScrollGesture(control, new Vector(50, 30));
simulator.ScrollGestureEnded(control);

// Touchpad magnify (macOS)
simulator.TouchpadMagnify(control, new Vector(0.5, 0), new Point(200, 150));
```

### Gesture Recognizer Testing (Triggers Actual Recognizers)

```csharp
using Avalonia.TouchTestingFramework;

// CRITICAL: Register handlers BEFORE window.Show()
var control = new MyControl();
Gestures.AddPinchHandler(control, (s, e) => {
    Console.WriteLine($"Pinch: Scale={e.Scale}");
});

var window = new Window { Content = control };
window.Show();

// Now simulate multi-touch pinch
var (first, second) = MultiTouchTestHelperFactory.CreatePair();

first.Down(control, new Point(100, 150));
second.Down(control, new Point(200, 150));

// Move fingers apart (zoom in)
first.Move(control, new Point(50, 150));
second.Move(control, new Point(250, 150));

first.Up(control);
second.Up(control);
```

## API Reference

### TouchInputSimulator

High-level touch event simulation using `RaiseEvent`:

| Method | Description |
|--------|-------------|
| `TouchDown(target, position)` | Simulate finger press |
| `TouchMove(target, touchId, position)` | Simulate finger move |
| `TouchUp(target, touchId)` | Simulate finger release |
| `Tap(target, position)` | Complete tap gesture |
| `DoubleTap(target, position)` | Complete double tap |
| `PinchGesture(target, scale, origin)` | Raise pinch event |
| `ScrollGesture(target, delta)` | Raise scroll event |
| `TouchpadMagnify(target, delta, position)` | Raise touchpad magnify |
| `TouchpadSwipe(target, delta, position)` | Raise touchpad swipe |
| `TouchpadRotate(target, delta, position)` | Raise touchpad rotate |
| `SimulatePinchZoom(target, center, start, end)` | Complete pinch zoom |
| `SimulateTwoFingerPan(target, start, end)` | Complete two-finger pan |
| `SimulateRotation(target, center, radius, startAngle, endAngle)` | Complete rotation |
| `SimulateDrag(target, start, end)` | Complete drag gesture |
| `Swipe(target, start, direction, distance)` | Complete swipe gesture |

### GestureRecognizerTestHelper

Low-level persistent pointer for triggering actual gesture recognizers:

| Method | Description |
|--------|-------------|
| `Down(target, position)` | Pointer press with capture |
| `Move(target, position)` | Pointer move (routes to gesture recognizer) |
| `Up(target, position)` | Pointer release |
| `Tap(target, position)` | Quick down + up |
| `Cancel()` | Cancel gesture and release capture |
| `Drag(target, start, end, steps)` | Complete drag with interpolation |

| Property | Description |
|----------|-------------|
| `Pointer` | The underlying Avalonia Pointer |
| `Captured` | Currently captured IInputElement |
| `CapturedGestureRecognizer` | Currently captured GestureRecognizer |

### MultiTouchTestHelperFactory

Factory for multi-finger gesture simulation:

| Method | Description |
|--------|-------------|
| `CreatePair()` | Create two touch helpers |
| `Create(count)` | Create N touch helpers |
| `SimulatePinch(target, start1, start2, end1, end2)` | Complete pinch gesture |
| `SimulatePinchZoomIn(target, center, startDist, endDist)` | Zoom in gesture |
| `SimulatePinchZoomOut(target, center, startDist, endDist)` | Zoom out gesture |
| `SimulateTwoFingerPan(target, start, end, spacing)` | Two-finger pan |
| `SimulateRotation(target, center, radius, startAngle, endAngle)` | Rotation gesture |

## Critical Usage Notes

### ⚠️ Event Handler Registration Order

When testing gesture events (Pinch, Scroll, etc.), **handlers must be registered BEFORE `window.Show()` is called**:

```csharp
// ✅ CORRECT - events will fire
var control = new MyControl();
Gestures.AddPinchHandler(control, handler);  // FIRST
var window = new Window { Content = control };
window.Show();  // AFTER

// ❌ WRONG - events won't fire
var window = new Window { Content = control };
window.Show();  // FIRST
Gestures.AddPinchHandler(control, handler);  // Too late!
```

### Using Gestures.AddHandler vs AddHandler

For gesture events, prefer using the static methods on `Gestures`:

```csharp
// ✅ Works correctly
Gestures.AddPinchHandler(control, handler);
Gestures.AddScrollGestureHandler(control, handler);

// ❌ May not work correctly
control.AddHandler(Gestures.PinchEvent, handler);
```

### TouchInputSimulator vs GestureRecognizerTestHelper

| Use Case | Recommended Class |
|----------|-------------------|
| Testing event handling code | `TouchInputSimulator` |
| Testing actual gesture recognizer behavior | `GestureRecognizerTestHelper` |
| Simple single-touch scenarios | `TouchInputSimulator` |
| Complex multi-touch with capture | `GestureRecognizerTestHelper` |

## Example Test

```csharp
[AvaloniaFact]
public void PinchZoom_ShouldChangeScale()
{
    // Arrange
    var pinchRaised = false;
    var lastScale = 1.0;
    
    var control = new ZoomBorder { Width = 400, Height = 300 };
    Gestures.AddPinchHandler(control, (s, e) => {
        pinchRaised = true;
        lastScale = e.Scale;
    });
    
    var window = new Window { Content = control };
    window.Show();
    
    // Act - simulate pinch zoom in
    var (first, second) = MultiTouchTestHelperFactory.CreatePair();
    
    first.Down(control, new Point(150, 150));
    second.Down(control, new Point(250, 150));
    
    // Move fingers apart
    first.Move(control, new Point(100, 150));
    second.Move(control, new Point(300, 150));
    
    first.Up(control);
    second.Up(control);
    
    // Assert
    Assert.True(pinchRaised);
    Assert.True(lastScale > 1.0);
}
```

## Requirements

- .NET 6.0, .NET 8.0, or .NET Standard 2.0
- Avalonia 11.x

## License

MIT License - see [LICENSE.TXT](LICENSE.TXT) for details.

## Contributing

Contributions are welcome! Please feel free to submit issues and pull requests.
