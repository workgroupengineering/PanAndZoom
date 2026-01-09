# HeadlessTestingFramework

A comprehensive touch input simulator, gesture recognizer test helpers, tree traversal utilities, and headless screen recorder for testing Avalonia controls.

[![NuGet](https://img.shields.io/nuget/v/HeadlessTestingFramework.svg)](https://www.nuget.org/packages/HeadlessTestingFramework)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](../../LICENSE.TXT)

## Installation

```bash
dotnet add package HeadlessTestingFramework
```

## Features

- **Touch Simulation** - Simulate tap, drag, swipe, pinch, and multi-touch gestures
- **Keyboard Simulation** - Key presses, shortcuts, text input
- **Mouse Simulation** - Clicks, drag-and-drop, wheel scroll
- **Gesture Recognition** - Trigger actual Avalonia gesture recognizers
- **Tree Navigation** - Find controls with fluent API, XPath queries, or type-safe methods
- **Tree Validation** - Assert visual tree structure with chainable rules
- **Tree Comparison** - Diff two visual trees for changes
- **Appium-like API** - Selenium/Appium-style testing with familiar patterns
- **Screen Recording** - Capture frames during tests for visual regression

## Quick Start

```csharp
using Avalonia.HeadlessTestingFramework;

[AvaloniaFact]
public void Button_Click_Test()
{
    var window = new Window { Content = new Button { Name = "MyButton" } };
    window.Show();
    
    var button = window.FindFirst<Button>();
    var simulator = new TouchInputSimulator();
    
    bool clicked = false;
    button.Click += (s, e) => clicked = true;
    
    simulator.Tap(button, new Point(10, 10));
    
    Assert.True(clicked);
}
```

## Which API Should I Use?

| Scenario | Recommended API |
|----------|-----------------|
| Simple button click/tap | `TouchInputSimulator.Tap()` |
| Pinch-to-zoom with gesture handlers | `MultiTouchTestHelperFactory` |
| Selenium/Appium-style testing | `AvaloniaDriver` |
| Complex keyboard shortcuts | `KeyboardInputSimulator` |
| Mouse drag and drop | `MouseInputSimulator` |
| High-level gesture events | `GestureSimulator` |
| Find controls by type/name | `ControlFinder` |
| XPath-like queries | `TreeXPath` |
| Assert tree structure | `TreeValidator` |
| Record test interactions | `RecordedTouchSimulator` |

## ⚠️ Important: Gesture Handler Registration

When testing gesture events (Pinch, Scroll, etc.), **handlers must be registered BEFORE `window.Show()`**:

```csharp
// ✅ CORRECT
var control = new MyControl();
Gestures.AddPinchHandler(control, handler);  // Register FIRST
var window = new Window { Content = control };
window.Show();  // Show AFTER

// ❌ WRONG - events won't fire
var window = new Window { Content = control };
window.Show();  // Show FIRST
Gestures.AddPinchHandler(control, handler);  // Too late!
```

## Documentation

📖 **[Full API Documentation](../../docs/HeadlessTestingAPI.md)** - Complete reference with examples

The full documentation includes:
- Detailed API reference for all components
- Code examples for common scenarios
- Appium compatibility matrix
- Migration guides from Selenium/Appium
- Troubleshooting tips

## Core Components

| Component | Purpose |
|-----------|---------|
| `TouchInputSimulator` | Low-level touch event simulation |
| `GestureSimulator` | High-level gesture events (tap, pinch, scroll) |
| `KeyboardInputSimulator` | Keyboard input simulation |
| `MouseInputSimulator` | Mouse input simulation |
| `GestureRecognizerTestHelper` | Trigger actual gesture recognizers |
| `MultiTouchTestHelperFactory` | Multi-finger gesture simulation |
| `VisualTreeTestHelper` | Visual tree queries (extension methods) |
| `LogicalTreeTestHelper` | Logical tree queries (extension methods) |
| `ControlFinder` | Fluent control finding API |
| `TreeXPath` | XPath-like queries |
| `TreeValidator` | Tree structure assertions |
| `TreeComparer` | Visual tree comparison |
| `AvaloniaDriver` | Appium-like API |
| `HeadlessScreenRecorder` | Frame capture for visual testing |
| `RecordedTouchSimulator` | Touch + recording integration |

## Requirements

- .NET 6.0, .NET 8.0, .NET 10.0, or .NET Standard 2.0
- Avalonia 11.x

## License

MIT License - see [LICENSE.TXT](../../LICENSE.TXT) for details.
