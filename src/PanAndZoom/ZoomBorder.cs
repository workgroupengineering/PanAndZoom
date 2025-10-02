// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Diagnostics;
using Avalonia.Controls.Metadata;
using Avalonia.Input;
using Avalonia.Input.GestureRecognizers;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using Avalonia.Reactive;
using Avalonia.Styling;
using static System.Math;

namespace Avalonia.Controls.PanAndZoom;

/// <summary>
/// Pan and zoom control for Avalonia.
/// </summary>
[PseudoClasses(":isPanning")]
public partial class ZoomBorder : Border
{
    [Conditional("DEBUG")]
    private static void Log(string message) => Debug.WriteLine(message);

    private static double ClampValue(double value, double minimum, double maximum)
    {
        if (minimum > maximum)
            throw new ArgumentException($"Parameter {nameof(minimum)} is greater than {nameof(maximum)}.");

        if (maximum < minimum)
            throw new ArgumentException($"Parameter {nameof(maximum)} is lower than {nameof(minimum)}.");

        return Min(Max(value, minimum), maximum);
    }

    /// <summary>
    /// Calculate pan and zoom matrix based on provided stretch mode.
    /// </summary>
    /// <param name="panelWidth">The panel width.</param>
    /// <param name="panelHeight">The panel height.</param>
    /// <param name="elementWidth">The element width.</param>
    /// <param name="elementHeight">The element height.</param>
    /// <param name="mode">The stretch mode.</param>
    public static Matrix CalculateMatrix(double panelWidth, double panelHeight, double elementWidth, double elementHeight, StretchMode mode)
    {
        var zx = panelWidth / elementWidth;
        var zy = panelHeight / elementHeight;
        var cx = elementWidth / 2.0;
        var cy = elementHeight / 2.0;

        switch (mode)
        {
            default:
            case StretchMode.None:
                return Matrix.Identity;
            case StretchMode.Fill:
                return MatrixHelper.ScaleAt(zx, zy, cx, cy);
            case StretchMode.Uniform:
            {
                var zoom = Min(zx, zy);
                return MatrixHelper.ScaleAt(zoom, zoom, cx, cy);
            }
            case StretchMode.UniformToFill:
            {
                var zoom = Max(zx, zy);
                return MatrixHelper.ScaleAt(zoom, zoom, cx, cy);
            }
        }
    }

    private PinchGestureRecognizer? _pinchGestureRecognizer;
    private ScrollGestureRecognizer? _scrollGestureRecognizer;
    private bool _gestureRecognizersAdded;

    /// <summary>
    /// Initializes a new instance of the <see cref="ZoomBorder"/> class.
    /// </summary>
    public ZoomBorder()
    {
        _isPanning = false;
        _matrix = Matrix.Identity;
        _captured = false;

        Focusable = true;
        Background = Brushes.Transparent;

        AttachedToVisualTree += PanAndZoom_AttachedToVisualTree;
        DetachedFromVisualTree += PanAndZoom_DetachedFromVisualTree;

        this.GetObservable(ChildProperty).Subscribe(new AnonymousObserver<Control?>(ChildChanged));
        this.GetObservable(BoundsProperty).Subscribe(new AnonymousObserver<Rect>(BoundsChangedHandler));
        
        // Initialize gesture recognizers
        _pinchGestureRecognizer = new PinchGestureRecognizer();
        _scrollGestureRecognizer = new ScrollGestureRecognizer
        {
            CanHorizontallyScroll = true,
            CanVerticallyScroll = true
        };
        
        // Add gesture recognizers based on EnableGestures flag
        UpdateGestureRecognizers();
        
        // Subscribe to EnableGestures property changes
        this.GetObservable(EnableGesturesProperty).Subscribe(new AnonymousObserver<bool>(_ => UpdateGestureRecognizers()));
    }

    /// <summary>
    /// Updates gesture recognizers based on EnableGestures flag.
    /// </summary>
    private void UpdateGestureRecognizers()
    {
        if (EnableGestures && !_gestureRecognizersAdded)
        {
            // Add pinch gesture recognizer
            if (_pinchGestureRecognizer != null)
            {
                GestureRecognizers.Add(_pinchGestureRecognizer);
            }
            
            // Add scroll gesture recognizer only if not disabled by ScrollViewer parent
            if (_scrollGestureRecognizer != null)
            {
                GestureRecognizers.Add(_scrollGestureRecognizer);
            }
            
            _gestureRecognizersAdded = true;
        }
        else if (!EnableGestures && _gestureRecognizersAdded)
        {
            // Since GestureRecognizerCollection doesn't support Remove/Clear,
            // we need to recreate the recognizers to effectively "remove" them
            _pinchGestureRecognizer = new PinchGestureRecognizer();
            _scrollGestureRecognizer = new ScrollGestureRecognizer
            {
                CanHorizontallyScroll = true,
                CanVerticallyScroll = true
            };
            
            _gestureRecognizersAdded = false;
        }
    }

    /// <summary>
    /// Checks if panning is allowed on pointer-wheel event.
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    protected virtual bool CanPanOnPointerWheel(PointerWheelEventArgs e)
    {
        return EnablePan;
    }

    /// <summary>
    /// Checks if zooming is allowed on pointer-wheel event.
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    protected virtual bool CanZoomOnPointerWheel(PointerWheelEventArgs e)
    {
        return EnableZoom;
    }

    /// <summary>
    /// Arranges the control's child.
    /// </summary>
    /// <param name="finalSize">The size allocated to the control.</param>
    /// <returns>The space taken.</returns>
    protected override Size ArrangeOverride(Size finalSize)
    {
        var size = base.ArrangeOverride(finalSize);

        if (_element == null || !_element.IsMeasureValid)
        {
            return size;
        }

        AutoFit(size.Width, size.Height, _element.Bounds.Width, _element.Bounds.Height);

        return size;
    }

    private void PanAndZoom_AttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        Log($"[AttachedToVisualTree] {Name}");
        ChildChanged(Child);

        // Add pointer event handlers
        PointerWheelChanged += Border_PointerWheelChanged;
        PointerPressed += Border_PointerPressed;
        PointerReleased += Border_PointerReleased;
        PointerMoved += Border_PointerMoved;
        PointerCaptureLost += Border_PointerCaptureLost;
        DoubleTapped += Border_DoubleTapped;
        
