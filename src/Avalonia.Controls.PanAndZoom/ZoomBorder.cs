﻿using System;
using System.Diagnostics;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media;
using static System.Math;

namespace Avalonia.Controls.PanAndZoom
{
    /// <summary>
    /// Zoom changed event arguments.
    /// </summary>
    public class ZoomChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the zoom ratio for x axis.
        /// </summary>
        public double ZoomX { get; set; }
        
        /// <summary>
        /// Gets the zoom ratio for y axis.
        /// </summary>
        public double ZoomY { get; set; }
        
        /// <summary>
        /// Gets the pan offset for x axis.
        /// </summary>
        public double OffsetX { get; set; }
        
        /// <summary>
        /// Gets the pan offset for y axis.
        /// </summary>
        public double OffsetY { get; set; }
    }

    /// <summary>
    /// Zoom changed event handler.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">Zoom changed event arguments.</param>
    public delegate void ZoomChangedEventHandler(object sender, ZoomChangedEventArgs e);

    /// <summary>
    /// Pan and zoom control for Avalonia.
    /// </summary>
    public class ZoomBorder : Border, ILogicalScrollable
    {
        /// <summary>
        /// Gets available stretch modes.
        /// </summary>
        public static StretchMode[] StretchModes { get; } = (StretchMode[])Enum.GetValues(typeof(StretchMode));

        /// <summary>
        /// Gets available button names.
        /// </summary>
        public static ButtonName[] ButtonNames { get; } = (ButtonName[])Enum.GetValues(typeof(ButtonName));

        /// <summary>
        /// Identifies the <seealso cref="PanButton"/> avalonia property.
        /// </summary>
        public static StyledProperty<ButtonName> PanButtonProperty =
            AvaloniaProperty.Register<ZoomBorder, ButtonName>(nameof(PanButton), ButtonName.Middle, false, BindingMode.TwoWay);

        /// <summary>
        /// Identifies the <seealso cref="ZoomSpeed"/> avalonia property.
        /// </summary>
        public static StyledProperty<double> ZoomSpeedProperty =
            AvaloniaProperty.Register<ZoomBorder, double>(nameof(ZoomSpeed), 1.2, false, BindingMode.TwoWay);

        /// <summary>
        /// Identifies the <seealso cref="Stretch"/> avalonia property.
        /// </summary>
        public static StyledProperty<StretchMode> StretchProperty =
            AvaloniaProperty.Register<ZoomBorder, StretchMode>(nameof(Stretch), StretchMode.Uniform, false, BindingMode.TwoWay);

        /// <summary>
        /// Identifies the <seealso cref="ZoomX"/> avalonia property.
        /// </summary>
        public static readonly DirectProperty<ZoomBorder, double> ZoomXProperty =
            AvaloniaProperty.RegisterDirect<ZoomBorder, double>(
                nameof(ZoomX),
                o => o.ZoomX,
                null,
                1.0);

        /// <summary>
        /// Identifies the <seealso cref="ZoomY"/> avalonia property.
        /// </summary>
        public static readonly DirectProperty<ZoomBorder, double> ZoomYProperty =
            AvaloniaProperty.RegisterDirect<ZoomBorder, double>(
                nameof(ZoomY),
                o => o.ZoomY,
                null,
                1.0);

        /// <summary>
        /// Identifies the <seealso cref="OffsetX"/> avalonia property.
        /// </summary>
        public static readonly DirectProperty<ZoomBorder, double> OffsetXProperty =
            AvaloniaProperty.RegisterDirect<ZoomBorder, double>(
                nameof(OffsetX),
                o => o.OffsetX,
                null,
                0.0);

        /// <summary>
        /// Identifies the <seealso cref="OffsetY"/> avalonia property.
        /// </summary>
        public static readonly DirectProperty<ZoomBorder, double> OffsetYProperty =
            AvaloniaProperty.RegisterDirect<ZoomBorder, double>(
                nameof(OffsetY),
                o => o.OffsetY,
                null,
                0.0);

        /// <summary>
        /// Identifies the <seealso cref="EnableConstrains"/> avalonia property.
        /// </summary>
        public static StyledProperty<bool> EnableConstrainsProperty =
            AvaloniaProperty.Register<ZoomBorder, bool>(nameof(EnableConstrains), true, false, BindingMode.TwoWay);

        /// <summary>
        /// Identifies the <seealso cref="MinZoomX"/> avalonia property.
        /// </summary>
        public static StyledProperty<double> MinZoomXProperty =
            AvaloniaProperty.Register<ZoomBorder, double>(nameof(MinZoomX), double.NegativeInfinity, false, BindingMode.TwoWay);

        /// <summary>
        /// Identifies the <seealso cref="MaxZoomX"/> avalonia property.
        /// </summary>
        public static StyledProperty<double> MaxZoomXProperty =
            AvaloniaProperty.Register<ZoomBorder, double>(nameof(MaxZoomX), double.PositiveInfinity, false, BindingMode.TwoWay);

        /// <summary>
        /// Identifies the <seealso cref="MinZoomY"/> avalonia property.
        /// </summary>
        public static StyledProperty<double> MinZoomYProperty =
            AvaloniaProperty.Register<ZoomBorder, double>(nameof(MinZoomY), double.NegativeInfinity, false, BindingMode.TwoWay);

        /// <summary>
        /// Identifies the <seealso cref="MaxZoomY"/> avalonia property.
        /// </summary>
        public static StyledProperty<double> MaxZoomYProperty =
            AvaloniaProperty.Register<ZoomBorder, double>(nameof(MaxZoomY), double.PositiveInfinity, false, BindingMode.TwoWay);

        /// <summary>
        /// Identifies the <seealso cref="MinOffsetX"/> avalonia property.
        /// </summary>
        public static StyledProperty<double> MinOffsetXProperty =
            AvaloniaProperty.Register<ZoomBorder, double>(nameof(MinOffsetX), double.NegativeInfinity, false, BindingMode.TwoWay);

        /// <summary>
        /// Identifies the <seealso cref="MaxOffsetX"/> avalonia property.
        /// </summary>
        public static StyledProperty<double> MaxOffsetXProperty =
            AvaloniaProperty.Register<ZoomBorder, double>(nameof(MaxOffsetX), double.PositiveInfinity, false, BindingMode.TwoWay);

        /// <summary>
        /// Identifies the <seealso cref="MinOffsetY"/> avalonia property.
        /// </summary>
        public static StyledProperty<double> MinOffsetYProperty =
            AvaloniaProperty.Register<ZoomBorder, double>(nameof(MinOffsetY), double.NegativeInfinity, false, BindingMode.TwoWay);

        /// <summary>
        /// Identifies the <seealso cref="MaxOffsetY"/> avalonia property.
        /// </summary>
        public static StyledProperty<double> MaxOffsetYProperty =
            AvaloniaProperty.Register<ZoomBorder, double>(nameof(MaxOffsetY), double.PositiveInfinity, false, BindingMode.TwoWay);

        /// <summary>
        /// Identifies the <seealso cref="EnablePan"/> avalonia property.
        /// </summary>
        public static StyledProperty<bool> EnablePanProperty =
            AvaloniaProperty.Register<ZoomBorder, bool>(nameof(EnablePan), true, false, BindingMode.TwoWay);

        /// <summary>
        /// Identifies the <seealso cref="EnableZoom"/> avalonia property.
        /// </summary>
        public static StyledProperty<bool> EnableZoomProperty =
            AvaloniaProperty.Register<ZoomBorder, bool>(nameof(EnableZoom), true, false, BindingMode.TwoWay);

        /// <summary>
        /// Identifies the <seealso cref="EnableGestureZoom"/> avalonia property.
        /// </summary>
        public static StyledProperty<bool> EnableGestureZoomProperty =
            AvaloniaProperty.Register<ZoomBorder, bool>(nameof(EnableGestureZoom), true, false, BindingMode.TwoWay);

        /// <summary>
        /// Identifies the <seealso cref="EnableGestureRotation"/> avalonia property.
        /// </summary>
        public static StyledProperty<bool> EnableGestureRotationProperty =
            AvaloniaProperty.Register<ZoomBorder, bool>(nameof(EnableGestureRotation), true, false, BindingMode.TwoWay);

        /// <summary>
        /// Identifies the <seealso cref="EnableGestureTranslation"/> avalonia property.
        /// </summary>
        public static StyledProperty<bool> EnableGestureTranslationProperty =
            AvaloniaProperty.Register<ZoomBorder, bool>(nameof(EnableGestureTranslation), true, false, BindingMode.TwoWay);

        static ZoomBorder()
        {
            AffectsArrange<ZoomBorder>(
                ZoomSpeedProperty,
                StretchProperty,
                EnableConstrainsProperty,
                MinZoomXProperty,
                MaxZoomXProperty,
                MinZoomYProperty,
                MaxZoomYProperty,
                MinOffsetXProperty,
                MaxOffsetXProperty,
                MinOffsetYProperty,
                MaxOffsetYProperty);
        }

        internal static void Log(string message) => Debug.WriteLine(message);

        private IControl? _element;
        private Point _pan;
        private Point _previous;
        private Matrix _matrix;
        private bool _isPanning;
        private double _zoomX = 1.0;
        private double _zoomY = 1.0;
        private double _offsetX = 0.0;
        private double _offsetY = 0.0;
        private bool _captured = false;

        /// <summary>
        /// Zoom changed event.
        /// </summary>
        public event ZoomChangedEventHandler? ZoomChanged;
        
        /// <summary>
        /// Gets or sets invalidate action for border child element.
        /// </summary>
        /// <remarks>
        /// First parameter is zoom ratio for x axis.
        /// Second parameter is zoom ratio for y axis.
        /// Third parameter is pan offset for x axis.
        /// Fourth parameter is pan offset for y axis.
        /// </remarks>
        public Action<double, double, double, double>? InvalidatedChild { get; set; }

        /// <summary>
        /// Gets or sets pan input button.
        /// </summary>
        public ButtonName PanButton
        {
            get => GetValue(PanButtonProperty);
            set => SetValue(PanButtonProperty, value);
        }

        /// <summary>
        /// Gets or sets zoom speed ratio.
        /// </summary>
        public double ZoomSpeed
        {
            get => GetValue(ZoomSpeedProperty);
            set => SetValue(ZoomSpeedProperty, value);
        }

        /// <summary>
        /// Gets or sets stretch mode.
        /// </summary>
        public StretchMode Stretch
        {
            get => GetValue(StretchProperty);
            set => SetValue(StretchProperty, value);
        }

        /// <summary>
        /// Gets the render transform matrix.
        /// </summary>
        public Matrix Matrix => _matrix;

        /// <summary>
        /// Gets the zoom ratio for x axis.
        /// </summary>
        public double ZoomX => _zoomX;

        /// <summary>
        /// Gets the zoom ratio for y axis.
        /// </summary>
        public double ZoomY => _zoomY;

        /// <summary>
        /// Gets the pan offset for x axis.
        /// </summary>
        public double OffsetX => _offsetX;

        /// <summary>
        /// Gets the pan offset for y axis.
        /// </summary>
        public double OffsetY => _offsetY;

        /// <summary>
        /// Gets or sets flag indicating whether zoom ratio and pan offset constrains are applied.
        /// </summary>
        public bool EnableConstrains
        {
            get => GetValue(EnableConstrainsProperty);
            set => SetValue(EnableConstrainsProperty, value);
        }

        /// <summary>
        /// Gets or sets minimum zoom ratio for x axis.
        /// </summary>
        public double MinZoomX
        {
            get => GetValue(MinZoomXProperty);
            set => SetValue(MinZoomXProperty, value);
        }

        /// <summary>
        /// Gets or sets maximum zoom ratio for x axis.
        /// </summary>
        public double MaxZoomX
        {
            get => GetValue(MaxZoomXProperty);
            set => SetValue(MaxZoomXProperty, value);
        }

        /// <summary>
        /// Gets or sets minimum zoom ratio for y axis.
        /// </summary>
        public double MinZoomY
        {
            get => GetValue(MinZoomYProperty);
            set => SetValue(MinZoomYProperty, value);
        }

        /// <summary>
        /// Gets or sets maximum zoom ratio for y axis.
        /// </summary>
        public double MaxZoomY
        {
            get => GetValue(MaxZoomYProperty);
            set => SetValue(MaxZoomYProperty, value);
        }

        /// <summary>
        /// Gets or sets minimum offset for x axis.
        /// </summary>
        public double MinOffsetX
        {
            get => GetValue(MinOffsetXProperty);
            set => SetValue(MinOffsetXProperty, value);
        }

        /// <summary>
        /// Gets or sets maximum offset for x axis.
        /// </summary>
        public double MaxOffsetX
        {
            get => GetValue(MaxOffsetXProperty);
            set => SetValue(MaxOffsetXProperty, value);
        }

        /// <summary>
        /// Gets or sets minimum offset for y axis.
        /// </summary>
        public double MinOffsetY
        {
            get => GetValue(MinOffsetYProperty);
            set => SetValue(MinOffsetYProperty, value);
        }

        /// <summary>
        /// Gets or sets maximum offset for y axis.
        /// </summary>
        public double MaxOffsetY
        {
            get => GetValue(MaxOffsetYProperty);
            set => SetValue(MaxOffsetYProperty, value);
        }

        /// <summary>
        /// Gets or sets flag indicating whether pan input events are processed.
        /// </summary>
        public bool EnablePan
        {
            get => GetValue(EnablePanProperty);
            set => SetValue(EnablePanProperty, value);
        }

        /// <summary>
        /// Gets or sets flag indicating whether input zoom events are processed.
        /// </summary>
        public bool EnableZoom
        {
            get => GetValue(EnableZoomProperty);
            set => SetValue(EnableZoomProperty, value);
        }

        /// <summary>
        /// Gets or sets flag indicating whether zoom gesture is enabled.
        /// </summary>
        public bool EnableGestureZoom
        {
            get => GetValue(EnableGestureZoomProperty);
            set => SetValue(EnableGestureZoomProperty, value);
        }

        /// <summary>
        /// Gets or sets flag indicating whether rotation gesture is enabled.
        /// </summary>
        public bool EnableGestureRotation
        {
            get => GetValue(EnableGestureRotationProperty);
            set => SetValue(EnableGestureRotationProperty, value);
        }

        /// <summary>
        /// Gets or sets flag indicating whether translation (pan) gesture is enabled.
        /// </summary>
        public bool EnableGestureTranslation
        {
            get => GetValue(EnableGestureTranslationProperty);
            set => SetValue(EnableGestureTranslationProperty, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoomBorder"/> class.
        /// </summary>
        public ZoomBorder()
            : base()
        {
            Defaults();
            Focusable = true;
            Background = Brushes.Transparent;
            AttachedToVisualTree += PanAndZoom_AttachedToVisualTree;
            DetachedFromVisualTree += PanAndZoom_DetachedFromVisualTree;
            AddHandler(ScrollViewer.ScrollChangedEvent, OnScrollChanged);
            this.GetObservable(ChildProperty).Subscribe(ChildChanged);
        }

        private void RaiseZoomChanged()
        {
            var args = new ZoomChangedEventArgs()
            {
                ZoomX = _zoomX,
                ZoomY =  _zoomY,
                OffsetX = _offsetX,
                OffsetY =  _offsetY
            };
            OnZoomChanged(args);
        }

        private void Defaults()
        {
            _isPanning = false;
            _matrix = Matrix.Identity;
            _captured = false;
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

            AutoFit(
                size.Width,
                size.Height,
                _element.Bounds.Width,
                _element.Bounds.Height);

            return size;
        }

        private void PanAndZoom_AttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            Log($"[AttachedToVisualTree] {Name}");
            ChildChanged(Child);
        }

        private void PanAndZoom_DetachedFromVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            Log($"[DetachedFromVisualTree] {Name}");
            DetachElement();
        }

        private void Border_PointerWheelChanged(object sender, PointerWheelEventArgs e)
        {
            if (!EnableZoom)
            {
                return;
            }
            Wheel(e);
        }

        private void Border_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            Pressed(e);
        }

        private void Border_PointerReleased(object sender, PointerReleasedEventArgs e)
        {
            Released(e);
        }

        private void Border_PointerMoved(object sender, PointerEventArgs e)
        {
            Moved(e);
        }

        private void ChildChanged(IControl? element)
        {
            if (element != null && element != _element && _element != null)
            {
                DetachElement();
            }

            if (element != null && element != _element)
            {
                AttachElement(element);
            }
        }

        private void AttachElement(IControl? element)
        {
            if (element == null)
            {
                return;
            }
            _element = element;
            PointerWheelChanged += Border_PointerWheelChanged;
            PointerPressed += Border_PointerPressed;
            PointerReleased += Border_PointerReleased;
            PointerMoved += Border_PointerMoved;
        }

        private void DetachElement()
        {
            if (_element == null)
            {
                return;
            }
            PointerWheelChanged -= Border_PointerWheelChanged;
            PointerPressed -= Border_PointerPressed;
            PointerReleased -= Border_PointerReleased;
            PointerMoved -= Border_PointerMoved;
            _element.RenderTransform = null;
            _element = null;
        }

        private void Wheel(PointerWheelEventArgs e)
        {
            if (_element == null || _captured != false)
            {
                return;
            }
            var point = e.GetPosition(_element);
            ZoomDeltaTo(e.Delta.Y, point.X, point.Y);
        }

        private void Pressed(PointerPressedEventArgs e)
        {
            if (!EnablePan)
            {
                return;
            }
            var button = PanButton;
            var properties = e.GetCurrentPoint(this).Properties;
            if ((!properties.IsLeftButtonPressed || button != ButtonName.Left)
                && (!properties.IsRightButtonPressed || button != ButtonName.Right)
                && (!properties.IsMiddleButtonPressed || button != ButtonName.Middle))
            {
                return;
            }
            if (_element != null && _captured == false && _isPanning == false)
            {
                var point = e.GetPosition(_element);
                StartPan(point.X, point.Y);
                _captured = true;
                _isPanning = true;
            }
        }

        private void Released(PointerReleasedEventArgs e)
        {
            if (!EnablePan)
            {
                return;
            }
            if (_element == null || _captured != true || _isPanning != true)
            {
                return;
            }
            _captured = false;
            _isPanning = false;
        }

        private void Moved(PointerEventArgs e)
        {
            if (!EnablePan)
            {
                return;
            }
            if (_element == null || _captured != true || _isPanning != true)
            {
                return;
            }
            var point = e.GetPosition(_element);
            PanTo(point.X, point.Y);
        }

        private double Constrain(double value, double minimum, double maximum)
        {
            if (minimum > maximum)
                throw new ArgumentException($"Parameter {nameof(minimum)} is greater than {nameof(maximum)}.");

            if (maximum < minimum)
                throw new ArgumentException($"Parameter {nameof(maximum)} is lower than {nameof(minimum)}.");

            return Min(Max(value, minimum), maximum);
        }

        private void Constrain()
        {
            var zoomX = Constrain(_matrix.M11, MinZoomX, MaxZoomX);
            var zoomY = Constrain(_matrix.M22, MinZoomY, MaxZoomY);
            var offsetX = Constrain(_matrix.M31, MinOffsetX, MaxOffsetX);
            var offsetY = Constrain(_matrix.M32, MinOffsetY, MaxOffsetY);
            _matrix = new Matrix(zoomX, 0.0, 0.0, zoomY, offsetX, offsetY);
        }

        /// <summary>
        /// Raises <see cref="ZoomChanged"/> event.
        /// </summary>
        /// <param name="e">Zoom changed event arguments.</param>
        protected virtual void OnZoomChanged(ZoomChangedEventArgs e)
        {
            ZoomChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Invalidate child element.
        /// </summary>
        public void Invalidate()
        {
            if (_element == null)
            {
                return;
            }
            
            if (EnableConstrains == true)
            {
                Constrain();
            }
            
            var oldZoomX = _zoomX;
            var oldZoomY = _zoomY;
            var oldOffsetX = _offsetX;
            var oldOffsetY = _offsetY;
            
            _zoomX = _matrix.M11;
            _zoomY = _matrix.M22;
            _offsetX = _matrix.M31;
            _offsetY = _matrix.M32;
            
            RaisePropertyChanged(ZoomXProperty, oldZoomX, _zoomX);
            RaisePropertyChanged(ZoomYProperty, oldZoomY, _zoomY);
            RaisePropertyChanged(OffsetXProperty, oldOffsetX, _offsetX);
            RaisePropertyChanged(OffsetYProperty, oldOffsetY, _offsetY);
            
            InvalidatedChild?.Invoke(_matrix.M11, _matrix.M22, _matrix.M31, _matrix.M32);
            
            _element.RenderTransformOrigin = new RelativePoint(new Point(0, 0), RelativeUnit.Relative);
            _element.RenderTransform = new MatrixTransform(_matrix);

            _element.InvalidateVisual();

            UpdateScrollable();

            RaiseZoomChanged();
        }

        /// <summary>
        /// Zoom to provided zoom ratio and provided center point.
        /// </summary>
        /// <param name="zoom">The zoom ratio.</param>
        /// <param name="x">The center point x axis coordinate.</param>
        /// <param name="y">The center point y axis coordinate.</param>
        public void ZoomTo(double zoom, double x, double y)
        {
            _matrix = MatrixHelper.ScaleAtPrepend(_matrix, zoom, zoom, x, y);
            Invalidate();
        }

        /// <summary>
        /// Zoom in one step positive delta ratio and panel center point.
        /// </summary>
        public void ZoomIn()
        {
            if (_element == null)
            {
                return;
            }
            var x = _element.Bounds.Width / 2.0;
            var y = _element.Bounds.Height / 2.0;
            ZoomTo(ZoomSpeed, x, y);
        }

        /// <summary>
        /// Zoom out one step positive delta ratio and panel center point.
        /// </summary>
        public void ZoomOut()
        {
            if (_element == null)
            {
                return;
            }
            var x = _element.Bounds.Width / 2.0;
            var y = _element.Bounds.Height / 2.0;
            ZoomTo(1 / ZoomSpeed, x, y);
        }

        /// <summary>
        /// Zoom to provided zoom delta ratio and provided center point.
        /// </summary>
        /// <param name="delta">The zoom delta ratio.</param>
        /// <param name="x">The center point x axis coordinate.</param>
        /// <param name="y">The center point y axis coordinate.</param>
        public void ZoomDeltaTo(double delta, double x, double y)
        {
            ZoomTo(delta > 0 ? ZoomSpeed : 1 / ZoomSpeed, x, y);
        }

        /// <summary>
        /// Set pan origin.
        /// </summary>
        /// <param name="x">The origin point x axis coordinate.</param>
        /// <param name="y">The origin point y axis coordinate.</param>
        public void StartPan(double x, double y)
        {
            _pan = new Point();
            _previous = new Point(x, y);
        }

        /// <summary>
        /// Pan control to provided target point.
        /// </summary>
        /// <param name="x">The target point x axis coordinate.</param>
        /// <param name="y">The target point y axis coordinate.</param>
        public void PanTo(double x, double y)
        {
            var dx = x - _previous.X;
            var dy = y - _previous.Y;
            var delta = new Point(dx, dy);
            _previous = new Point(x, y);
            _pan = new Point(_pan.X + delta.X, _pan.Y + delta.Y);
            _matrix = MatrixHelper.TranslatePrepend(_matrix, _pan.X, _pan.Y);
            Invalidate();
        }

        private Matrix GetMatrix(double panelWidth, double panelHeight, double elementWidth, double elementHeight, StretchMode mode)
        {
            var zx = panelWidth / elementWidth;
            var zy = panelHeight / elementHeight;
            var cx = elementWidth / 2.0;
            var cy = elementHeight / 2.0;
            switch (mode)
            {
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
                case StretchMode.None:
                    break;
            }
            return Matrix.Identity;
        }

        /// <summary>
        /// Zoom and pan.
        /// </summary>
        /// <param name="panelWidth">The panel width.</param>
        /// <param name="panelHeight">The panel height.</param>
        /// <param name="elementWidth">The element width.</param>
        /// <param name="elementHeight">The element height.</param>
        public void None(double panelWidth, double panelHeight, double elementWidth, double elementHeight)
        {
            Log($"[None] {panelWidth}x{panelHeight} {elementWidth}x{elementHeight}");
            if (_element == null)
            {
                return;
            }
            _matrix = GetMatrix(panelWidth, panelHeight, elementWidth, elementHeight, StretchMode.None);
            Invalidate();
        }

        /// <summary>
        /// Zoom and pan to fill panel.
        /// </summary>
        /// <param name="panelWidth">The panel width.</param>
        /// <param name="panelHeight">The panel height.</param>
        /// <param name="elementWidth">The element width.</param>
        /// <param name="elementHeight">The element height.</param>
        public void Fill(double panelWidth, double panelHeight, double elementWidth, double elementHeight)
        {
            Log($"[Fill] {panelWidth}x{panelHeight} {elementWidth}x{elementHeight}");
            if (_element == null)
            {
                return;
            }
            _matrix = GetMatrix(panelWidth, panelHeight, elementWidth, elementHeight, StretchMode.Fill);
            Invalidate();
        }

        /// <summary>
        /// Zoom and pan to panel extents while maintaining aspect ratio.
        /// </summary>
        /// <param name="panelWidth">The panel width.</param>
        /// <param name="panelHeight">The panel height.</param>
        /// <param name="elementWidth">The element width.</param>
        /// <param name="elementHeight">The element height.</param>
        public void Uniform(double panelWidth, double panelHeight, double elementWidth, double elementHeight)
        {
            Log($"[Uniform] {panelWidth}x{panelHeight} {elementWidth}x{elementHeight}");
            if (_element == null)
            {
                return;
            }
            _matrix = GetMatrix(panelWidth, panelHeight, elementWidth, elementHeight, StretchMode.Uniform);
            Invalidate();
        }

        /// <summary>
        /// Zoom and pan to panel extents while maintaining aspect ratio. If aspect of panel is different panel is filled.
        /// </summary>
        /// <param name="panelWidth">The panel width.</param>
        /// <param name="panelHeight">The panel height.</param>
        /// <param name="elementWidth">The element width.</param>
        /// <param name="elementHeight">The element height.</param>
        public void UniformToFill(double panelWidth, double panelHeight, double elementWidth, double elementHeight)
        {
            Log($"[UniformToFill] {panelWidth}x{panelHeight} {elementWidth}x{elementHeight}");
            if (_element == null)
            {
                return;
            }
            _matrix = GetMatrix(panelWidth, panelHeight, elementWidth, elementHeight, StretchMode.UniformToFill);
            Invalidate();
        }

        /// <summary>
        /// Zoom and pan child element inside panel using stretch mode.
        /// </summary>
        /// <param name="panelWidth">The panel width.</param>
        /// <param name="panelHeight">The panel height.</param>
        /// <param name="elementWidth">The element width.</param>
        /// <param name="elementHeight">The element height.</param>
        public void AutoFit(double panelWidth, double panelHeight, double elementWidth, double elementHeight)
        {
            Log($"[AutoFit] {panelWidth}x{panelHeight} {elementWidth}x{elementHeight}");
            if (_element == null)
            {
                return;
            }
            switch (Stretch)
            {
                case StretchMode.Fill:
                    Fill(panelWidth, panelHeight, elementWidth, elementHeight);
                    break;
                case StretchMode.Uniform:
                    Uniform(panelWidth, panelHeight, elementWidth, elementHeight);
                    break;
                case StretchMode.UniformToFill:
                    UniformToFill(panelWidth, panelHeight, elementWidth, elementHeight);
                    break;
                case StretchMode.None:
                    break;
            }
            Invalidate();
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
        /// Reset pan and zoom matrix.
        /// </summary>
        public void Reset()
        {
            _matrix = Matrix.Identity;
            Invalidate();
        }

        /// <summary>
        /// Zoom and pan.
        /// </summary>
        public void None()
        {
            if (_element == null)
            {
                return;
            }
            None(Bounds.Width, Bounds.Height, _element.Bounds.Width, _element.Bounds.Height);
        }
        
        /// <summary>
        /// Zoom and pan to fill panel.
        /// </summary>
        public void Fill()
        {
            if (_element == null)
            {
                return;
            }
            Fill(Bounds.Width, Bounds.Height, _element.Bounds.Width, _element.Bounds.Height);
        }

        /// <summary>
        /// Zoom and pan to panel extents while maintaining aspect ratio.
        /// </summary>
        public void Uniform()
        {
            if (_element == null)
            {
                return;
            }
            Uniform(Bounds.Width, Bounds.Height, _element.Bounds.Width, _element.Bounds.Height);
        }

        /// <summary>
        /// Zoom and pan to panel extents while maintaining aspect ratio. If aspect of panel is different panel is filled.
        /// </summary>
        public void UniformToFill()
        {
            if (_element == null)
            {
                return;
            }
            UniformToFill(Bounds.Width, Bounds.Height, _element.Bounds.Width, _element.Bounds.Height);
        }

        /// <summary>
        /// Zoom and pan child element inside panel using stretch mode.
        /// </summary>
        public void AutoFit()
        {
            if (_element == null)
            {
                return;
            }
            AutoFit(Bounds.Width, Bounds.Height, _element.Bounds.Width, _element.Bounds.Height);
        }

        private Size _extent = new Size();
        private Size _viewport = new Size();
        private Vector _offset = new Vector();
        private bool _canHorizontallyScroll = false;
        private bool _canVerticallyScroll = false;
        private EventHandler? _scrollInvalidated;

        /// <inheritdoc/>
        Size IScrollable.Extent => _extent;

        /// <inheritdoc/>
        Vector IScrollable.Offset
        {
            get => _offset;
            set => _offset = value;
        }

        /// <inheritdoc/>
        Size IScrollable.Viewport => _viewport;

        bool ILogicalScrollable.CanHorizontallyScroll
        {
            get => _canHorizontallyScroll;
            set
            {
                _canHorizontallyScroll = value;
                InvalidateMeasure();
            }
        }

        bool ILogicalScrollable.CanVerticallyScroll
        {
            get => _canVerticallyScroll;
            set
            {
                _canVerticallyScroll = value;
                InvalidateMeasure();
            }
        }

        bool ILogicalScrollable.IsLogicalScrollEnabled => true;

        event EventHandler ILogicalScrollable.ScrollInvalidated
        {
            add => _scrollInvalidated += value;
            remove => _scrollInvalidated -= value;
        }

        Size ILogicalScrollable.ScrollSize => new Size(1, 1);

        Size ILogicalScrollable.PageScrollSize => new Size(1, 1);

        bool ILogicalScrollable.BringIntoView(IControl target, Rect targetRect)
        {
            return false;
        }

        IControl? ILogicalScrollable.GetControlInDirection(NavigationDirection direction, IControl from)
        {
            return null;
        }

        void ILogicalScrollable.RaiseScrollInvalidated(EventArgs e)
        {
            _scrollInvalidated?.Invoke(this, e);
        }

        private void UpdateScrollable()
        {
            if (!(this is ILogicalScrollable scrollable))
            {
                return;
            }

            if (_element == null)
            {
                return;
            }
            
            ZoomHelper.CalculateScrollable(_element.Bounds, _matrix, out var extent, out var viewport, out var  offset);

            _extent = extent;
            _offset = offset;
            _viewport = viewport;

            scrollable.RaiseScrollInvalidated(EventArgs.Empty);
        }

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            Log($"[OnScrollChanged] ExtentDelta: {e.ExtentDelta}, OffsetDelta: {e.OffsetDelta}, ViewportDelta: {e.ViewportDelta}");
        }
    }
}
