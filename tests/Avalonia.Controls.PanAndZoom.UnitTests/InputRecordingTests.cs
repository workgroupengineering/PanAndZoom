// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.HeadlessTestingFramework;
using Avalonia.HeadlessTestingFramework.Recording;
using Xunit;
using Path = System.IO.Path;

namespace Avalonia.Controls.PanAndZoom.UnitTests;

/// <summary>
/// Tests for keyboard input simulation with screen recording.
/// </summary>
public class KeyboardInputRecordingTests
{
    private readonly string _testOutputDir;

    public KeyboardInputRecordingTests()
    {
        _testOutputDir = Path.Combine(
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..")),
            "artifacts", "recordings", $"KeyboardRecording_{DateTime.Now:yyyyMMdd_HHmmss}");
    }

    private static (TextBox textBox, Window window) CreateTextBoxWindow()
    {
        var textBox = new TextBox
        {
            Width = 300,
            Height = 30,
            Text = ""
        };

        var window = new Window
        {
            Width = 400,
            Height = 200,
            Content = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Children = { textBox }
            }
        };

        return (textBox, window);
    }

    [AvaloniaFact]
    public void KeyboardSimulator_KeyPress_RaisesKeyDownAndKeyUp()
    {
        var (textBox, window) = CreateTextBoxWindow();
        window.Show();

        try
        {
            var simulator = new KeyboardInputSimulator();
            var keyDownRaised = false;
            var keyUpRaised = false;

            textBox.KeyDown += (s, e) => keyDownRaised = true;
            textBox.KeyUp += (s, e) => keyUpRaised = true;

            simulator.KeyPress(textBox, Key.A);

            Assert.True(keyDownRaised, "KeyDown event should be raised");
            Assert.True(keyUpRaised, "KeyUp event should be raised");
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void KeyboardSimulator_ModifierKeys_UpdatesCurrentModifiers()
    {
        var (textBox, window) = CreateTextBoxWindow();
        window.Show();

        try
        {
            var simulator = new KeyboardInputSimulator();

            Assert.Equal(KeyModifiers.None, simulator.CurrentModifiers);

            simulator.CtrlDown(textBox);
            Assert.Equal(KeyModifiers.Control, simulator.CurrentModifiers);

            simulator.ShiftDown(textBox);
            Assert.Equal(KeyModifiers.Control | KeyModifiers.Shift, simulator.CurrentModifiers);

            simulator.ShiftUp(textBox);
            Assert.Equal(KeyModifiers.Control, simulator.CurrentModifiers);

            simulator.CtrlUp(textBox);
            Assert.Equal(KeyModifiers.None, simulator.CurrentModifiers);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void KeyboardSimulator_NavigationKeys_Work()
    {
        var (textBox, window) = CreateTextBoxWindow();
        window.Show();

        try
        {
            var simulator = new KeyboardInputSimulator();
            var keysPressed = new System.Collections.Generic.List<Key>();

            textBox.KeyDown += (s, e) => keysPressed.Add(e.Key);

            simulator.ArrowUp(textBox);
            simulator.ArrowDown(textBox);
            simulator.ArrowLeft(textBox);
            simulator.ArrowRight(textBox);
            simulator.Tab(textBox);
            simulator.Enter(textBox);
            simulator.Escape(textBox);

            Assert.Contains(Key.Up, keysPressed);
            Assert.Contains(Key.Down, keysPressed);
            Assert.Contains(Key.Left, keysPressed);
            Assert.Contains(Key.Right, keysPressed);
            Assert.Contains(Key.Tab, keysPressed);
            Assert.Contains(Key.Enter, keysPressed);
            Assert.Contains(Key.Escape, keysPressed);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void Recording_KeyboardInput_CapturesFrames()
    {
        using var simulator = new RecordedTouchSimulator();
        simulator.FramesPerStep = 1;

        var (textBox, window) = CreateTextBoxWindow();
        window.Show();
        window.UpdateLayout();

        try
        {
            var outputPath = Path.Combine(_testOutputDir, "keyboard_input");
            var session = simulator.StartRecording(window, outputPath: outputPath);

            // Simulate keyboard input
            simulator.RecordedKeyPress(textBox, Key.H);
            simulator.RecordedKeyPress(textBox, Key.E);
            simulator.RecordedKeyPress(textBox, Key.L);
            simulator.RecordedKeyPress(textBox, Key.L);
            simulator.RecordedKeyPress(textBox, Key.O);

            var stats = simulator.StopRecording();

            Assert.True(stats.FramesCaptured > 0, $"Expected frames > 0, got {stats.FramesCaptured}");
            Assert.True(session.Events.Count > 0, "Expected events to be recorded");
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void Recording_KeyboardShortcut_CapturesFrames()
    {
        using var simulator = new RecordedTouchSimulator();
        simulator.FramesPerStep = 1;

        var (textBox, window) = CreateTextBoxWindow();
        window.Show();
        window.UpdateLayout();

        try
        {
            var outputPath = Path.Combine(_testOutputDir, "keyboard_shortcut");
            var session = simulator.StartRecording(window, outputPath: outputPath);

            // Simulate Ctrl+A (Select All)
            simulator.RecordedKeyboardShortcut(textBox, Key.A, KeyModifiers.Control);
            
            // Simulate Ctrl+C (Copy)
            simulator.RecordedKeyboardShortcut(textBox, Key.C, KeyModifiers.Control);
            
            // Simulate Ctrl+V (Paste)
            simulator.RecordedKeyboardShortcut(textBox, Key.V, KeyModifiers.Control);

            var stats = simulator.StopRecording();

            Assert.True(stats.FramesCaptured > 0, $"Expected frames > 0, got {stats.FramesCaptured}");
            Assert.Contains(session.Events, e => e.Description == "KeyboardShortcut");
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void Recording_TabNavigation_CapturesFrames()
    {
        using var simulator = new RecordedTouchSimulator();
        simulator.FramesPerStep = 1;

        var button1 = new Button { Content = "Button 1" };
        var button2 = new Button { Content = "Button 2" };
        var button3 = new Button { Content = "Button 3" };

        var window = new Window
        {
            Width = 400,
            Height = 200,
            Content = new StackPanel
            {
                Children = { button1, button2, button3 }
            }
        };
        window.Show();
        window.UpdateLayout();

        try
        {
            var outputPath = Path.Combine(_testOutputDir, "tab_navigation");
            var session = simulator.StartRecording(window, outputPath: outputPath);

            // Tab through buttons
            simulator.RecordedTabNavigation(button1, 3);
            
            // Shift+Tab back
            simulator.RecordedTabNavigation(button3, 2, reverse: true);

            var stats = simulator.StopRecording();

            Assert.True(stats.FramesCaptured > 0, $"Expected frames > 0, got {stats.FramesCaptured}");
            Assert.Contains(session.Events, e => e.Description == "TabNavigation");
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void Recording_ArrowNavigation_CapturesFrames()
    {
        using var simulator = new RecordedTouchSimulator();
        simulator.FramesPerStep = 1;

        var listBox = new ListBox
        {
            Width = 200,
            Height = 150,
            ItemsSource = new[] { "Item 1", "Item 2", "Item 3", "Item 4", "Item 5" }
        };

        var window = new Window
        {
            Width = 400,
            Height = 300,
            Content = listBox
        };
        window.Show();
        window.UpdateLayout();

        try
        {
            var outputPath = Path.Combine(_testOutputDir, "arrow_navigation");
            var session = simulator.StartRecording(window, outputPath: outputPath);

            // Navigate with arrow keys
            var keys = new[] { Key.Down, Key.Down, Key.Down, Key.Up, Key.Up };
            simulator.RecordedArrowNavigation(listBox, keys);

            var stats = simulator.StopRecording();

            Assert.True(stats.FramesCaptured > 0, $"Expected frames > 0, got {stats.FramesCaptured}");
            Assert.Contains(session.Events, e => e.Description == "ArrowNavigation");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Recording_KeyboardInput_ConvertsToVideo()
    {
        var converter = new VideoConverter();
        if (!converter.IsFfmpegAvailable())
        {
            return; // Skip if FFmpeg not available
        }

        // Find existing keyboard recording
        var recordingsDir = Path.Combine(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..")), "artifacts", "recordings");
        if (!Directory.Exists(recordingsDir))
        {
            return;
        }

        var pngDirs = Directory.GetDirectories(recordingsDir, "*keyboard*", SearchOption.AllDirectories)
            .Where(d => Directory.GetFiles(d, "frame_*.png").Length > 0)
            .ToArray();

        if (pngDirs.Length == 0)
        {
            pngDirs = Directory.GetDirectories(recordingsDir, "*", SearchOption.AllDirectories)
                .Where(d => Directory.GetFiles(d, "frame_*.png").Length > 0)
                .ToArray();
        }

        if (pngDirs.Length == 0)
        {
            return;
        }

        var result = converter.ConvertPngSequenceToVideo(pngDirs[0], options: new VideoConversionOptions
        {
            Format = VideoFormat.Mp4,
            FrameRate = 30,
            Quality = 23,
            Timeout = TimeSpan.FromSeconds(30)
        });

        Assert.True(result.Success, $"Video conversion failed: {result.ErrorMessage}");
        Assert.True(File.Exists(result.OutputPath!));
    }
}

/// <summary>
/// Tests for mouse input simulation with screen recording.
/// </summary>
public class MouseInputRecordingTests
{
    private readonly string _testOutputDir;

    public MouseInputRecordingTests()
    {
        _testOutputDir = Path.Combine(
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..")),
            "artifacts", "recordings", $"MouseRecording_{DateTime.Now:yyyyMMdd_HHmmss}");
    }

    private static (Canvas canvas, Window window) CreateCanvasWindow()
    {
        var canvas = new Canvas
        {
            Width = 400,
            Height = 300,
            Background = Brushes.LightGray
        };

        // Add some shapes for visual feedback
        var rect = new Rectangle
        {
            Width = 50,
            Height = 50,
            Fill = Brushes.Blue
        };
        Canvas.SetLeft(rect, 175);
        Canvas.SetTop(rect, 125);
        canvas.Children.Add(rect);

        var window = new Window
        {
            Width = 400,
            Height = 300,
            Content = canvas
        };

        return (canvas, window);
    }

    [AvaloniaFact]
    public void MouseSimulator_Click_RaisesPointerEvents()
    {
        var (canvas, window) = CreateCanvasWindow();
        window.Show();

        try
        {
            var simulator = new MouseInputSimulator();
            var pointerPressedRaised = false;
            var pointerReleasedRaised = false;

            canvas.PointerPressed += (s, e) => pointerPressedRaised = true;
            canvas.PointerReleased += (s, e) => pointerReleasedRaised = true;

            simulator.Click(canvas, new Point(200, 150));

            Assert.True(pointerPressedRaised, "PointerPressed event should be raised");
            Assert.True(pointerReleasedRaised, "PointerReleased event should be raised");
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void MouseSimulator_Move_RaisesPointerMovedEvent()
    {
        var (canvas, window) = CreateCanvasWindow();
        window.Show();

        try
        {
            var simulator = new MouseInputSimulator();
            var pointerMovedRaised = false;

            canvas.PointerMoved += (s, e) => pointerMovedRaised = true;

            simulator.MoveTo(canvas, new Point(200, 150));

            Assert.True(pointerMovedRaised, "PointerMoved event should be raised");
            Assert.Equal(new Point(200, 150), simulator.Position);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void MouseSimulator_Wheel_RaisesPointerWheelChangedEvent()
    {
        var (canvas, window) = CreateCanvasWindow();
        window.Show();

        try
        {
            var simulator = new MouseInputSimulator();
            var wheelRaised = false;
            Vector? wheelDelta = null;

            canvas.PointerWheelChanged += (s, e) =>
            {
                wheelRaised = true;
                wheelDelta = e.Delta;
            };

            simulator.Wheel(canvas, new Vector(0, 1), new Point(200, 150));

            Assert.True(wheelRaised, "PointerWheelChanged event should be raised");
            Assert.NotNull(wheelDelta);
            Assert.Equal(1, wheelDelta!.Value.Y);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void MouseSimulator_Drag_RaisesCorrectEvents()
    {
        var (canvas, window) = CreateCanvasWindow();
        window.Show();

        try
        {
            var simulator = new MouseInputSimulator();
            var pressCount = 0;
            var moveCount = 0;
            var releaseCount = 0;

            canvas.PointerPressed += (s, e) => pressCount++;
            canvas.PointerMoved += (s, e) => moveCount++;
            canvas.PointerReleased += (s, e) => releaseCount++;

            simulator.Drag(canvas, new Point(100, 100), new Point(300, 200), steps: 10);

            Assert.Equal(1, pressCount);
            Assert.True(moveCount >= 10, $"Expected at least 10 move events, got {moveCount}");
            Assert.Equal(1, releaseCount);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void Recording_MouseClick_CapturesFrames()
    {
        using var simulator = new RecordedTouchSimulator();
        simulator.FramesPerStep = 1;

        var (canvas, window) = CreateCanvasWindow();
        window.Show();
        window.UpdateLayout();

        try
        {
            var outputPath = Path.Combine(_testOutputDir, "mouse_click");
            var session = simulator.StartRecording(window, outputPath: outputPath);

            // Simulate mouse clicks
            simulator.RecordedMouseClick(canvas, new Point(100, 100));
            simulator.RecordedMouseClick(canvas, new Point(200, 150));
            simulator.RecordedMouseClick(canvas, new Point(300, 200));

            var stats = simulator.StopRecording();

            Assert.True(stats.FramesCaptured > 0, $"Expected frames > 0, got {stats.FramesCaptured}");
            Assert.Contains(session.Events, e => e.Description == "MouseClick");
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void Recording_MouseDoubleClick_CapturesFrames()
    {
        using var simulator = new RecordedTouchSimulator();
        simulator.FramesPerStep = 1;

        var (canvas, window) = CreateCanvasWindow();
        window.Show();
        window.UpdateLayout();

        try
        {
            var outputPath = Path.Combine(_testOutputDir, "mouse_doubleclick");
            var session = simulator.StartRecording(window, outputPath: outputPath);

            // Simulate double click
            simulator.RecordedMouseDoubleClick(canvas, new Point(200, 150));

            var stats = simulator.StopRecording();

            Assert.True(stats.FramesCaptured > 0, $"Expected frames > 0, got {stats.FramesCaptured}");
            Assert.Contains(session.Events, e => e.Description == "MouseDoubleClick");
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void Recording_MouseRightClick_CapturesFrames()
    {
        using var simulator = new RecordedTouchSimulator();
        simulator.FramesPerStep = 1;

        var (canvas, window) = CreateCanvasWindow();
        window.Show();
        window.UpdateLayout();

        try
        {
            var outputPath = Path.Combine(_testOutputDir, "mouse_rightclick");
            var session = simulator.StartRecording(window, outputPath: outputPath);

            // Simulate right click (context menu)
            simulator.RecordedMouseRightClick(canvas, new Point(200, 150));

            var stats = simulator.StopRecording();

            Assert.True(stats.FramesCaptured > 0, $"Expected frames > 0, got {stats.FramesCaptured}");
            Assert.Contains(session.Events, e => e.Description == "MouseRightClick");
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void Recording_MouseDrag_CapturesFrames()
    {
        using var simulator = new RecordedTouchSimulator();
        simulator.FramesPerStep = 1;

        var (canvas, window) = CreateCanvasWindow();
        window.Show();
        window.UpdateLayout();

        try
        {
            var outputPath = Path.Combine(_testOutputDir, "mouse_drag");
            var session = simulator.StartRecording(window, outputPath: outputPath);

            // Simulate drag operation
            simulator.RecordedMouseDrag(canvas, new Point(50, 50), new Point(350, 250), steps: 15);

            var stats = simulator.StopRecording();

            Assert.True(stats.FramesCaptured > 0, $"Expected frames > 0, got {stats.FramesCaptured}");
            Assert.Contains(session.Events, e => e.Description == "MouseDrag");
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void Recording_MouseWheel_CapturesFrames()
    {
        using var simulator = new RecordedTouchSimulator();
        simulator.FramesPerStep = 1;

        var scrollViewer = new ScrollViewer
        {
            Width = 400,
            Height = 300,
            Content = new StackPanel
            {
                Children =
                {
                    new TextBlock { Text = "Item 1", Height = 50 },
                    new TextBlock { Text = "Item 2", Height = 50 },
                    new TextBlock { Text = "Item 3", Height = 50 },
                    new TextBlock { Text = "Item 4", Height = 50 },
                    new TextBlock { Text = "Item 5", Height = 50 },
                    new TextBlock { Text = "Item 6", Height = 50 },
                    new TextBlock { Text = "Item 7", Height = 50 },
                    new TextBlock { Text = "Item 8", Height = 50 },
                    new TextBlock { Text = "Item 9", Height = 50 },
                    new TextBlock { Text = "Item 10", Height = 50 },
                }
            }
        };

        var window = new Window
        {
            Width = 400,
            Height = 300,
            Content = scrollViewer
        };
        window.Show();
        window.UpdateLayout();

        try
        {
            var outputPath = Path.Combine(_testOutputDir, "mouse_wheel");
            var session = simulator.StartRecording(window, outputPath: outputPath);

            // Scroll down
            simulator.RecordedMouseWheel(scrollViewer, new Vector(0, -3), new Point(200, 150), steps: 5);
            
            // Scroll up
            simulator.RecordedMouseWheel(scrollViewer, new Vector(0, 3), new Point(200, 150), steps: 5);

            var stats = simulator.StopRecording();

            Assert.True(stats.FramesCaptured > 0, $"Expected frames > 0, got {stats.FramesCaptured}");
            Assert.Contains(session.Events, e => e.Description == "MouseWheel");
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void Recording_MouseHover_CapturesFrames()
    {
        using var simulator = new RecordedTouchSimulator();
        simulator.FramesPerStep = 1;

        var button = new Button
        {
            Content = "Hover Me",
            Width = 100,
            Height = 40
        };

        var window = new Window
        {
            Width = 400,
            Height = 300,
            Content = new Grid
            {
                Children = { button }
            }
        };
        window.Show();
        window.UpdateLayout();

        try
        {
            var outputPath = Path.Combine(_testOutputDir, "mouse_hover");
            var session = simulator.StartRecording(window, outputPath: outputPath);

            // Hover over button
            simulator.RecordedMouseHover(button, new Point(50, 20), duration: 300);

            var stats = simulator.StopRecording();

            Assert.True(stats.FramesCaptured > 0, $"Expected frames > 0, got {stats.FramesCaptured}");
            Assert.Contains(session.Events, e => e.Description == "MouseHover");
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void Recording_MouseMovePath_CapturesFrames()
    {
        using var simulator = new RecordedTouchSimulator();
        simulator.FramesPerStep = 1;

        var (canvas, window) = CreateCanvasWindow();
        window.Show();
        window.UpdateLayout();

        try
        {
            var outputPath = Path.Combine(_testOutputDir, "mouse_path");
            var session = simulator.StartRecording(window, outputPath: outputPath);

            // Move through a path (zigzag pattern)
            var path = new[]
            {
                new Point(50, 50),
                new Point(150, 150),
                new Point(250, 50),
                new Point(350, 150),
                new Point(350, 250)
            };
            simulator.RecordedMouseMovePath(canvas, path);

            var stats = simulator.StopRecording();

            Assert.True(stats.FramesCaptured > 0, $"Expected frames > 0, got {stats.FramesCaptured}");
            Assert.Contains(session.Events, e => e.Description == "MouseMovePath");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Recording_MouseInput_ConvertsToVideo()
    {
        var converter = new VideoConverter();
        if (!converter.IsFfmpegAvailable())
        {
            return; // Skip if FFmpeg not available
        }

        // Find existing mouse recording
        var recordingsDir = Path.Combine(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..")), "artifacts", "recordings");
        if (!Directory.Exists(recordingsDir))
        {
            return;
        }

        var pngDirs = Directory.GetDirectories(recordingsDir, "*mouse*", SearchOption.AllDirectories)
            .Where(d => Directory.GetFiles(d, "frame_*.png").Length > 0)
            .ToArray();

        if (pngDirs.Length == 0)
        {
            pngDirs = Directory.GetDirectories(recordingsDir, "*", SearchOption.AllDirectories)
                .Where(d => Directory.GetFiles(d, "frame_*.png").Length > 0)
                .ToArray();
        }

        if (pngDirs.Length == 0)
        {
            return;
        }

        var result = converter.ConvertPngSequenceToVideo(pngDirs[0], options: new VideoConversionOptions
        {
            Format = VideoFormat.Mp4,
            FrameRate = 30,
            Quality = 23,
            Timeout = TimeSpan.FromSeconds(30)
        });

        Assert.True(result.Success, $"Video conversion failed: {result.ErrorMessage}");
        Assert.True(File.Exists(result.OutputPath!));
    }
}

/// <summary>
/// Tests for combined keyboard and mouse input with screen recording.
/// </summary>
public class CombinedInputRecordingTests
{
    private readonly string _testOutputDir;

    public CombinedInputRecordingTests()
    {
        _testOutputDir = Path.Combine(
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..")),
            "artifacts", "recordings", $"CombinedInput_{DateTime.Now:yyyyMMdd_HHmmss}");
    }

    [AvaloniaFact]
    public void Recording_CompleteInputWorkflow_CapturesAllInputTypes()
    {
        using var simulator = new RecordedTouchSimulator();
        simulator.FramesPerStep = 1;

        var textBox = new TextBox { Width = 300, Height = 30, Text = "" };
        var button = new Button { Content = "Submit", Width = 100, Height = 30 };
        
        var window = new Window
        {
            Width = 400,
            Height = 300,
            Content = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Spacing = 20,
                Children = { textBox, button }
            }
        };
        window.Show();
        window.UpdateLayout();

        try
        {
            var outputPath = Path.Combine(_testOutputDir, "combined_workflow");
            var session = simulator.StartRecording(window, outputPath: outputPath);

            // 1. Click on text box
            simulator.AddMarker("ClickTextBox");
            simulator.RecordedMouseClick(textBox, new Point(150, 15));

            // 2. Type some text
            simulator.AddMarker("TypeText");
            simulator.RecordedKeyPress(textBox, Key.H);
            simulator.RecordedKeyPress(textBox, Key.E);
            simulator.RecordedKeyPress(textBox, Key.L);
            simulator.RecordedKeyPress(textBox, Key.L);
            simulator.RecordedKeyPress(textBox, Key.O);

            // 3. Select all with Ctrl+A
            simulator.AddMarker("SelectAll");
            simulator.RecordedKeyboardShortcut(textBox, Key.A, KeyModifiers.Control);

            // 4. Tab to button
            simulator.AddMarker("TabToButton");
            simulator.RecordedTabNavigation(textBox, 1);

            // 5. Click button
            simulator.AddMarker("ClickButton");
            simulator.RecordedMouseClick(button, new Point(50, 15));

            var stats = simulator.StopRecording();

            Assert.True(stats.FramesCaptured > 0, $"Expected frames > 0, got {stats.FramesCaptured}");
            Assert.Contains(session.Events, e => e.Description == "ClickTextBox");
            Assert.Contains(session.Events, e => e.Description == "TypeText");
            Assert.Contains(session.Events, e => e.Description == "SelectAll");
            Assert.Contains(session.Events, e => e.Description == "TabToButton");
            Assert.Contains(session.Events, e => e.Description == "ClickButton");
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void Recording_MouseDragWithModifiers_CapturesFrames()
    {
        using var simulator = new RecordedTouchSimulator();
        simulator.FramesPerStep = 1;

        var canvas = new Canvas
        {
            Width = 400,
            Height = 300,
            Background = Brushes.White
        };

        var window = new Window
        {
            Width = 400,
            Height = 300,
            Content = canvas
        };
        window.Show();
        window.UpdateLayout();

        try
        {
            var outputPath = Path.Combine(_testOutputDir, "drag_with_modifiers");
            var session = simulator.StartRecording(window, outputPath: outputPath);

            // Hold Ctrl while dragging (common for multi-select)
            simulator.AddMarker("CtrlDrag");
            simulator.MouseSimulator.Modifiers = KeyModifiers.Control;
            simulator.RecordedMouseDrag(canvas, new Point(50, 50), new Point(200, 200), steps: 10);
            simulator.MouseSimulator.Modifiers = KeyModifiers.None;

            // Hold Shift while clicking (common for range select)
            simulator.AddMarker("ShiftClick");
            simulator.MouseSimulator.Modifiers = KeyModifiers.Shift;
            simulator.RecordedMouseClick(canvas, new Point(300, 150));
            simulator.MouseSimulator.Modifiers = KeyModifiers.None;

            var stats = simulator.StopRecording();

            Assert.True(stats.FramesCaptured > 0, $"Expected frames > 0, got {stats.FramesCaptured}");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Recording_CombinedInput_ConvertsToVideo()
    {
        var converter = new VideoConverter();
        if (!converter.IsFfmpegAvailable())
        {
            return;
        }

        var recordingsDir = Path.Combine(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..")), "artifacts", "recordings");
        if (!Directory.Exists(recordingsDir))
        {
            return;
        }

        var pngDirs = Directory.GetDirectories(recordingsDir, "*combined*", SearchOption.AllDirectories)
            .Where(d => Directory.GetFiles(d, "frame_*.png").Length > 0)
            .ToArray();

        if (pngDirs.Length == 0)
        {
            pngDirs = Directory.GetDirectories(recordingsDir, "*", SearchOption.AllDirectories)
                .Where(d => Directory.GetFiles(d, "frame_*.png").Length > 0)
                .ToArray();
        }

        if (pngDirs.Length == 0)
        {
            return;
        }

        var result = converter.ConvertPngSequenceToVideo(pngDirs[0], options: new VideoConversionOptions
        {
            Format = VideoFormat.Mp4,
            FrameRate = 30,
            Quality = 23,
            Timeout = TimeSpan.FromSeconds(30)
        });

        Assert.True(result.Success, $"Video conversion failed: {result.ErrorMessage}");
        Assert.True(File.Exists(result.OutputPath!));
    }
}
