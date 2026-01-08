// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Input.GestureRecognizers;
using Avalonia.Input.Raw;
using Avalonia.Interactivity;

namespace Avalonia.HeadlessTestingFramework;

/// <summary>
/// Comprehensive gesture simulator for headless testing of Avalonia controls.
/// Provides methods to simulate all Avalonia gesture events including tap, double-tap,
/// right-tap, holding, pinch, scroll, pull-to-refresh, and touchpad gestures.
/// </summary>
/// <remarks>
/// Note: Some gesture event args have internal constructors in Avalonia.
/// For these cases, we use reflection or alternative approaches.
/// TappedEventArgs requires a PointerEventArgs which we create internally.
/// ScrollGestureInertiaStartingEventArgs has an internal constructor and cannot be
/// directly instantiated from user code - scroll sequences work without inertia events.
/// </remarks>
public class GestureSimulator
{
    private ulong _timestamp;
    private int _nextGestureId;
    private int _nextPointerId;

    /// <summary>
    /// Gets the current timestamp.
    /// </summary>
    public ulong Timestamp => _timestamp;

    /// <summary>
    /// Creates a new gesture simulator.
    /// </summary>
    public GestureSimulator()
    {
        _timestamp = 0;
        _nextGestureId = 1;
        _nextPointerId = 0;
    }

    /// <summary>
    /// Advances the internal timestamp by the specified number of milliseconds.
    /// </summary>
    /// <param name="milliseconds">Number of milliseconds to advance.</param>
    public void AdvanceTime(int milliseconds)
    {
        _timestamp += (ulong)milliseconds;
    }

    /// <summary>
    /// Resets the simulator to its initial state.
    /// </summary>
    public void Reset()
    {
        _timestamp = 0;
        _nextGestureId = 1;
        _nextPointerId = 0;
    }

    /// <summary>
    /// Generates a unique gesture ID.
    /// </summary>
    /// <returns>A new gesture ID.</returns>
    public int GetNextGestureId() => _nextGestureId++;

    #region Tap Gestures

    /// <summary>
    /// Simulates a Tapped gesture event.
    /// Uses RoutedEventArgs approach since TappedEventArgs requires internal PointerEventArgs.
    /// </summary>
    /// <param name="target">The target control to receive the event.</param>
    /// <param name="position">Position of the tap.</param>
    /// <param name="pointerType">Type of pointer (Touch, Mouse, Pen).</param>
    /// <param name="modifiers">Key modifiers held during the tap.</param>
    public void Tapped(Interactive target, Point position, PointerType pointerType = PointerType.Touch, KeyModifiers modifiers = KeyModifiers.None)
    {
        // TappedEventArgs requires (RoutedEvent, PointerEventArgs) where PointerEventArgs needs internal construction
        // We create a proper PointerEventArgs and wrap it
        var pointer = CreatePointer(pointerType);
        var properties = new PointerPointProperties(RawInputModifiers.None, PointerUpdateKind.LeftButtonReleased);
        
        // Create the underlying pointer event args
        var pointerEventArgs = new PointerReleasedEventArgs(
            target,
            pointer,
            (Visual)target,
            position,
            _timestamp,
            properties,
            modifiers,
            MouseButton.Left);
        
        // Create TappedEventArgs with the proper constructor
        var args = new TappedEventArgs(Gestures.TappedEvent, pointerEventArgs);
        args.Source = target;
        
        target.RaiseEvent(args);
    }

    /// <summary>
    /// Simulates a DoubleTapped gesture event.
    /// </summary>
    /// <param name="target">The target control to receive the event.</param>
    /// <param name="position">Position of the double tap.</param>
    /// <param name="pointerType">Type of pointer (Touch, Mouse, Pen).</param>
    /// <param name="modifiers">Key modifiers held during the tap.</param>
    public void DoubleTapped(Interactive target, Point position, PointerType pointerType = PointerType.Touch, KeyModifiers modifiers = KeyModifiers.None)
    {
        var pointer = CreatePointer(pointerType);
        var properties = new PointerPointProperties(RawInputModifiers.None, PointerUpdateKind.LeftButtonReleased);
        
        var pointerEventArgs = new PointerReleasedEventArgs(
            target,
            pointer,
            (Visual)target,
            position,
            _timestamp,
            properties,
            modifiers,
            MouseButton.Left);
        
        var args = new TappedEventArgs(Gestures.DoubleTappedEvent, pointerEventArgs);
        args.Source = target;
        
        target.RaiseEvent(args);
    }

