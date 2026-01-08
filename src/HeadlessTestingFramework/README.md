# HeadlessTestingFramework

A comprehensive touch input simulator, gesture recognizer test helpers, tree traversal utilities, and headless screen recorder for testing Avalonia controls.

[![NuGet](https://img.shields.io/nuget/v/Avalonia.HeadlessTestingFramework.svg)](https://www.nuget.org/packages/Avalonia.HeadlessTestingFramework)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](../../LICENSE.TXT)

## Overview

HeadlessTestingFramework enables testing touch and gesture-based interactions in Avalonia headless tests. It provides:

- **TouchInputSimulator** - Simulate raw touch events and high-level gesture events
- **GestureRecognizerTestHelper** - Trigger actual Avalonia gesture recognizers with proper multi-touch support
- **MultiTouchTestHelperFactory** - Convenient factory for multi-finger gesture simulation
- **VisualTreeTestHelper** - Extension methods for visual tree traversal and querying
- **LogicalTreeTestHelper** - Extension methods for logical tree traversal and querying
- **ControlFinder** - Fluent API for complex control queries with chainable filters
- **TreeValidator** - Fluent validation API for asserting visual tree structure
- **TreeXPath** - XPath-like queries for visual tree navigation
- **TreeComparer** - Compare visual trees for structural differences
- **TemplateComparer** - Compare control templates for differences
- **AvaloniaDriver** - Appium-like API for UI automation testing
- **HeadlessScreenRecorder** - Capture frames during headless tests for visual regression testing
- **RecordedTouchSimulator** - Integrated touch simulation with automatic frame capture

## Installation

```bash
dotnet add package Avalonia.HeadlessTestingFramework
```

## Quick Start

### Basic Touch Events

```csharp
using Avalonia.HeadlessTestingFramework;

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
using Avalonia.HeadlessTestingFramework;

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

### VisualTreeTestHelper

Extension methods for visual tree traversal and querying in tests:

```csharp
using Avalonia.HeadlessTestingFramework;

// Find controls by type
var button = window.FindFirst<Button>();
var allButtons = window.FindAll<Button>();

// Find by name
var submitBtn = window.FindByName<Button>("SubmitButton");
var controls = window.FindByNameStartingWith<Button>("btn_");
var matching = window.FindByNameMatching<TextBox>(@"^Input\d+$");

// Find by property
var enabled = window.FindByProperty<Button, bool>(Button.IsEnabledProperty, true);
var tagged = window.FindByTag<Button>("primary");
var styled = window.FindByClass<Border>("section");

// Find by state
var enabledButtons = window.FindEnabled<Button>();
var disabledButtons = window.FindDisabled<Button>();
var visibleBorders = window.FindVisible<Border>();
var hiddenPanels = window.FindHidden<Panel>();

// Ancestor queries
var parentPanel = button.FindAncestor<StackPanel>();
var namedAncestor = button.FindAncestorByName<Border>("Container");
var ancestors = button.GetAncestors<Panel>();

// Tree navigation
var children = panel.GetChildren<Button>();
var firstChild = panel.GetFirstChild<TextBlock>();
var path = button.GetPathFromRoot();

// Hit testing
var hitResult = window.HitTest(new Point(100, 100));
var bounds = button.GetBoundsRelativeTo(window);

// Debugging
window.PrintTree();  // Prints full visual tree to console
```

| Method | Description |
|--------|-------------|
| `FindFirst<T>()` | Find first descendant of type |
| `FindAll<T>(predicate?)` | Find all descendants of type |
| `FindByName<T>(name)` | Find by exact Name property |
| `FindByNameStartingWith<T>(prefix)` | Find by name prefix |
| `FindByNameEndingWith<T>(suffix)` | Find by name suffix |
| `FindByNameContaining<T>(substring)` | Find by name substring |
| `FindByNameMatching<T>(regex)` | Find by name regex pattern |
| `FindByProperty<T,TValue>(prop, value)` | Find by AvaloniaProperty value |
| `FindByTag<T>(tag)` | Find by Tag property |
| `FindByClass<T>(className)` | Find by style class |
| `FindByClasses<T>(classes)` | Find by multiple style classes |
| `FindEnabled<T>()` | Find enabled controls |
| `FindDisabled<T>()` | Find disabled controls |
| `FindVisible<T>()` | Find visible controls |
| `FindHidden<T>()` | Find hidden controls |
| `FindAncestor<T>()` | Find nearest ancestor |
| `GetAncestors<T>()` | Get all ancestors |
| `GetChildren<T>()` | Get typed children |
| `GetPathFromRoot()` | Get path from root to control |
| `HitTest(point)` | Hit test at point |
| `PrintTree()` | Debug print tree structure |

### LogicalTreeTestHelper

Extension methods for logical tree traversal:

```csharp
using Avalonia.HeadlessTestingFramework;

// Find in logical tree
var button = window.FindFirstLogical<Button>();
var allButtons = window.FindAllLogical<Button>();
var named = window.FindLogicalByName<TextBox>("Input");

// Ancestor queries  
var parent = button.FindLogicalAncestor<StackPanel>();
var ancestors = button.GetLogicalAncestors<Panel>();

// Sibling navigation
var nextButton = button.GetNextLogicalSibling<Button>();
var prevButton = button.GetPreviousLogicalSibling<Button>();
var siblings = button.GetLogicalSiblings<Button>();
var siblingIndex = button.GetLogicalSiblingIndex();
var isFirst = button.IsFirstLogicalChild();
var isLast = button.IsLastLogicalChild();

// Content access
var content = contentControl.GetContent<TextBlock>();
var items = itemsControl.GetItems<string>();

// Debugging
window.PrintLogicalTree();
```

| Method | Description |
|--------|-------------|
| `FindFirstLogical<T>()` | Find first in logical tree |
| `FindAllLogical<T>(predicate?)` | Find all in logical tree |
| `FindLogicalByName<T>(name)` | Find by name in logical tree |
| `FindLogicalAncestor<T>()` | Find logical ancestor |
| `GetLogicalAncestors<T>()` | Get all logical ancestors |
| `GetLogicalChildren<T>()` | Get typed logical children |
| `GetNextLogicalSibling<T>()` | Get next sibling |
| `GetPreviousLogicalSibling<T>()` | Get previous sibling |
| `GetLogicalSiblings<T>()` | Get all siblings (excluding self) |
| `GetLogicalSiblingIndex()` | Get index among siblings |
| `IsFirstLogicalChild()` | Check if first child |
| `IsLastLogicalChild()` | Check if last child |
| `GetContent<T>()` | Get content from ContentControl |
| `GetItems<T>()` | Get items from ItemsControl |
| `PrintLogicalTree()` | Debug print logical tree |

### ControlFinder (Fluent API)

Chainable fluent API for complex control queries:

```csharp
using Avalonia.HeadlessTestingFramework;

// Basic usage
var buttons = window.Find()
    .OfType<Button>()
    .FindAll<Button>();

// Type filtering
var exactButtons = window.Find()
    .ExactType<Button>()  // Only Button, not ToggleButton subclasses
    .FindAll<Button>();

// Name filtering
var submitBtns = window.Find()
    .OfType<Button>()
    .WithName("SubmitButton")
    .FindFirst<Button>();

var matchingNames = window.Find()
    .OfType<TextBox>()
    .WithNameMatching(@"^Input\d+$")
    .FindAll<TextBox>();

// Property and style filtering
var primary = window.Find()
    .OfType<Button>()
    .WithTag("action")
    .WithClass("primary")
    .FindAll<Button>();

// State filtering
var enabledPrimary = window.Find()
    .OfType<Button>()
    .Enabled()
    .WithClass("primary")
    .FindAll<Button>();

// Complex queries
var results = window.Find()
    .InVisualTree()           // Search visual tree (default)
    .OfType<Button>()
    .Enabled()
    .WithAnyClass("primary", "action")
    .Where<Button>(b => b.Width > 50)
    .Except<Button>(b => b.Name == "Cancel")
    .Skip(1)
    .Take(5)
    .FindAll<Button>();

// Aggregation
var count = window.Find().OfType<Button>().Count();
var exists = window.Find().OfType<Button>().WithName("Submit").Any();
var single = window.Find().OfType<CheckBox>().Single<CheckBox>();
```

| Method | Description |
|--------|-------------|
| `From(root)` | Set search root |
| `InVisualTree()` | Search visual tree |
| `InLogicalTree()` | Search logical tree |
| `OfType<T>()` | Filter by type (includes subclasses) |
| `ExactType<T>()` | Filter by exact type only |
| `AssignableFrom(type)` | Filter by assignable type |
| `WithName(name)` | Filter by exact name |
| `WithNameStartingWith(prefix)` | Filter by name prefix |
| `WithNameEndingWith(suffix)` | Filter by name suffix |
| `WithNameContaining(substring)` | Filter by name substring |
| `WithNameMatching(pattern)` | Filter by regex pattern |
| `WithProperty<T>(prop, value)` | Filter by property value |
| `WithTag(tag)` | Filter by Tag |
| `WithClass(className)` | Filter by style class |
| `WithClasses(classes)` | Filter by all classes |
| `WithAnyClass(classes)` | Filter by any class |
| `WithoutClass(className)` | Exclude by class |
| `Enabled()` | Filter to enabled controls |
| `Disabled()` | Filter to disabled controls |
| `Visible()` | Filter to visible controls |
| `Hidden()` | Filter to hidden controls |
| `WithText(text)` | Filter controls with text |
| `WithDataContext<T>()` | Filter by DataContext type |
| `WithMinItemCount(min)` | Filter ItemsControls |
| `Where(predicate)` | Custom filter |
| `Except(predicate)` | Exclude by predicate |
| `Except<T>()` | Exclude specific type |
| `Skip(count)` | Skip first N results |
| `Take(count)` | Take first N results |
| `IncludeSelf()` | Include root in search |
| `MaxDepth(depth)` | Limit search depth |
| `FindAll<T>()` | Execute and return all |
| `FindFirst<T>()` | Execute and return first |
| `Single<T>()` | Execute expecting exactly one |
| `Count()` | Execute and count results |
| `Any()` | Check if any results exist |
| `None()` | Check if no results exist |

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

- .NET 6.0, .NET 8.0, .NET 10.0, or .NET Standard 2.0
- Avalonia 11.x

## Headless Screen Recording

The framework includes powerful screen recording capabilities for visual regression testing.

### Basic Screen Recording

```csharp
using Avalonia.HeadlessTestingFramework.Recording;

var recorder = new HeadlessScreenRecorder();

// Start recording
var session = recorder.StartRecording(window, new RecordingOptions
{
    FrameRate = 30,
    Format = RecordingFormat.PngSequence,
    OutputDirectory = "/path/to/output"
});

// Capture frames manually or automatically
recorder.CaptureFrame();

// Record events
recorder.RecordInputEvent("Button clicked", new { ButtonName = "Submit" });
recorder.RecordScrollEvent("Scrolled down", new Vector(0, -100));
recorder.AddMarker("Test checkpoint");

// Stop and get statistics
var stats = recorder.StopRecording();
Console.WriteLine($"Captured {stats.FramesCaptured} frames in {stats.Duration}");
```

### Recording Options

```csharp
// High quality for detailed analysis
var highQuality = RecordingOptions.HighQuality;

// Performance mode for faster tests
var performance = RecordingOptions.Performance;

// Custom options
var custom = new RecordingOptions
{
    FrameRate = 60,
    Format = RecordingFormat.PngSequence,
    Quality = 100,
    ScaleFactor = 1.0,
    AutoCapture = true,           // Capture automatically at frame rate
    WriteImmediately = true,      // Write frames to disk immediately
    CaptureOnChangeOnly = false,  // Capture every frame
    MaxDuration = TimeSpan.FromSeconds(30),
    BaseFileName = "recording"
};
```

### Integrated Touch Recording

Use `RecordedTouchSimulator` to combine touch simulation with automatic frame capture:

```csharp
using Avalonia.HeadlessTestingFramework.Recording;

using var simulator = new RecordedTouchSimulator();

// Start recording
var session = simulator.StartRecording(window, outputPath: "/path/to/output");

// Gestures automatically capture frames at each step
simulator.RecordedPinchZoom(control, center, startDist: 50, endDist: 150);
simulator.RecordedDrag(control, startPoint, endPoint, steps: 10);
simulator.RecordedSwipe(control, startPoint, SwipeDirection.Right);

// Add markers for analysis
simulator.AddMarker("ZoomComplete");

// Stop and analyze
var stats = simulator.StopRecording();
```

### Recording ZoomBorder Interactions

```csharp
[AvaloniaFact]
public void ZoomBorder_PinchZoom_CapturesAnimation()
{
    using var simulator = new RecordedTouchSimulator();
    var zoomBorder = new ZoomBorder { Child = content };
    var window = new Window { Content = zoomBorder };
    window.Show();

    var session = simulator.StartRecording(window, outputPath: outputDir);

    // Record zoom interaction with automatic frame capture
    simulator.AddMarker("ZoomStart", new { Zoom = zoomBorder.ZoomX });
    simulator.RecordedPinchZoom(zoomBorder, new Point(200, 150), 50, 150, steps: 10);
    simulator.AddMarker("ZoomEnd", new { Zoom = zoomBorder.ZoomX });

    var stats = simulator.StopRecording();

    Assert.True(stats.FramesCaptured >= 10);
    Assert.True(stats.OutputFiles.Count > 0);
}
```

### Output Formats

| Format | Description | Use Case |
|--------|-------------|----------|
| `PngSequence` | Individual PNG files | High quality, frame analysis |
| `JpegSequence` | Individual JPEG files | Smaller file size |
| `RawFrames` | In-memory frames | Custom processing |
| `Gif` | Animated GIF (via PNG sequence) | Quick sharing |

### Recording Session Events

Track events during recording for analysis:

```csharp
session.RecordEvent(RecordingEventType.Input, "Touch down at (100, 100)");
session.RecordEvent(RecordingEventType.Scroll, "Scrolled", new { Delta = delta });
session.RecordEvent(RecordingEventType.Animation, "Zoom animation started");
session.RecordEvent(RecordingEventType.Gesture, "Pinch gesture", new { Scale = 1.5 });
session.RecordEvent(RecordingEventType.Marker, "Checkpoint 1");
```

### Async Recording

Record animations and async operations:

```csharp
// Record for a specific duration
await recorder.RecordDurationAsync(TimeSpan.FromSeconds(2), frameInterval: 16);

// Record during an async action
await recorder.RecordActionAsync(async () =>
{
    await control.AnimateZoomAsync(2.0);
}, frameInterval: 16);
```

### TreeValidator (Fluent Validation)

Fluent API for validating visual tree structure in tests:

```csharp
using Avalonia.HeadlessTestingFramework;

// Basic validation
var validator = new TreeValidator()
    .RequireName("SubmitButton")
    .RequireType<TextBox>()
    .RequireMinCount<Button>(3)
    .ValidateVisualTree(window);

if (!validator.IsValid)
{
    Console.WriteLine(validator.GetSummary());
}

// Assert throws on failure
validator.AssertValid();

// Complex validation
new TreeValidator()
    .RequireNames("SaveButton", "CancelButton", "DeleteButton")
    .RequireNameOfType<Button>("SubmitButton")
    .RequireExactCount<TextBox>(2)
    .RequireMaxCount<ComboBox>(5)
    .RequireEnabled("SaveButton")
    .RequireVisible("FormPanel")
    .RequireClass("SubmitButton", "primary")
    .RequirePattern("//Button[@Name='Submit']")
    .ForbidType<Slider>()
    .ForbidPattern("//Button[@IsEnabled='False']")
    .Custom("Has valid form", root => root.FindByName<Panel>("Form") != null)
    .ValidateVisualTree(window)
    .AssertValid();
```

| Method | Description |
|--------|-------------|
| `RequireName(name)` | Assert control with name exists |
| `RequireNames(names...)` | Assert all named controls exist |
| `RequireType<T>()` | Assert control of type exists |
| `RequireNameOfType<T>(name)` | Assert named control is of type |
| `RequireExactCount<T>(count)` | Assert exact count of type |
| `RequireMinCount<T>(min)` | Assert minimum count of type |
| `RequireMaxCount<T>(max)` | Assert maximum count of type |
| `RequireEnabled(name)` | Assert control is enabled |
| `RequireVisible(name)` | Assert control is visible |
| `RequireClass(name, class)` | Assert control has style class |
| `RequirePattern(xpath)` | Assert XPath pattern matches |
| `ForbidType<T>()` | Assert type does not exist |
| `ForbidPattern(xpath)` | Assert XPath pattern doesn't match |
| `Custom(desc, predicate)` | Custom validation rule |
| `ValidateVisualTree(root)` | Execute against visual tree |
| `ValidateLogicalTree(root)` | Execute against logical tree |
| `AssertValid()` | Throw if validation failed |
| `GetSummary()` | Get detailed failure summary |

### TreeXPath (XPath-like Queries)

XPath-like query syntax for visual tree navigation:

```csharp
using Avalonia.HeadlessTestingFramework;

// Create XPath engine
var xpath = new TreeXPath(window);

// Or use extension methods
var button = window.SelectFirstXPath<Button>("//Button[@Name='Submit']");
var allButtons = window.SelectXPath<Button>("//Button").ToList();
var exists = window.ExistsXPath("//TextBox[@Name='Input']");
var count = window.CountXPath("//Button");

// Query examples
var buttons = xpath.Select<Button>("//Button");                    // All buttons
var named = xpath.SelectFirst<Button>("//Button[@Name='Submit']"); // By name
var tagged = xpath.Select<Button>("//Button[@Tag='primary']");     // By tag
var disabled = xpath.Select<Button>("//Button[@IsEnabled='False']"); // By property

// Predicate functions
var matching = xpath.Select<TextBox>("//TextBox[contains(@Name, 'Input')]");
var starting = xpath.Select<Button>("//Button[starts-with(@Name, 'btn_')]");
var ending = xpath.Select<Control>("//Control[ends-with(@Name, 'Panel')]");
var regex = xpath.Select<TextBox>("//TextBox[matches(@Name, '^Input\\d+$')]");

// Position predicates
var first = xpath.SelectFirst<Button>("//Button[1]");
var last = xpath.SelectFirst<Button>("//Button[last()]");

// Child/descendant queries
var panels = xpath.Select<StackPanel>("//StackPanel[Button]");  // Has Button child
var containers = xpath.Select<Border>("//Border[//TextBox]");   // Has TextBox descendant
```

| Method | Description |
|--------|-------------|
| `Select<T>(xpath)` | Select all matching controls |
| `SelectFirst<T>(xpath)` | Select first match |
| `Exists(xpath)` | Check if pattern matches |
| `Count(xpath)` | Count matching controls |

### TreeComparer (Visual Tree Comparison)

Compare visual trees for structural differences:

```csharp
using Avalonia.HeadlessTestingFramework;

// Compare two trees
var result = TreeComparer.Compare(expected, actual);

if (!result.AreEqual)
{
    Console.WriteLine($"Trees differ: {result.Summary}");
    foreach (var diff in result.Differences)
    {
        Console.WriteLine($"  {diff.Path}: {diff.Description}");
    }
}

// With options
var result = TreeComparer.Compare(expected, actual, new TreeCompareOptions
{
    CompareProperties = true,
    CompareNames = true,
    CompareClasses = true,
    CompareBounds = false,
    IgnoredTypes = { typeof(AdornerLayer) },
    IgnoredProperties = { "IsPointerOver", "IsFocused" },
    MaxDepth = 10
});

// Assert trees are equal
TreeComparer.AssertEqual(expected, actual);
```

| Property | Description |
|----------|-------------|
| `AreEqual` | Whether trees are structurally equal |
| `Differences` | List of differences found |
| `Summary` | Human-readable summary |

### TemplateComparer (Template Comparison)

Compare control templates for differences:

```csharp
using Avalonia.HeadlessTestingFramework;

// Compare templates
var result = TemplateComparer.Compare(control1, control2);

if (!result.AreEqual)
{
    Console.WriteLine($"Templates differ:");
    foreach (var diff in result.Differences)
    {
        Console.WriteLine($"  {diff}");
    }
}

// Compare with baseline
var baseline = CreateBaselineControl();
var current = CreateCurrentControl();
TemplateComparer.AssertEqual(baseline, current, "Template should match baseline");
```

### AvaloniaDriver (Appium-like API)

Selenium/Appium-style API for UI automation testing:

```csharp
using Avalonia.HeadlessTestingFramework.Appium;

// Create driver
var driver = new AvaloniaDriver(window);

// Find elements using locators
var button = driver.FindElement(By.Name("SubmitButton"));
var textBox = driver.FindElement(By.AutomationId("email_input"));
var allButtons = driver.FindElements(By.ClassName("Button"));
var typed = driver.FindElement(By.Type<ZoomBorder>());
var xpath = driver.FindElement(By.XPath("//Button[@Name='Submit']"));

// Element interactions
button.Click();
textBox.SendKeys("test@example.com");
textBox.Clear();
var text = textBox.Text;
var isEnabled = button.Enabled;
var isDisplayed = button.Displayed;

// Get/set properties
var zoom = element.GetProperty<double>("ZoomX");
element.SetProperty("EnablePan", true);

// Screenshots
var screenshot = driver.Screenshot();
var elementShot = button.Screenshot();

// Wait for conditions
var result = driver.Wait.Until(
    ExpectedConditions.ElementExists(By.Name("Result")),
    timeout: TimeSpan.FromSeconds(5)
);

var visible = driver.Wait.Until(
    ExpectedConditions.ElementIsVisible(By.Name("Panel")),
    timeout: TimeSpan.FromSeconds(3)
);

// Check existence
var exists = driver.ElementExists(By.Name("OptionalButton"));
var count = driver.ElementCount(By.ClassName("Button"));
```

#### Locator Strategies

| Locator | Description |
|---------|-------------|
| `By.Name(name)` | Find by Name property |
| `By.AutomationId(id)` | Find by AutomationId |
| `By.ClassName(name)` | Find by type name |
| `By.Type<T>()` | Find by exact type |
| `By.XPath(expr)` | Find by XPath expression |
| `By.Property(name, value)` | Find by property value |
| `By.Predicate(desc, func)` | Find by custom predicate |
| `By.Chained(locators...)` | Chain locators (parent→child) |
| `By.All(locators...)` | Match all conditions |
| `By.Any(locators...)` | Match any condition |

#### Touch Actions

```csharp
using Avalonia.HeadlessTestingFramework.Appium;

var element = driver.FindElement(By.Type<ZoomBorder>());

// Simple gestures
var action = new TouchAction(driver)
    .Tap(element)
    .Perform();

// Double tap
new TouchAction(driver)
    .DoubleTap(element)
    .Perform();

// Long press
new TouchAction(driver)
    .LongPress(element, 500)
    .Perform();

// Swipe/drag
new TouchAction(driver)
    .Press(element)
    .MoveTo(element.Center.X + 100, element.Center.Y)
    .Release()
    .Perform();

// Chained actions
new TouchAction(driver)
    .Press(element)
    .Wait(100)
    .MoveBy(50, 0)
    .Wait(100)
    .Release()
    .Perform();

// Multi-touch gestures
MultiTouchAction.Pinch(driver, element, scale: 1.5);  // Zoom in
MultiTouchAction.Pinch(driver, element, scale: 0.5);  // Zoom out
MultiTouchAction.Scroll(driver, element, 0, 100);     // Scroll
```

#### Expected Conditions

```csharp
using Avalonia.HeadlessTestingFramework.Appium;

// Wait for element to exist
driver.Wait.Until(ExpectedConditions.ElementExists(By.Name("Button")));

// Wait for element to be visible
driver.Wait.Until(ExpectedConditions.ElementIsVisible(By.Name("Panel")));

// Wait for element to be clickable
driver.Wait.Until(ExpectedConditions.ElementToBeClickable(By.Name("Submit")));

// Wait for attribute value
driver.Wait.Until(ExpectedConditions.AttributeToBe(By.Name("Input"), "Text", "Hello"));

// Wait for element count
driver.Wait.Until(ExpectedConditions.NumberOfElementsToBe(By.ClassName("Item"), 5));

// Custom condition
driver.Wait.Until(d => d.FindElement(By.Name("Status")).Text == "Complete");
```

## License

MIT License - see [LICENSE.TXT](../../LICENSE.TXT) for details.

## Contributing

Contributions are welcome! Please feel free to submit issues and pull requests.