        // Add gesture event handlers
        AddHandler(Gestures.PinchEvent, Border_PinchGesture);
        AddHandler(Gestures.PinchEndedEvent, Border_PinchGestureEnded);
        AddHandler(Gestures.ScrollGestureEvent, Border_ScrollGesture);
        AddHandler(Gestures.ScrollGestureEndedEvent, Border_ScrollGestureEnded);
        
        // Add touch pad gesture handler
        Gestures.AddPointerTouchPadGestureMagnifyHandler(this, Border_Magnified);

        // Update gesture recognizers based on the new state
        UpdateGestureRecognizers();

        _updating = true;
        Invalidate(skipTransitions: false);
        _updating = false;
    }

    private void PanAndZoom_DetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        Log($"[DetachedFromVisualTree] {Name}");
        DetachElement();
        
        // Remove pointer event handlers
        PointerWheelChanged -= Border_PointerWheelChanged;
        PointerPressed -= Border_PointerPressed;
        PointerReleased -= Border_PointerReleased;
        PointerMoved -= Border_PointerMoved;
        PointerCaptureLost -= Border_PointerCaptureLost;
        DoubleTapped -= Border_DoubleTapped;
        
        // Remove gesture event handlers
        RemoveHandler(Gestures.PinchEvent, Border_PinchGesture);
        RemoveHandler(Gestures.PinchEndedEvent, Border_PinchGestureEnded);
        RemoveHandler(Gestures.ScrollGestureEvent, Border_ScrollGesture);
        RemoveHandler(Gestures.ScrollGestureEndedEvent, Border_ScrollGestureEnded);
        
        // Remove touch pad gesture handler
        Gestures.RemovePointerTouchPadGestureMagnifyHandler(this, Border_Magnified);
    }

    private void Border_Magnified(object? sender, PointerDeltaEventArgs e)
    {
        Log($"[Magnified] {Name} {e.Delta}");
        var point = e.GetPosition(_element);
        ZoomDeltaTo(e.Delta.X, point.X, point.Y);
    }

    private void Border_PinchGesture(object? sender, PinchEventArgs e)
    {
        if (!EnableGestures || !EnableGestureZoom || _element == null)
            return;

        Log($"[PinchGesture] {Name} Scale: {e.Scale}");
        
        var point = e.ScaleOrigin;
        var elementPoint = new Point(point.X * _element.Bounds.Width, point.Y * _element.Bounds.Height);
        
        // Raise GestureStarted event
        var previousMatrix = _matrix;
        var gestureArgs = new GestureEventArgs(
            "Pinch",
            _zoomX,
            _zoomY,
            _offsetX,
            _offsetY,
            elementPoint.X,
            elementPoint.Y,
            e.Scale - 1.0,
            _matrix,
            previousMatrix
        );
        RaiseGestureStarted(gestureArgs);
        
        // Calculate zoom delta based on scale change
        var zoomDelta = e.Scale - 1.0;
        ZoomDeltaTo(zoomDelta, elementPoint.X, elementPoint.Y);
        
        e.Handled = true;
    }

    private void Border_PinchGestureEnded(object? sender, PinchEndedEventArgs e)
    {
        if (!EnableGestures)
            return;
            
        Log($"[PinchGestureEnded] {Name}");
        
        // Raise GestureEnded event
        var gestureArgs = new GestureEventArgs(
            "Pinch",
            _zoomX,
            _zoomY,
            _offsetX,
            _offsetY,
            0,
            0,
            0,
            _matrix,
            _matrix
        );
        RaiseGestureEnded(gestureArgs);
        
        e.Handled = true;
    }

    private void Border_ScrollGesture(object? sender, ScrollGestureEventArgs e)
    {
        if (!EnableGestureTranslation || _element == null)
            return;

        Log($"[ScrollGesture] {Name} Delta: {e.Delta}");
        
        // Raise GestureStarted event
        var previousMatrix = _matrix;
        var gestureArgs = new GestureEventArgs(
            "Scroll",
            _zoomX,
            _zoomY,
            _offsetX,
            _offsetY,
            0,
            0,
            Math.Sqrt(e.Delta.X * e.Delta.X + e.Delta.Y * e.Delta.Y),
            _matrix,
            previousMatrix
        );
        RaiseGestureStarted(gestureArgs);
        
        // Use the scroll delta for panning. Scroll gesture delta follows
        // scroll direction semantics (positive = scroll down/right), which is
        // opposite to direct manipulation (content following finger). Invert
        // it so the content moves with the finger on touch/gesture devices.
        PanDelta(-e.Delta.X, -e.Delta.Y);
        
        e.Handled = true;
    }

    private void Border_ScrollGestureEnded(object? sender, ScrollGestureEndedEventArgs e)
    {
        Log($"[ScrollGestureEnded] {Name}");
        
        // Raise GestureEnded event
        var gestureArgs = new GestureEventArgs(
            "Scroll",
            _zoomX,
            _zoomY,
            _offsetX,
            _offsetY,
            0,
            0,
            0,
            _matrix,
            _matrix
        );
        RaiseGestureEnded(gestureArgs);
        
        e.Handled = true;
    }

    private void Border_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        var behavior = GetWheelBehavior(e.KeyModifiers);

        switch (behavior)
        {
            case WheelBehaviorMode.Zoom:
                if (EnableZoom)
                {
                    Wheel(e, WheelZoomSensitivity);
                    e.Handled = true;
                }
                else if (EnablePan && e.KeyModifiers == KeyModifiers.None)
                {
                    // Backward compatibility: If zoom is disabled but pan is enabled,
                    // fall back to panning with the wheel (old behavior)
                    PanDelta(10 * e.Delta.X * WheelPanSensitivity, 10 * e.Delta.Y * WheelPanSensitivity);
                    e.Handled = true;
                }
                break;

            case WheelBehaviorMode.PanVertical:
                if (EnablePan)
                {
                    PanDelta(0, 10 * e.Delta.Y * WheelPanSensitivity);
                    e.Handled = true;
                }
                break;

            case WheelBehaviorMode.PanHorizontal:
                if (EnablePan)
                {
                    PanDelta(10 * e.Delta.X * WheelPanSensitivity, 0);
                    e.Handled = true;
                }
                break;

            case WheelBehaviorMode.None:
                break;
        }
    }

    private WheelBehaviorMode GetWheelBehavior(KeyModifiers modifiers)
    {
        if (modifiers.HasFlag(KeyModifiers.Control))
        {
            return WheelWithCtrl;
        }

        if (modifiers.HasFlag(KeyModifiers.Shift))
        {
            return WheelWithShift;
        }

        return WheelBehavior;
    }

    private void Border_DoubleTapped(object? sender, TappedEventArgs e)
    {
        if (!EnableDoubleClickZoom || _element == null)
            return;

        var point = e.GetPosition(_element);

        switch (DoubleClickZoomMode)
        {
            case DoubleClickZoomMode.ZoomIn:
                ZoomTo(DoubleClickZoomFactor, point.X, point.Y, ShouldAnimate());
                break;

            case DoubleClickZoomMode.ZoomOut:
                ZoomTo(1.0 / DoubleClickZoomFactor, point.X, point.Y, ShouldAnimate());
                break;

            case DoubleClickZoomMode.ZoomInOut:
                // Toggle between zoom in and zoom out based on current zoom level
                if (_zoomX >= _doubleClickZoomThreshold)
                {
                    // Zoom out or reset
                    ResetMatrix(ShouldAnimate());
                }
                else
                {
                    // Zoom in
                    ZoomTo(DoubleClickZoomFactor, point.X, point.Y, ShouldAnimate());
                }
                break;

            case DoubleClickZoomMode.ZoomToFit:
                AutoFit(ShouldAnimate());
                break;

            case DoubleClickZoomMode.None:
                return;
        }

        e.Handled = true;
    }

    private void Border_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        Pressed(e);
    }

    private void Border_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        Released(e);
    }

    private void Border_PointerMoved(object? sender, PointerEventArgs e)
    {
        Moved(e);
    }

    private void Border_PointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        CaptureLost();
        e.Handled = true;
    }

    private void Element_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == BoundsProperty)
        {
            InvalidateScrollable();
        }
    }

    private void BoundsChangedHandler(Rect bounds)
    {
        // Log($"[BoundsChanged] {bounds}");
        InvalidateScrollable();
        HandleResizeBehavior(bounds);
    }

    private void HandleResizeBehavior(Rect newBounds)
    {
        if (_element == null || _sizeBeforeResize == default(Size) || _sizeBeforeResize.Width == 0 || _sizeBeforeResize.Height == 0)
        {
            _sizeBeforeResize = newBounds.Size;
            return;
        }

        var oldSize = _sizeBeforeResize;
        var newSize = newBounds.Size;

        if (oldSize == newSize)
        {
            return;
        }

        switch (ResizeBehavior)
        {
            case ResizeBehaviorMode.None:
                break;

            case ResizeBehaviorMode.MaintainCenter:
                MaintainCenterOnResize(oldSize, newSize);
                break;

            case ResizeBehaviorMode.MaintainTopLeft:
                // Default behavior - do nothing
                break;

            case ResizeBehaviorMode.MaintainZoom:
                MaintainZoomOnResize(oldSize, newSize);
                break;

            case ResizeBehaviorMode.ReapplyStretch:
                AutoFit(skipTransitions: true);
                break;

            case ResizeBehaviorMode.Custom:
                OnResized(oldSize, newSize);
                break;
        }

        _sizeBeforeResize = newSize;
    }

    /// <summary>
    /// Virtual method called when control is resized in Custom resize behavior mode.
    /// </summary>
    /// <param name="oldSize">The previous size.</param>
    /// <param name="newSize">The new size.</param>
    protected virtual void OnResized(Size oldSize, Size newSize)
    {
    }

    private void MaintainCenterOnResize(Size oldSize, Size newSize)
    {
        if (_element == null)
            return;

        var oldCenterX = (oldSize.Width / 2.0 - _offsetX) / _zoomX;
        var oldCenterY = (oldSize.Height / 2.0 - _offsetY) / _zoomY;

        var newOffsetX = newSize.Width / 2.0 - oldCenterX * _zoomX;
        var newOffsetY = newSize.Height / 2.0 - oldCenterY * _zoomY;

        _matrix = MatrixHelper.ScaleAndTranslate(_zoomX, _zoomY, newOffsetX, newOffsetY);
        Invalidate(skipTransitions: true);
    }

    private void MaintainZoomOnResize(Size oldSize, Size newSize)
    {
        if (_element == null)
            return;

        var scaleX = newSize.Width / oldSize.Width;
        var scaleY = newSize.Height / oldSize.Height;

        var newOffsetX = _offsetX * scaleX;
        var newOffsetY = _offsetY * scaleY;

        _matrix = MatrixHelper.ScaleAndTranslate(_zoomX, _zoomY, newOffsetX, newOffsetY);
        Invalidate(skipTransitions: true);
    }

    private void ChildChanged(Control? element)
    {
        Log($"[ChildChanged] {element}");

        if (element != null && element != _element && _element != null)
        {
            DetachElement();
        }

        if (element != null && element != _element)
        {
            AttachElement(element);
        }
    }

    private void AttachElement(Control? element)
    {
        if (element == null)
        {
            return;
        }

        _element = element;
        _element.PropertyChanged += Element_PropertyChanged;

    }

    private void DetachElement()
    {
        if (_element == null)
        {
            return;
        }

        _element.PropertyChanged -= Element_PropertyChanged;
        _element.RenderTransform = null;
        _element = null;
    }

    private void Wheel(PointerWheelEventArgs e, double sensitivity = 1.0)
    {
        if (_element == null || _captured)
        {
            return;
        }
        var point = e.GetPosition(_element);
        ZoomDeltaTo(e.Delta.Y * sensitivity, point.X, point.Y);
    }

    private bool ShouldAnimate()
    {
        return EnableAnimations && AnimationDuration > TimeSpan.Zero;
    }

    private void Pressed(PointerPressedEventArgs e)
    {
        if (!EnablePan)
        {
            return;
        }

        if (_element != null && !_captured && !_isPanning && IsPanButtonPressed(e))
        {
            var point = e.GetPosition(_element);
            BeginPanTo(point.X, point.Y);
            _captured = true;
            _isPanning = true;
            ((IPseudoClasses)Classes).Set(":isPanning", _isPanning);
        }
    }

    // ReSharper disable once UnusedParameter.Local
    private void Released(PointerReleasedEventArgs e) => PanningFinished();

    private void CaptureLost() => PanningFinished();

    private void PanningFinished()
    {
        if (!EnablePan)
        {
            return;
        }
        if (_element == null || _captured != true || _isPanning != true)
        {
            return;
        }
        
        // Raise PanEnded event
        var args = new PanEventArgs(
            _zoomX,
            _zoomY,
            _offsetX,
            _offsetY,
            _offsetX,
            _offsetY,
            0,
            0,
            _matrix,
            _matrix
        );
        RaisePanEnded(args);
        
        _captured = false;
        _isPanning = false;
        ((IPseudoClasses)Classes).Set(":isPanning", _isPanning);
    }

    private void Moved(PointerEventArgs e)
    {
        if (!EnablePan)
        {
            return;
        }

        if (_element == null || _captured != true || _isPanning != true || !IsPanButtonPressed(e))
        {
            return;
        }

        var point = e.GetPosition(_element);
        ContinuePanTo(point.X, point.Y, true);
    }

    private bool IsPanButtonPressed(PointerEventArgs e)
    {
        var button = PanButton;
        var properties = e.GetCurrentPoint(this).Properties;
        return (properties.IsLeftButtonPressed && button == ButtonName.Left)
            || (properties.IsRightButtonPressed && button == ButtonName.Right)
            || (properties.IsMiddleButtonPressed && button == ButtonName.Middle);
    }

    /// <summary>
    /// Raises <see cref="ZoomChanged"/> event.
    /// </summary>
    /// <param name="e">Zoom changed event arguments.</param>
    protected virtual void OnZoomChanged(ZoomChangedEventArgs e)
    {
        ZoomChanged?.Invoke(this, e);
    }

    private void RaiseZoomChanged()
    {
        var args = new ZoomChangedEventArgs(_zoomX, _zoomY, _offsetX, _offsetY);
        OnZoomChanged(args);
    }

    private void Constrain()
    {
        var zoomX = ClampValue(_matrix.M11, MinZoomX, MaxZoomX);
        var zoomY = ClampValue(_matrix.M22, MinZoomY, MaxZoomY);
        var offsetX = ClampValue(_matrix.M31, MinOffsetX, MaxOffsetX);
        var offsetY = ClampValue(_matrix.M32, MinOffsetY, MaxOffsetY);
        _matrix = new Matrix(zoomX, 0.0, 0.0, zoomY, offsetX, offsetY);

        // Apply content bounds restriction
        ApplyContentBoundsRestriction();
    }

    private void ApplyContentBoundsRestriction()
    {
        if (BoundsMode == ContentBoundsMode.Unrestricted || _element == null)
            return;

        switch (BoundsMode)
        {
            case ContentBoundsMode.KeepContentVisible:
                ApplyKeepContentVisible();
                break;

            case ContentBoundsMode.FillViewport:
                ApplyFillViewport();
                break;

            case ContentBoundsMode.KeepCentered:
                ApplyKeepCentered();
                break;

            case ContentBoundsMode.Custom:
                var customBounds = GetContentBounds();
                ApplyCustomBounds(customBounds);
                break;
        }
    }

    /// <summary>
    /// Virtual method to get custom content bounds.
    /// </summary>
    /// <returns>The content bounds rectangle.</returns>
    protected virtual Rect GetContentBounds()
    {
        return _element?.Bounds ?? new Rect();
    }

    /// <summary>
    /// Virtual method to validate transform matrix.
    /// </summary>
    /// <param name="newMatrix">The proposed new matrix.</param>
    /// <returns>True if the matrix is valid, false otherwise.</returns>
    protected virtual bool ValidateTransform(Matrix newMatrix)
    {
        return true;
    }

    private void ApplyKeepContentVisible()
    {
        if (_element == null)
            return;

        var contentWidth = _element.Bounds.Width * _matrix.M11;
        var contentHeight = _element.Bounds.Height * _matrix.M22;
        var viewportWidth = Bounds.Width;
        var viewportHeight = Bounds.Height;

        var minVisibleWidth = contentWidth * MinimumVisibleContentPercentage;
        var minVisibleHeight = contentHeight * MinimumVisibleContentPercentage;

        var offsetX = _matrix.M31;
        var offsetY = _matrix.M32;

        // Apply padding
        var padding = BoundsPadding;

        // Constrain X
        var maxOffsetX = viewportWidth - minVisibleWidth + padding.Right;
        var minOffsetX = -contentWidth + minVisibleWidth - padding.Left;
        offsetX = ClampValue(offsetX, minOffsetX, maxOffsetX);

        // Constrain Y
        var maxOffsetY = viewportHeight - minVisibleHeight + padding.Bottom;
        var minOffsetY = -contentHeight + minVisibleHeight - padding.Top;
        offsetY = ClampValue(offsetY, minOffsetY, maxOffsetY);

        _matrix = new Matrix(_matrix.M11, 0.0, 0.0, _matrix.M22, offsetX, offsetY);
    }

    private void ApplyFillViewport()
    {
        if (_element == null)
            return;

        var contentWidth = _element.Bounds.Width * _matrix.M11;
        var contentHeight = _element.Bounds.Height * _matrix.M22;
        var viewportWidth = Bounds.Width;
        var viewportHeight = Bounds.Height;

        var offsetX = _matrix.M31;
        var offsetY = _matrix.M32;

        // If content is smaller than viewport, center it
        if (contentWidth <= viewportWidth)
        {
            offsetX = (viewportWidth - contentWidth) / 2.0;
        }
        else
        {
            // Constrain so no empty space is visible
            offsetX = ClampValue(offsetX, viewportWidth - contentWidth, 0);
        }

        if (contentHeight <= viewportHeight)
        {
            offsetY = (viewportHeight - contentHeight) / 2.0;
        }
        else
        {
            // Constrain so no empty space is visible
            offsetY = ClampValue(offsetY, viewportHeight - contentHeight, 0);
        }

        _matrix = new Matrix(_matrix.M11, 0.0, 0.0, _matrix.M22, offsetX, offsetY);
    }

    private void ApplyKeepCentered()
    {
        if (_element == null)
            return;

        var contentWidth = _element.Bounds.Width * _matrix.M11;
        var contentHeight = _element.Bounds.Height * _matrix.M22;
        var viewportWidth = Bounds.Width;
        var viewportHeight = Bounds.Height;

        var offsetX = (viewportWidth - contentWidth) / 2.0;
        var offsetY = (viewportHeight - contentHeight) / 2.0;

        _matrix = new Matrix(_matrix.M11, 0.0, 0.0, _matrix.M22, offsetX, offsetY);
    }

    private void ApplyCustomBounds(Rect customBounds)
    {
        // Subclasses can override GetContentBounds and ValidateTransform
        // for custom bounds logic
    }

    /// <summary>
    /// Invalidate pan and zoom control.
    /// </summary>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    private void Invalidate(bool skipTransitions = false)
    {
        Log("[Invalidate] Begin");

        if (_element == null)
        {
            Log("[Invalidate] End");
            return;
        }

        if (EnableConstrains)
        {
            Constrain();
        }

        InvalidateProperties();
        InvalidateScrollable();
        InvalidateElement(skipTransitions);
        RaiseZoomChanged();

        Log("[Invalidate] End");
    }

    /// <summary>
    /// Invalidate properties.
    /// </summary>
    private void InvalidateProperties()
    {
        SetAndRaise(ZoomXProperty, ref _zoomX, _matrix.M11);
        SetAndRaise(ZoomYProperty, ref _zoomY, _matrix.M22);
        SetAndRaise(OffsetXProperty, ref _offsetX, _matrix.M31);
        SetAndRaise(OffsetYProperty, ref _offsetY, _matrix.M32);
    }

    /// <summary>
    /// Invalidate child element.
    /// </summary>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    private void InvalidateElement(bool skipTransitions)
    {
        if (_element == null)
        {
            return;
        }

        Animation.Transitions? backupTransitions = null;

        if (skipTransitions)
        {
            Animation.Animatable? anim = _element;

            if (anim != null)
            {
                backupTransitions = anim.Transitions;
                anim.Transitions = null;
            }
        }

        _element.RenderTransformOrigin = new RelativePoint(new Point(0, 0), RelativeUnit.Relative);
        _transformBuilder = new TransformOperations.Builder(1);
        _transformBuilder.AppendMatrix(_matrix);
        _element.RenderTransform = _transformBuilder.Build();

        if (skipTransitions && backupTransitions != null)
        {
            Animation.Animatable? anim = _element;

            if (anim != null)
            {
                anim.Transitions = backupTransitions;
            }
        }

        _element.InvalidateVisual();
    }

    /// <summary>
    /// Set pan and zoom matrix.
    /// </summary>
    /// <param name="matrix">The matrix to set as current.</param>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void SetMatrix(Matrix matrix, bool skipTransitions = false)
    {
        if (_updating)
        {
            return;
        }
        _updating = true;

        Log("[SetMatrix]");
        var previousMatrix = _matrix;
        _matrix = matrix;
        Invalidate(skipTransitions);

        // Raise MatrixChanged event
        var args = new MatrixChangedEventArgs(
            _matrix,
            previousMatrix,
            _zoomX,
            _zoomY,
            _offsetX,
            _offsetY,
            previousMatrix.M11,
            previousMatrix.M22,
            previousMatrix.M31,
            previousMatrix.M32,
            "SetMatrix"
        );
        RaiseMatrixChanged(args);

        _updating = false;
    }

    /// <summary>
    /// Reset pan and zoom matrix.
    /// </summary>
    public void ResetMatrix()
    {
        ResetMatrix(false);
    }

    /// <summary>
    /// Reset pan and zoom matrix.
    /// </summary>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void ResetMatrix(bool skipTransitions)
    {
        var previousMatrix = _matrix;
        SetMatrix(Matrix.Identity, skipTransitions);
        
        // Raise MatrixReset event
        var args = new MatrixChangedEventArgs(
            Matrix.Identity,
            previousMatrix,
            1.0,
            1.0,
            0.0,
            0.0,
            previousMatrix.M11,
            previousMatrix.M22,
            previousMatrix.M31,
            previousMatrix.M32,
            "ResetMatrix"
        );
        RaiseMatrixReset(args);
    }

    /// <summary>
    /// Zoom to provided zoom value and provided center point.
    /// </summary>
    /// <param name="zoom">The zoom value.</param>
    /// <param name="x">The center point x axis coordinate.</param>
    /// <param name="y">The center point y axis coordinate.</param>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void Zoom(double zoom, double x, double y, bool skipTransitions = false)
    {
        if (_updating)
        {
            return;
        }
        _updating = true;

        Log("[Zoom]");
        var previousMatrix = _matrix;
        var previousZoomX = _zoomX;
        var previousZoomY = _zoomY;
        
        _matrix = MatrixHelper.ScaleAt(zoom, zoom, x, y);
        Invalidate(skipTransitions);

        // Raise ZoomStarted event
        var args = new ZoomEventArgs(
            _zoomX,
            _zoomY,
            previousZoomX,
            previousZoomY,
            zoom / previousZoomX,
            x,
            y,
            _offsetX,
            _offsetY,
            _matrix,
            previousMatrix
        );
        RaiseZoomStarted(args);

        _updating = false;
    }

    /// <summary>
    /// Zoom to provided zoom ratio and provided center point.
    /// </summary>
    /// <param name="ratio">The zoom ratio.</param>
    /// <param name="x">The center point x axis coordinate.</param>
    /// <param name="y">The center point y axis coordinate.</param>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void ZoomTo(double ratio, double x, double y, bool skipTransitions = false)
    {
        if (_updating)
        {
            return;
        }

        if ((ZoomX >= MaxZoomX && ZoomY >= MaxZoomY && ratio > 1) || (ZoomX <= MinZoomX && ZoomY <= MinZoomY && ratio < 1))
        {
            return;
        }

        _updating = true;

        Log("[ZoomTo]");
        var previousMatrix = _matrix;
        var previousZoomX = _zoomX;
        var previousZoomY = _zoomY;
        
        _matrix = MatrixHelper.ScaleAtPrepend(_matrix, ratio, ratio, x, y);
        Invalidate(skipTransitions);

        // Raise ZoomDeltaChanged event
        var args = new ZoomEventArgs(
            _zoomX,
            _zoomY,
            previousZoomX,
            previousZoomY,
            ratio,
            x,
            y,
            _offsetX,
            _offsetY,
            _matrix,
            previousMatrix
        );
        RaiseZoomDeltaChanged(args);

        _updating = false;
    }

    /// <summary>
    /// Zoom in one step positive delta ratio and panel center point.
    /// </summary>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void ZoomIn(bool skipTransitions = false)
    {
        if (_element == null)
        {
            return;
        }

        var previousZoomX = _zoomX;
        var previousZoomY = _zoomY;
        var previousMatrix = _matrix;
        
        var x = _element.Bounds.Width / 2.0;
        var y = _element.Bounds.Height / 2.0;
        ZoomTo(ZoomSpeed, x, y, skipTransitions);
        
        // Raise ZoomEnded event
        var args = new ZoomEventArgs(
            _zoomX,
            _zoomY,
            previousZoomX,
            previousZoomY,
            ZoomSpeed,
            x,
            y,
            _offsetX,
            _offsetY,
            _matrix,
            previousMatrix
        );
        RaiseZoomEnded(args);
    }

    /// <summary>
    /// Zoom out one step positive delta ratio and panel center point.
    /// </summary>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void ZoomOut(bool skipTransitions = false)
    {
        if (_element == null)
        {
            return;
        }

        var previousZoomX = _zoomX;
        var previousZoomY = _zoomY;
        var previousMatrix = _matrix;
        
        var x = _element.Bounds.Width / 2.0;
        var y = _element.Bounds.Height / 2.0;
        ZoomTo(1 / ZoomSpeed, x, y, skipTransitions);
        
        // Raise ZoomEnded event
        var args = new ZoomEventArgs(
            _zoomX,
            _zoomY,
            previousZoomX,
            previousZoomY,
            1 / ZoomSpeed,
            x,
            y,
            _offsetX,
            _offsetY,
            _matrix,
            previousMatrix
        );
        RaiseZoomEnded(args);
    }

    /// <summary>
    /// Zoom to provided zoom delta ratio and provided center point.
    /// </summary>
    /// <param name="delta">The zoom delta ratio.</param>
    /// <param name="x">The center point x axis coordinate.</param>
    /// <param name="y">The center point y axis coordinate.</param>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void ZoomDeltaTo(double delta, double x, double y, bool skipTransitions = false)
    {
        double realDelta = Sign(delta) * Pow(Abs(delta), PowerFactor);
        ZoomTo(Pow(ZoomSpeed, realDelta), x, y, skipTransitions || Abs(realDelta) <= TransitionThreshold);
    }

    /// <summary>
    /// Pan control to provided delta.
    /// </summary>
    /// <param name="dx">The target x axis delta.</param>
    /// <param name="dy">The target y axis delta.</param>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void PanDelta(double dx, double dy, bool skipTransitions = false)
    {
        if (_updating)
        {
            return;
        }
        _updating = true;

        Log("[PanDelta]");
        var previousMatrix = _matrix;
        var previousOffsetX = _offsetX;
        var previousOffsetY = _offsetY;
        
        _matrix = MatrixHelper.ScaleAndTranslate(_zoomX, _zoomY, _matrix.M31 + dx, _matrix.M32 + dy);
        Invalidate(skipTransitions);

        // Raise PanContinued event
        var args = new PanEventArgs(
            _zoomX,
            _zoomY,
            _offsetX,
            _offsetY,
            previousOffsetX,
            previousOffsetY,
            dx,
            dy,
            _matrix,
            previousMatrix
        );
        RaisePanContinued(args);

        _updating = false;
    }

    /// <summary>
    /// Pan control to provided target point.
    /// </summary>
    /// <param name="x">The target point x axis coordinate.</param>
    /// <param name="y">The target point y axis coordinate.</param>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void Pan(double x, double y, bool skipTransitions = false)
    {
        if (_updating)
        {
            return;
        }
        _updating = true;

        Log("[Pan]");
        var previousMatrix = _matrix;
        var previousOffsetX = _offsetX;
        var previousOffsetY = _offsetY;
        
        _matrix = MatrixHelper.ScaleAndTranslate(_zoomX, _zoomY, x, y);
        Invalidate(skipTransitions);

        // Raise PanContinued event
        var args = new PanEventArgs(
            _zoomX,
            _zoomY,
            _offsetX,
            _offsetY,
            previousOffsetX,
            previousOffsetY,
            x - previousOffsetX,
            y - previousOffsetY,
            _matrix,
            previousMatrix
        );
        RaisePanContinued(args);

        _updating = false;
    }

    /// <summary>
    /// Set pan origin.
    /// </summary>
    /// <param name="x">The origin point x axis coordinate.</param>
    /// <param name="y">The origin point y axis coordinate.</param>
    public void BeginPanTo(double x, double y)
    {
        _pan = new Point();
        _previous = new Point(x, y);
        
        // Raise PanStarted event
        var args = new PanEventArgs(
            _zoomX,
            _zoomY,
            _offsetX,
            _offsetY,
            _offsetX,
            _offsetY,
            0.0,
            0.0,
            _matrix,
            _matrix
        );
        RaisePanStarted(args);
    }

    /// <summary>
    /// Continue pan to provided target point.
    /// </summary>
    /// <param name="x">The target point x axis coordinate.</param>
    /// <param name="y">The target point y axis coordinate.</param>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void ContinuePanTo(double x, double y, bool skipTransitions = false)
    {
        if (_updating)
        {
            return;
        }
        _updating = true;

        Log("[ContinuePanTo]");
        var previousMatrix = _matrix;
        var previousOffsetX = _offsetX;
        var previousOffsetY = _offsetY;
        
        var dx = x - _previous.X;
        var dy = y - _previous.Y;
        var delta = new Point(dx, dy);
        _previous = new Point(x, y);
        _pan = new Point(_pan.X + delta.X, _pan.Y + delta.Y);
        _matrix = MatrixHelper.TranslatePrepend(_matrix, _pan.X, _pan.Y);
        Invalidate(skipTransitions);

        // Raise PanContinued event
        var args = new PanEventArgs(
            _zoomX,
            _zoomY,
            _offsetX,
            _offsetY,
            previousOffsetX,
            previousOffsetY,
            dx,
            dy,
            _matrix,
            previousMatrix
        );
        RaisePanContinued(args);

        _updating = false;
    }

    /// <summary>
    /// Zoom and pan.
    /// </summary>
    /// <param name="panelWidth">The panel width.</param>
    /// <param name="panelHeight">The panel height.</param>
    /// <param name="elementWidth">The element width.</param>
    /// <param name="elementHeight">The element height.</param>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void None(double panelWidth, double panelHeight, double elementWidth, double elementHeight, bool skipTransitions = false)
    {
        if (_updating)
        {
            return;
        }
        _updating = true;

        Log($"[None] {panelWidth}x{panelHeight} {elementWidth}x{elementHeight}");
        if (_element == null)
        {
            _updating = false;
            return;
        }

        _matrix = CalculateMatrix(panelWidth, panelHeight, elementWidth, elementHeight, StretchMode.None);
        Invalidate(skipTransitions);

        _updating = false;
    }

    /// <summary>
    /// Zoom and pan to fill panel.
    /// </summary>
    /// <param name="panelWidth">The panel width.</param>
    /// <param name="panelHeight">The panel height.</param>
    /// <param name="elementWidth">The element width.</param>
    /// <param name="elementHeight">The element height.</param>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void Fill(double panelWidth, double panelHeight, double elementWidth, double elementHeight, bool skipTransitions = false)
    {
        if (_updating)
        {
            return;
        }
        _updating = true;

        Log($"[Fill] {panelWidth}x{panelHeight} {elementWidth}x{elementHeight}");
        if (_element == null)
        {
            _updating = false;
            return;
        }

        _matrix = CalculateMatrix(panelWidth, panelHeight, elementWidth, elementHeight, StretchMode.Fill);
        Invalidate(skipTransitions);

        _updating = false;
    }

    /// <summary>
    /// Zoom and pan to panel extents while maintaining aspect ratio.
    /// </summary>
    /// <param name="panelWidth">The panel width.</param>
    /// <param name="panelHeight">The panel height.</param>
    /// <param name="elementWidth">The element width.</param>
    /// <param name="elementHeight">The element height.</param>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void Uniform(double panelWidth, double panelHeight, double elementWidth, double elementHeight, bool skipTransitions = false)
    {
        if (_updating)
        {
            return;
        }
        _updating = true;

        Log($"[Uniform] {panelWidth}x{panelHeight} {elementWidth}x{elementHeight}");
        if (_element == null)
        {
            _updating = false;
            return;
        }

        _matrix = CalculateMatrix(panelWidth, panelHeight, elementWidth, elementHeight, StretchMode.Uniform);
        Invalidate(skipTransitions);

        _updating = false;
    }

    /// <summary>
    /// Zoom and pan to panel extents while maintaining aspect ratio. If aspect of panel is different panel is filled.
    /// </summary>
    /// <param name="panelWidth">The panel width.</param>
    /// <param name="panelHeight">The panel height.</param>
    /// <param name="elementWidth">The element width.</param>
    /// <param name="elementHeight">The element height.</param>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void UniformToFill(double panelWidth, double panelHeight, double elementWidth, double elementHeight, bool skipTransitions = false)
    {
        if (_updating)
        {
            return;
        }
        _updating = true;

        Log($"[UniformToFill] {panelWidth}x{panelHeight} {elementWidth}x{elementHeight}");
        if (_element == null)
        {
            _updating = false;
            return;
        }

        _matrix = CalculateMatrix(panelWidth, panelHeight, elementWidth, elementHeight, StretchMode.UniformToFill);
        Invalidate(skipTransitions);

        _updating = false;
    }

    /// <summary>
    /// Zoom and pan child element inside panel using stretch mode.
    /// </summary>
    /// <param name="panelWidth">The panel width.</param>
    /// <param name="panelHeight">The panel height.</param>
    /// <param name="elementWidth">The element width.</param>
    /// <param name="elementHeight">The element height.</param>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void AutoFit(double panelWidth, double panelHeight, double elementWidth, double elementHeight, bool skipTransitions = false)
    {
        if (_element == null)
        {
            return;
        }
        switch (Stretch)
        {
            case StretchMode.Fill:
                Fill(panelWidth, panelHeight, elementWidth, elementHeight, skipTransitions);
                break;
            case StretchMode.Uniform:
                Uniform(panelWidth, panelHeight, elementWidth, elementHeight, skipTransitions);
                break;
            case StretchMode.UniformToFill:
                UniformToFill(panelWidth, panelHeight, elementWidth, elementHeight, skipTransitions);
                break;
            case StretchMode.None:
                break;
        }
    }

    /// <summary>
    /// Set next stretch mode.
    /// </summary>
    public void ToggleStretchMode()
    {
        switch (Stretch)
        {
            case StretchMode.None:
                Stretch = StretchMode.Fill;
                break;
            case StretchMode.Fill:
                Stretch = StretchMode.Uniform;
                break;
            case StretchMode.Uniform:
                Stretch = StretchMode.UniformToFill;
                break;
            case StretchMode.UniformToFill:
                Stretch = StretchMode.None;
                break;
        }
    }

    /// <summary>
    /// Zoom and pan.
    /// </summary>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void None(bool skipTransitions = false)
    {
        if (_element == null)
        {
            return;
        }
        None(Bounds.Width, Bounds.Height, _element.Bounds.Width, _element.Bounds.Height, skipTransitions);
    }

    /// <summary>
    /// Zoom and pan to fill panel.
    /// </summary>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void Fill(bool skipTransitions = false)
    {
        if (_element == null)
        {
            return;
        }
        var previousStretch = Stretch;
        var previousMatrix = _matrix;
        
        Fill(Bounds.Width, Bounds.Height, _element.Bounds.Width, _element.Bounds.Height, skipTransitions);
        
        // Raise StretchModeChanged event
        var args = new StretchModeChangedEventArgs(
            StretchMode.Fill,
            previousStretch,
            _matrix,
            previousMatrix,
            _zoomX,
            _zoomY,
            _offsetX,
            _offsetY,
            Bounds.Width,
            Bounds.Height,
            _element.Bounds.Width,
            _element.Bounds.Height
        );
        RaiseStretchModeChanged(args);
    }

    /// <summary>
    /// Zoom and pan to panel extents while maintaining aspect ratio.
    /// </summary>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void Uniform(bool skipTransitions = false)
    {
        if (_element == null)
        {
            return;
        }
        var previousStretch = Stretch;
        var previousMatrix = _matrix;
        
        Uniform(Bounds.Width, Bounds.Height, _element.Bounds.Width, _element.Bounds.Height, skipTransitions);
        
        // Raise StretchModeChanged event
        var args = new StretchModeChangedEventArgs(
            StretchMode.Uniform,
            previousStretch,
            _matrix,
            previousMatrix,
            _zoomX,
            _zoomY,
            _offsetX,
            _offsetY,
            Bounds.Width,
            Bounds.Height,
            _element.Bounds.Width,
            _element.Bounds.Height
        );
        RaiseStretchModeChanged(args);
    }

    /// <summary>
    /// Zoom and pan to panel extents while maintaining aspect ratio. If aspect of panel is different panel is filled.
    /// </summary>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void UniformToFill(bool skipTransitions = false)
    {
        if (_element == null)
        {
            return;
        }
        var previousStretch = Stretch;
        var previousMatrix = _matrix;
        
        UniformToFill(Bounds.Width, Bounds.Height, _element.Bounds.Width, _element.Bounds.Height, skipTransitions);
        
        // Raise StretchModeChanged event
        var args = new StretchModeChangedEventArgs(
            StretchMode.UniformToFill,
            previousStretch,
            _matrix,
            previousMatrix,
            _zoomX,
            _zoomY,
            _offsetX,
            _offsetY,
            Bounds.Width,
            Bounds.Height,
            _element.Bounds.Width,
            _element.Bounds.Height
        );
        RaiseStretchModeChanged(args);
    }

    /// <summary>
    /// Zoom and pan child element inside panel using stretch mode.
    /// </summary>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void AutoFit(bool skipTransitions = false)
    {
        if (_element == null)
        {
            return;
        }
        var previousMatrix = _matrix;
        
        AutoFit(Bounds.Width, Bounds.Height, _element.Bounds.Width, _element.Bounds.Height, skipTransitions);
        
        // Raise AutoFitApplied event
        var args = new StretchModeChangedEventArgs(
            Stretch,
            Stretch,
            _matrix,
            previousMatrix,
            _zoomX,
            _zoomY,
            _offsetX,
            _offsetY,
            Bounds.Width,
            Bounds.Height,
            _element.Bounds.Width,
            _element.Bounds.Height
        );
        RaiseAutoFitApplied(args);
    }

    /// <summary>
    /// Raises the PanStarted event.
    /// </summary>
    /// <param name="args">The pan event arguments.</param>
    protected virtual void RaisePanStarted(PanEventArgs args)
    {
        PanStarted?.Invoke(this, args);
    }

    /// <summary>
    /// Raises the PanContinued event.
    /// </summary>
    /// <param name="args">The pan event arguments.</param>
    protected virtual void RaisePanContinued(PanEventArgs args)
    {
        PanContinued?.Invoke(this, args);
    }

    /// <summary>
    /// Raises the PanEnded event.
    /// </summary>
    /// <param name="args">The pan event arguments.</param>
    protected virtual void RaisePanEnded(PanEventArgs args)
    {
        PanEnded?.Invoke(this, args);
    }

    /// <summary>
    /// Raises the ZoomStarted event.
    /// </summary>
    /// <param name="args">The zoom event arguments.</param>
    protected virtual void RaiseZoomStarted(ZoomEventArgs args)
    {
        ZoomStarted?.Invoke(this, args);
    }

    /// <summary>
    /// Raises the ZoomEnded event.
    /// </summary>
    /// <param name="args">The zoom event arguments.</param>
    protected virtual void RaiseZoomEnded(ZoomEventArgs args)
    {
        ZoomEnded?.Invoke(this, args);
    }

    /// <summary>
    /// Raises the ZoomDeltaChanged event.
    /// </summary>
    /// <param name="args">The zoom event arguments.</param>
    protected virtual void RaiseZoomDeltaChanged(ZoomEventArgs args)
    {
        ZoomDeltaChanged?.Invoke(this, args);
    }

    /// <summary>
    /// Raises the MatrixChanged event.
    /// </summary>
    /// <param name="args">The matrix changed event arguments.</param>
    protected virtual void RaiseMatrixChanged(MatrixChangedEventArgs args)
    {
        MatrixChanged?.Invoke(this, args);
    }

    /// <summary>
    /// Raises the MatrixReset event.
    /// </summary>
    /// <param name="args">The matrix changed event arguments.</param>
    protected virtual void RaiseMatrixReset(MatrixChangedEventArgs args)
    {
        MatrixReset?.Invoke(this, args);
    }

    /// <summary>
    /// Raises the StretchModeChanged event.
    /// </summary>
    /// <param name="args">The stretch mode changed event arguments.</param>
    protected virtual void RaiseStretchModeChanged(StretchModeChangedEventArgs args)
    {
        StretchModeChanged?.Invoke(this, args);
    }

    /// <summary>
    /// Raises the AutoFitApplied event.
    /// </summary>
    /// <param name="args">The stretch mode changed event arguments.</param>
    protected virtual void RaiseAutoFitApplied(StretchModeChangedEventArgs args)
    {
        AutoFitApplied?.Invoke(this, args);
    }

    /// <summary>
    /// Raises the GestureStarted event.
    /// </summary>
    /// <param name="args">The gesture event arguments.</param>
    protected virtual void RaiseGestureStarted(GestureEventArgs args)
    {
        GestureStarted?.Invoke(this, args);
    }

    /// <summary>
    /// Raises the GestureEnded event.
    /// </summary>
    /// <param name="args">The gesture event arguments.</param>
    protected virtual void RaiseGestureEnded(GestureEventArgs args)
    {
        GestureEnded?.Invoke(this, args);
    }

}