    /// <summary>
    /// Simulates a RightTapped gesture event (context menu trigger).
    /// </summary>
    /// <param name="target">The target control to receive the event.</param>
    /// <param name="position">Position of the right tap.</param>
    /// <param name="pointerType">Type of pointer (Touch, Mouse, Pen).</param>
    /// <param name="modifiers">Key modifiers held during the tap.</param>
    public void RightTapped(Interactive target, Point position, PointerType pointerType = PointerType.Touch, KeyModifiers modifiers = KeyModifiers.None)
    {
        var pointer = CreatePointer(pointerType);
        var properties = new PointerPointProperties(RawInputModifiers.RightMouseButton, PointerUpdateKind.RightButtonReleased);
        
        var pointerEventArgs = new PointerReleasedEventArgs(
            target,
            pointer,
            (Visual)target,
            position,
            _timestamp,
            properties,
            modifiers,
            MouseButton.Right);
        
        var args = new TappedEventArgs(Gestures.RightTappedEvent, pointerEventArgs);
        args.Source = target;
        
        target.RaiseEvent(args);
    }

    #endregion

    #region Holding Gesture

    /// <summary>
    /// Simulates a Holding gesture started event (press and hold).
    /// </summary>
    /// <param name="target">The target control to receive the event.</param>
    /// <param name="position">Position of the holding gesture.</param>
    /// <param name="pointerType">Type of pointer (Touch, Mouse, Pen).</param>
    public void HoldingStarted(Interactive target, Point position, PointerType pointerType = PointerType.Touch)
    {
        var args = new HoldingRoutedEventArgs(HoldingState.Started, position, pointerType)
        {
            RoutedEvent = Gestures.HoldingEvent,
            Source = target
        };
        
        target.RaiseEvent(args);
    }

    /// <summary>
    /// Simulates a Holding gesture completed event.
    /// </summary>
    /// <param name="target">The target control to receive the event.</param>
    /// <param name="position">Position of the holding gesture.</param>
    /// <param name="pointerType">Type of pointer (Touch, Mouse, Pen).</param>
    public void HoldingCompleted(Interactive target, Point position, PointerType pointerType = PointerType.Touch)
    {
        var args = new HoldingRoutedEventArgs(HoldingState.Completed, position, pointerType)
        {
            RoutedEvent = Gestures.HoldingEvent,
            Source = target
        };
        
        target.RaiseEvent(args);
    }

    /// <summary>
    /// Simulates a Holding gesture cancelled event.
    /// </summary>
    /// <param name="target">The target control to receive the event.</param>
    /// <param name="position">Position of the holding gesture.</param>
    /// <param name="pointerType">Type of pointer (Touch, Mouse, Pen).</param>
    public void HoldingCancelled(Interactive target, Point position, PointerType pointerType = PointerType.Touch)
    {
        var args = new HoldingRoutedEventArgs(HoldingState.Cancelled, position, pointerType)
        {
            RoutedEvent = Gestures.HoldingEvent,
            Source = target
        };
        
        target.RaiseEvent(args);
    }

    /// <summary>
    /// Simulates a complete holding gesture sequence (started -> wait -> completed).
    /// </summary>
    /// <param name="target">The target control to receive the event.</param>
    /// <param name="position">Position of the holding gesture.</param>
    /// <param name="holdDuration">Duration to hold in milliseconds.</param>
    /// <param name="pointerType">Type of pointer (Touch, Mouse, Pen).</param>
    public void Hold(Interactive target, Point position, int holdDuration = 500, PointerType pointerType = PointerType.Touch)
    {
        HoldingStarted(target, position, pointerType);
        AdvanceTime(holdDuration);
        HoldingCompleted(target, position, pointerType);
    }

    #endregion

    #region Pinch Gestures

    /// <summary>
    /// Simulates a Pinch gesture event.
    /// </summary>
    /// <param name="target">The target control to receive the event.</param>
    /// <param name="scale">Scale factor (> 1 for zoom in, &lt; 1 for zoom out).</param>
    /// <param name="scaleOrigin">Origin point of the pinch gesture.</param>
    /// <param name="angleDelta">Rotation angle delta in radians.</param>
    /// <param name="distance">Total distance between fingers.</param>
    public void Pinch(Interactive target, double scale, Point scaleOrigin, double angleDelta = 0.0, double distance = 0.0)
    {
        var args = new PinchEventArgs(scale, scaleOrigin, angleDelta, distance)
        {
            RoutedEvent = Gestures.PinchEvent,
            Source = target
        };

        target.RaiseEvent(args);
    }

    /// <summary>
    /// Simulates a Pinch gesture ended event.
    /// </summary>
    /// <param name="target">The target control to receive the event.</param>
    public void PinchEnded(Interactive target)
    {
        var args = new PinchEndedEventArgs()
        {
            RoutedEvent = Gestures.PinchEndedEvent,
            Source = target
        };

        target.RaiseEvent(args);
    }

    /// <summary>
    /// Simulates a complete pinch zoom gesture sequence.
    /// </summary>
    /// <param name="target">The target control to receive the event.</param>
    /// <param name="origin">Center point of the pinch.</param>
    /// <param name="startScale">Starting scale (typically 1.0).</param>
    /// <param name="endScale">Ending scale.</param>
    /// <param name="steps">Number of intermediate steps.</param>
    /// <param name="startAngle">Starting angle in radians.</param>
    /// <param name="endAngle">Ending angle in radians.</param>
    public void PinchZoom(Interactive target, Point origin, double startScale, double endScale, int steps = 10, double startAngle = 0, double endAngle = 0)
    {
        var scaleStep = (endScale - startScale) / steps;
        var angleStep = (endAngle - startAngle) / steps;

        for (int i = 0; i <= steps; i++)
        {
            var currentScale = startScale + (scaleStep * i);
            var currentAngle = startAngle + (angleStep * i);
            var angleDelta = i > 0 ? angleStep : 0;
            
            Pinch(target, currentScale, origin, angleDelta);
            AdvanceTime(16);
        }

        PinchEnded(target);
    }

    /// <summary>
    /// Simulates a pinch rotation gesture.
    /// </summary>
    /// <param name="target">The target control to receive the event.</param>
    /// <param name="origin">Center point of the rotation.</param>
    /// <param name="startAngle">Starting angle in degrees.</param>
    /// <param name="endAngle">Ending angle in degrees.</param>
    /// <param name="steps">Number of intermediate steps.</param>
    public void PinchRotate(Interactive target, Point origin, double startAngle, double endAngle, int steps = 10)
    {
        var startAngleRad = startAngle * Math.PI / 180;
        var endAngleRad = endAngle * Math.PI / 180;
        PinchZoom(target, origin, 1.0, 1.0, steps, startAngleRad, endAngleRad);
    }

    #endregion

    #region Scroll Gestures

    /// <summary>
    /// Simulates a Scroll gesture event.
    /// </summary>
    /// <param name="target">The target control to receive the event.</param>
    /// <param name="delta">Scroll delta vector.</param>
    /// <param name="gestureId">Optional gesture ID (auto-generated if not specified).</param>
    public void Scroll(Interactive target, Vector delta, int? gestureId = null)
    {
        var id = gestureId ?? GetNextGestureId();
        var args = new ScrollGestureEventArgs(id, delta)
        {
            RoutedEvent = Gestures.ScrollGestureEvent,
            Source = target
        };

        target.RaiseEvent(args);
    }

    /// <summary>
    /// Simulates a Scroll gesture ended event.
    /// </summary>
    /// <param name="target">The target control to receive the event.</param>
    /// <param name="gestureId">Gesture ID of the scroll to end.</param>
    public void ScrollEnded(Interactive target, int gestureId)
    {
        var args = new ScrollGestureEndedEventArgs(gestureId)
        {
            RoutedEvent = Gestures.ScrollGestureEndedEvent,
            Source = target
        };

        target.RaiseEvent(args);
    }

    /// <summary>
    /// Simulates a Scroll gesture inertia starting event.
    /// Note: ScrollGestureInertiaStartingEventArgs has an internal constructor in Avalonia.
    /// This method uses reflection to create the event args.
    /// </summary>
    /// <param name="target">The target control to receive the event.</param>
    /// <param name="gestureId">Gesture ID of the scroll.</param>
    /// <param name="inertia">Inertia velocity vector.</param>
    /// <returns>True if the event was raised successfully, false if reflection failed.</returns>
    public bool ScrollInertiaStarting(Interactive target, int gestureId, Vector inertia)
    {
        try
        {
            // ScrollGestureInertiaStartingEventArgs has internal constructor
            // Use reflection to create instance
            var type = typeof(ScrollGestureEventArgs).Assembly.GetType("Avalonia.Input.ScrollGestureInertiaStartingEventArgs");
            if (type == null) return false;
            
            var constructor = type.GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new[] { typeof(int), typeof(Vector) },
                null);
            
            if (constructor == null) return false;
            
            var args = (RoutedEventArgs)constructor.Invoke(new object[] { gestureId, inertia });
            args.RoutedEvent = Gestures.ScrollGestureInertiaStartingEvent;
            args.Source = target;
            
            target.RaiseEvent(args);
            return true;
        }
        catch
        {
            // If reflection fails, just skip the inertia event
            return false;
        }
    }

    /// <summary>
    /// Simulates a complete scroll gesture sequence with optional inertia.
    /// </summary>
    /// <param name="target">The target control to receive the event.</param>
    /// <param name="totalDelta">Total scroll distance.</param>
    /// <param name="steps">Number of intermediate steps.</param>
    /// <param name="withInertia">Whether to include inertia at the end.</param>
    /// <param name="inertiaVelocity">Velocity for inertia (if enabled).</param>
    public void ScrollSequence(Interactive target, Vector totalDelta, int steps = 10, bool withInertia = false, Vector? inertiaVelocity = null)
    {
        var gestureId = GetNextGestureId();
        var deltaPerStep = new Vector(totalDelta.X / steps, totalDelta.Y / steps);

        for (int i = 0; i < steps; i++)
        {
            Scroll(target, deltaPerStep, gestureId);
            AdvanceTime(16);
        }

        if (withInertia && inertiaVelocity.HasValue)
        {
            ScrollInertiaStarting(target, gestureId, inertiaVelocity.Value);
            AdvanceTime(16);
        }

        ScrollEnded(target, gestureId);
    }

    #endregion

    #region Pull Gestures (Pull-to-Refresh)

    /// <summary>
    /// Simulates a Pull gesture event (pull-to-refresh style).
    /// </summary>
    /// <param name="target">The target control to receive the event.</param>
    /// <param name="delta">Pull delta vector.</param>
    /// <param name="direction">Pull direction.</param>
    /// <param name="gestureId">Optional gesture ID.</param>
    public void Pull(Interactive target, Vector delta, PullDirection direction, int? gestureId = null)
    {
        var id = gestureId ?? GetNextGestureId();
        var args = new PullGestureEventArgs(id, delta, direction)
        {
            RoutedEvent = Gestures.PullGestureEvent,
            Source = target
        };

        target.RaiseEvent(args);
    }

    /// <summary>
    /// Simulates a Pull gesture ended event.
    /// </summary>
    /// <param name="target">The target control to receive the event.</param>
    /// <param name="gestureId">Gesture ID of the pull to end.</param>
    /// <param name="direction">Pull direction.</param>
    public void PullEnded(Interactive target, int gestureId, PullDirection direction)
    {
        var args = new PullGestureEndedEventArgs(gestureId, direction)
        {
            RoutedEvent = Gestures.PullGestureEndedEvent,
            Source = target
        };

        target.RaiseEvent(args);
    }

    /// <summary>
    /// Simulates a complete pull-to-refresh gesture sequence.
    /// </summary>
    /// <param name="target">The target control to receive the event.</param>
    /// <param name="direction">Pull direction.</param>
    /// <param name="distance">Total pull distance.</param>
    /// <param name="steps">Number of intermediate steps.</param>
    public void PullToRefresh(Interactive target, PullDirection direction, double distance = 100, int steps = 10)
    {
        var gestureId = GetNextGestureId();
        var delta = direction switch
        {
            PullDirection.TopToBottom => new Vector(0, distance / steps),
            PullDirection.BottomToTop => new Vector(0, -distance / steps),
            PullDirection.LeftToRight => new Vector(distance / steps, 0),
            PullDirection.RightToLeft => new Vector(-distance / steps, 0),
            _ => Vector.Zero
        };

        for (int i = 0; i < steps; i++)
        {
            Pull(target, delta, direction, gestureId);
            AdvanceTime(16);
        }

        PullEnded(target, gestureId, direction);
    }

    #endregion

    #region Touchpad Gestures

    /// <summary>
    /// Simulates a touchpad magnify gesture (macOS trackpad pinch-to-zoom).
    /// </summary>
    /// <param name="target">The target control to receive the event.</param>
    /// <param name="delta">Magnification delta.</param>
    /// <param name="position">Position of the gesture.</param>
    /// <param name="modifiers">Key modifiers.</param>
    public void TouchpadMagnify(Interactive target, Vector delta, Point position, KeyModifiers modifiers = KeyModifiers.None)
    {
        var args = CreatePointerDeltaEventArgs(target, delta, position, Gestures.PointerTouchPadGestureMagnifyEvent, modifiers);
        target.RaiseEvent(args);
    }

    /// <summary>
    /// Simulates a touchpad rotate gesture (macOS trackpad rotation).
    /// </summary>
    /// <param name="target">The target control to receive the event.</param>
    /// <param name="angleDelta">Rotation delta in degrees.</param>
    /// <param name="position">Position of the gesture.</param>
    /// <param name="modifiers">Key modifiers.</param>
    public void TouchpadRotate(Interactive target, double angleDelta, Point position, KeyModifiers modifiers = KeyModifiers.None)
    {
        var args = CreatePointerDeltaEventArgs(target, new Vector(angleDelta, 0), position, Gestures.PointerTouchPadGestureRotateEvent, modifiers);
        target.RaiseEvent(args);
    }

    /// <summary>
    /// Simulates a touchpad swipe gesture (macOS trackpad multi-finger swipe).
    /// </summary>
    /// <param name="target">The target control to receive the event.</param>
    /// <param name="delta">Swipe delta vector.</param>
    /// <param name="position">Position of the gesture.</param>
    /// <param name="modifiers">Key modifiers.</param>
    public void TouchpadSwipe(Interactive target, Vector delta, Point position, KeyModifiers modifiers = KeyModifiers.None)
    {
        var args = CreatePointerDeltaEventArgs(target, delta, position, Gestures.PointerTouchPadGestureSwipeEvent, modifiers);
        target.RaiseEvent(args);
    }

    /// <summary>
    /// Simulates a complete touchpad magnify sequence (zoom in or out).
    /// </summary>
    /// <param name="target">The target control to receive the event.</param>
    /// <param name="position">Position of the gesture.</param>
    /// <param name="totalMagnification">Total magnification amount (positive = zoom in, negative = zoom out).</param>
    /// <param name="steps">Number of intermediate steps.</param>
    /// <param name="modifiers">Key modifiers.</param>
    public void TouchpadMagnifySequence(Interactive target, Point position, double totalMagnification, int steps = 10, KeyModifiers modifiers = KeyModifiers.None)
    {
        var deltaPerStep = totalMagnification / steps;
        
        for (int i = 0; i < steps; i++)
        {
            TouchpadMagnify(target, new Vector(deltaPerStep, 0), position, modifiers);
            AdvanceTime(16);
        }
    }

    /// <summary>
    /// Simulates a complete touchpad rotation sequence.
    /// </summary>
    /// <param name="target">The target control to receive the event.</param>
    /// <param name="position">Position of the gesture.</param>
    /// <param name="totalAngle">Total rotation angle in degrees.</param>
    /// <param name="steps">Number of intermediate steps.</param>
    /// <param name="modifiers">Key modifiers.</param>
    public void TouchpadRotateSequence(Interactive target, Point position, double totalAngle, int steps = 10, KeyModifiers modifiers = KeyModifiers.None)
    {
        var deltaPerStep = totalAngle / steps;
        
        for (int i = 0; i < steps; i++)
        {
            TouchpadRotate(target, deltaPerStep, position, modifiers);
            AdvanceTime(16);
        }
    }

    /// <summary>
    /// Simulates a complete touchpad swipe sequence.
    /// </summary>
    /// <param name="target">The target control to receive the event.</param>
    /// <param name="position">Position of the gesture.</param>
    /// <param name="direction">Swipe direction.</param>
    /// <param name="distance">Swipe distance.</param>
    /// <param name="steps">Number of intermediate steps.</param>
    /// <param name="modifiers">Key modifiers.</param>
    public void TouchpadSwipeSequence(Interactive target, Point position, SwipeDirection direction, double distance = 100, int steps = 10, KeyModifiers modifiers = KeyModifiers.None)
    {
        var delta = direction switch
        {
            SwipeDirection.Left => new Vector(-distance / steps, 0),
            SwipeDirection.Right => new Vector(distance / steps, 0),
            SwipeDirection.Up => new Vector(0, -distance / steps),
            SwipeDirection.Down => new Vector(0, distance / steps),
            _ => Vector.Zero
        };
        
        for (int i = 0; i < steps; i++)
        {
            TouchpadSwipe(target, delta, position, modifiers);
            AdvanceTime(16);
        }
    }

    #endregion

    #region Compound Gestures

    /// <summary>
    /// Simulates a tap-and-hold gesture (tap followed by hold).
    /// </summary>
    /// <param name="target">The target control to receive the event.</param>
    /// <param name="position">Position of the gesture.</param>
    /// <param name="holdDuration">Duration to hold in milliseconds.</param>
    /// <param name="pointerType">Type of pointer.</param>
    public void TapAndHold(Interactive target, Point position, int holdDuration = 500, PointerType pointerType = PointerType.Touch)
    {
        Tapped(target, position, pointerType);
        AdvanceTime(100);
        Hold(target, position, holdDuration, pointerType);
    }

    /// <summary>
    /// Simulates a double-tap-and-zoom gesture (common iOS/Android pattern).
    /// </summary>
    /// <param name="target">The target control to receive the event.</param>
    /// <param name="position">Position of the gesture.</param>
    /// <param name="zoomScale">Zoom scale factor.</param>
    /// <param name="steps">Animation steps.</param>
    public void DoubleTapZoom(Interactive target, Point position, double zoomScale = 2.0, int steps = 10)
    {
        DoubleTapped(target, position);
        AdvanceTime(50);
        PinchZoom(target, position, 1.0, zoomScale, steps);
    }

    /// <summary>
    /// Simulates a flick/fling gesture (fast swipe with inertia).
    /// </summary>
    /// <param name="target">The target control to receive the event.</param>
    /// <param name="direction">Flick direction.</param>
    /// <param name="distance">Initial flick distance.</param>
    /// <param name="velocity">Inertia velocity.</param>
    public void Flick(Interactive target, SwipeDirection direction, double distance = 50, double velocity = 500)
    {
        var delta = direction switch
        {
            SwipeDirection.Left => new Vector(-distance, 0),
            SwipeDirection.Right => new Vector(distance, 0),
            SwipeDirection.Up => new Vector(0, -distance),
            SwipeDirection.Down => new Vector(0, distance),
            _ => Vector.Zero
        };

        var inertiaVector = direction switch
        {
            SwipeDirection.Left => new Vector(-velocity, 0),
            SwipeDirection.Right => new Vector(velocity, 0),
            SwipeDirection.Up => new Vector(0, -velocity),
            SwipeDirection.Down => new Vector(0, velocity),
            _ => Vector.Zero
        };

        ScrollSequence(target, delta, steps: 3, withInertia: true, inertiaVelocity: inertiaVector);
    }

    /// <summary>
    /// Simulates a three-finger swipe gesture (common for navigation).
    /// </summary>
    /// <param name="target">The target control to receive the event.</param>
    /// <param name="position">Position of the gesture.</param>
    /// <param name="direction">Swipe direction.</param>
    /// <param name="distance">Swipe distance.</param>
    public void ThreeFingerSwipe(Interactive target, Point position, SwipeDirection direction, double distance = 100)
    {
        // Use touchpad swipe which is typically a multi-finger gesture
        TouchpadSwipeSequence(target, position, direction, distance, steps: 5);
    }

    #endregion

    #region Helper Methods

    private Avalonia.Input.Pointer CreatePointer(PointerType pointerType)
    {
        return new Avalonia.Input.Pointer(_nextPointerId++, pointerType, true);
    }

    private PointerDeltaEventArgs CreatePointerDeltaEventArgs(Interactive target, Vector delta, Point position, RoutedEvent routedEvent, KeyModifiers modifiers)
    {
        var pointer = CreatePointer(PointerType.Touch);
        var properties = new PointerPointProperties(RawInputModifiers.None, PointerUpdateKind.Other);
        
        return new PointerDeltaEventArgs(
            routedEvent,
            target,
            pointer,
            (Visual)target,
            position,
            _timestamp,
            properties,
            modifiers,
            delta);
    }

    #endregion
}
