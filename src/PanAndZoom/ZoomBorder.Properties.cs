// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Data;
using Avalonia.Media.Transformation;

namespace Avalonia.Controls.PanAndZoom;

/// <summary>
/// Pan and zoom control for Avalonia.
/// </summary>
public partial class ZoomBorder
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
    public static readonly StyledProperty<ButtonName> PanButtonProperty =
        AvaloniaProperty.Register<ZoomBorder, ButtonName>(nameof(PanButton), ButtonName.Middle, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="ZoomSpeed"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> ZoomSpeedProperty =
        AvaloniaProperty.Register<ZoomBorder, double>(nameof(ZoomSpeed), 1.2, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="PowerFactor"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> PowerFactorProperty =
        AvaloniaProperty.Register<ZoomBorder, double>(nameof(PowerFactor), 1, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="TransitionThreshold"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> TransitionThresholdProperty =
        AvaloniaProperty.Register<ZoomBorder, double>(nameof(TransitionThreshold), 0.5, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="Stretch"/> avalonia property.
    /// </summary>
    public static readonly  StyledProperty<StretchMode> StretchProperty =
        AvaloniaProperty.Register<ZoomBorder, StretchMode>(nameof(Stretch), StretchMode.Uniform, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="ZoomX"/> avalonia property.
    /// </summary>
    public static readonly DirectProperty<ZoomBorder, double> ZoomXProperty =
        AvaloniaProperty.RegisterDirect<ZoomBorder, double>(nameof(ZoomX), o => o.ZoomX, null, 1.0);

    /// <summary>
    /// Identifies the <seealso cref="ZoomY"/> avalonia property.
    /// </summary>
    public static readonly DirectProperty<ZoomBorder, double> ZoomYProperty =
        AvaloniaProperty.RegisterDirect<ZoomBorder, double>(nameof(ZoomY), o => o.ZoomY, null, 1.0);

    /// <summary>
    /// Identifies the <seealso cref="OffsetX"/> avalonia property.
    /// </summary>
    public static readonly DirectProperty<ZoomBorder, double> OffsetXProperty =
        AvaloniaProperty.RegisterDirect<ZoomBorder, double>(nameof(OffsetX), o => o.OffsetX, null, 0.0);

    /// <summary>
    /// Identifies the <seealso cref="OffsetY"/> avalonia property.
    /// </summary>
    public static readonly DirectProperty<ZoomBorder, double> OffsetYProperty =
        AvaloniaProperty.RegisterDirect<ZoomBorder, double>(nameof(OffsetY), o => o.OffsetY, null, 0.0);

    /// <summary>
    /// Identifies the <seealso cref="EnableConstrains"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> EnableConstrainsProperty =
        AvaloniaProperty.Register<ZoomBorder, bool>(nameof(EnableConstrains), true, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="MinZoomX"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> MinZoomXProperty =
        AvaloniaProperty.Register<ZoomBorder, double>(nameof(MinZoomX), double.NegativeInfinity, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="MaxZoomX"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> MaxZoomXProperty =
        AvaloniaProperty.Register<ZoomBorder, double>(nameof(MaxZoomX), double.PositiveInfinity, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="MinZoomY"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> MinZoomYProperty =
        AvaloniaProperty.Register<ZoomBorder, double>(nameof(MinZoomY), double.NegativeInfinity, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="MaxZoomY"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> MaxZoomYProperty =
        AvaloniaProperty.Register<ZoomBorder, double>(nameof(MaxZoomY), double.PositiveInfinity, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="MinOffsetX"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> MinOffsetXProperty =
        AvaloniaProperty.Register<ZoomBorder, double>(nameof(MinOffsetX), double.NegativeInfinity, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="MaxOffsetX"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> MaxOffsetXProperty =
        AvaloniaProperty.Register<ZoomBorder, double>(nameof(MaxOffsetX), double.PositiveInfinity, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="MinOffsetY"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> MinOffsetYProperty =
        AvaloniaProperty.Register<ZoomBorder, double>(nameof(MinOffsetY), double.NegativeInfinity, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="MaxOffsetY"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> MaxOffsetYProperty =
        AvaloniaProperty.Register<ZoomBorder, double>(nameof(MaxOffsetY), double.PositiveInfinity, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="EnablePan"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> EnablePanProperty =
        AvaloniaProperty.Register<ZoomBorder, bool>(nameof(EnablePan), true, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="EnableZoom"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> EnableZoomProperty =
        AvaloniaProperty.Register<ZoomBorder, bool>(nameof(EnableZoom), true, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="EnableGestureZoom"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> EnableGestureZoomProperty =
        AvaloniaProperty.Register<ZoomBorder, bool>(nameof(EnableGestureZoom), true, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="EnableGestureRotation"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> EnableGestureRotationProperty =
        AvaloniaProperty.Register<ZoomBorder, bool>(nameof(EnableGestureRotation), true, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="EnableGestureTranslation"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> EnableGestureTranslationProperty =
        AvaloniaProperty.Register<ZoomBorder, bool>(nameof(EnableGestureTranslation), true, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="EnableGestures"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> EnableGesturesProperty =
        AvaloniaProperty.Register<ZoomBorder, bool>(nameof(EnableGestures), true, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="AnimationDuration"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> AnimationDurationProperty =
        AvaloniaProperty.Register<ZoomBorder, TimeSpan>(nameof(AnimationDuration), TimeSpan.FromMilliseconds(300), false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="EnableAnimations"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> EnableAnimationsProperty =
        AvaloniaProperty.Register<ZoomBorder, bool>(nameof(EnableAnimations), false, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="EnableDoubleClickZoom"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> EnableDoubleClickZoomProperty =
        AvaloniaProperty.Register<ZoomBorder, bool>(nameof(EnableDoubleClickZoom), true, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="DoubleClickZoomMode"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<DoubleClickZoomMode> DoubleClickZoomModeProperty =
        AvaloniaProperty.Register<ZoomBorder, DoubleClickZoomMode>(nameof(DoubleClickZoomMode), DoubleClickZoomMode.ZoomInOut, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="DoubleClickZoomFactor"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> DoubleClickZoomFactorProperty =
        AvaloniaProperty.Register<ZoomBorder, double>(nameof(DoubleClickZoomFactor), 2.0, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="BoundsMode"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ContentBoundsMode> BoundsModeProperty =
        AvaloniaProperty.Register<ZoomBorder, ContentBoundsMode>(nameof(BoundsMode), ContentBoundsMode.Unrestricted, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="BoundsPadding"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Thickness> BoundsPaddingProperty =
        AvaloniaProperty.Register<ZoomBorder, Thickness>(nameof(BoundsPadding), new Thickness(0), false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="MinimumVisibleContentPercentage"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> MinimumVisibleContentPercentageProperty =
        AvaloniaProperty.Register<ZoomBorder, double>(nameof(MinimumVisibleContentPercentage), 0.1, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="ResizeBehavior"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ResizeBehaviorMode> ResizeBehaviorProperty =
        AvaloniaProperty.Register<ZoomBorder, ResizeBehaviorMode>(nameof(ResizeBehavior), ResizeBehaviorMode.None, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="WheelBehavior"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<WheelBehaviorMode> WheelBehaviorProperty =
        AvaloniaProperty.Register<ZoomBorder, WheelBehaviorMode>(nameof(WheelBehavior), WheelBehaviorMode.Zoom, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="WheelWithCtrl"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<WheelBehaviorMode> WheelWithCtrlProperty =
        AvaloniaProperty.Register<ZoomBorder, WheelBehaviorMode>(nameof(WheelWithCtrl), WheelBehaviorMode.Zoom, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="WheelWithShift"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<WheelBehaviorMode> WheelWithShiftProperty =
        AvaloniaProperty.Register<ZoomBorder, WheelBehaviorMode>(nameof(WheelWithShift), WheelBehaviorMode.PanHorizontal, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="WheelZoomSensitivity"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> WheelZoomSensitivityProperty =
        AvaloniaProperty.Register<ZoomBorder, double>(nameof(WheelZoomSensitivity), 1.0, false, BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="WheelPanSensitivity"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> WheelPanSensitivityProperty =
        AvaloniaProperty.Register<ZoomBorder, double>(nameof(WheelPanSensitivity), 1.0, false, BindingMode.TwoWay);

    static ZoomBorder()
    {
        AffectsArrange<ZoomBorder>(
            ZoomSpeedProperty,
            StretchProperty,
            EnableConstrainsProperty,
            EnablePanProperty,
            EnableZoomProperty,
            EnableGestureZoomProperty,
            EnableGestureRotationProperty,
            EnableGestureTranslationProperty,
            EnableGesturesProperty,
            MinZoomXProperty,
            MaxZoomXProperty,
            MinZoomYProperty,
            MaxZoomYProperty,
            MinOffsetXProperty,
            MaxOffsetXProperty,
            MinOffsetYProperty,
            MaxOffsetYProperty);
    }

    private Control? _element;
    private Point _pan;
    private Point _previous;
    private Matrix _matrix;
    private TransformOperations.Builder _transformBuilder;
    private bool _isPanning;
    private volatile bool _updating = false;
    private double _zoomX = 1.0;
    private double _zoomY = 1.0;
    private double _offsetX = 0.0;
    private double _offsetY = 0.0;
    private bool _captured = false;
    private Size _sizeBeforeResize;
    private double _doubleClickZoomThreshold = 1.5;

    /// <summary>
    /// Zoom changed event.
    /// </summary>
    public event ZoomChangedEventHandler? ZoomChanged;

    /// <summary>
    /// Pan started event.
    /// </summary>
    public event PanEventHandler? PanStarted;

    /// <summary>
    /// Pan continued event.
    /// </summary>
    public event PanEventHandler? PanContinued;

    /// <summary>
    /// Pan ended event.
    /// </summary>
    public event PanEventHandler? PanEnded;

    /// <summary>
    /// Zoom started event.
    /// </summary>
    public event ZoomEventHandler? ZoomStarted;

    /// <summary>
    /// Zoom ended event.
    /// </summary>
    public event ZoomEventHandler? ZoomEnded;

    /// <summary>
    /// Zoom delta changed event.
    /// </summary>
    public event ZoomEventHandler? ZoomDeltaChanged;

    /// <summary>
    /// Matrix changed event.
    /// </summary>
    public event MatrixChangedEventHandler? MatrixChanged;

    /// <summary>
    /// Matrix reset event.
    /// </summary>
    public event MatrixChangedEventHandler? MatrixReset;

    /// <summary>
    /// Stretch mode changed event.
    /// </summary>
    public event StretchModeChangedEventHandler? StretchModeChanged;

    /// <summary>
    /// Auto fit applied event.
    /// </summary>
    public event StretchModeChangedEventHandler? AutoFitApplied;

    /// <summary>
    /// Gesture started event.
    /// </summary>
    public event GestureEventHandler? GestureStarted;

    /// <summary>
    /// Gesture ended event.
    /// </summary>
    public event GestureEventHandler? GestureEnded;

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
    /// Gets or sets the power factor used to transform the mouse wheel delta value.
    /// </summary>
    public double PowerFactor
    {
        get => GetValue(PowerFactorProperty);
        set => SetValue(PowerFactorProperty, value);
    }

    /// <summary>
    /// Gets or sets the threshold below which zoom operations will skip all transitions.
    /// </summary>
    public double TransitionThreshold
    {
        get => GetValue(TransitionThresholdProperty);
        set => SetValue(TransitionThresholdProperty, value);
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
    /// Gets or sets flag indicating whether gestures are enabled.
    /// </summary>
    public bool EnableGestures
    {
        get => GetValue(EnableGesturesProperty);
        set => SetValue(EnableGesturesProperty, value);
    }

    /// <summary>
    /// Gets or sets the duration of animations for zoom and pan operations.
    /// </summary>
    public TimeSpan AnimationDuration
    {
        get => GetValue(AnimationDurationProperty);
        set => SetValue(AnimationDurationProperty, value);
    }

    /// <summary>
    /// Gets or sets flag indicating whether animations are enabled for zoom and pan operations.
    /// </summary>
    public bool EnableAnimations
    {
        get => GetValue(EnableAnimationsProperty);
        set => SetValue(EnableAnimationsProperty, value);
    }

    /// <summary>
    /// Gets or sets flag indicating whether double-click zoom is enabled.
    /// </summary>
    public bool EnableDoubleClickZoom
    {
        get => GetValue(EnableDoubleClickZoomProperty);
        set => SetValue(EnableDoubleClickZoomProperty, value);
    }

    /// <summary>
    /// Gets or sets the double-click zoom behavior mode.
    /// </summary>
    public DoubleClickZoomMode DoubleClickZoomMode
    {
        get => GetValue(DoubleClickZoomModeProperty);
        set => SetValue(DoubleClickZoomModeProperty, value);
    }

    /// <summary>
    /// Gets or sets the zoom factor for double-click zoom operations.
    /// </summary>
    public double DoubleClickZoomFactor
    {
        get => GetValue(DoubleClickZoomFactorProperty);
        set => SetValue(DoubleClickZoomFactorProperty, value);
    }

    /// <summary>
    /// Gets or sets the content bounds restriction mode.
    /// </summary>
    public ContentBoundsMode BoundsMode
    {
        get => GetValue(BoundsModeProperty);
        set => SetValue(BoundsModeProperty, value);
    }

    /// <summary>
    /// Gets or sets the padding around content bounds.
    /// </summary>
    public Thickness BoundsPadding
    {
        get => GetValue(BoundsPaddingProperty);
        set => SetValue(BoundsPaddingProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum percentage of content that must remain visible.
    /// </summary>
    public double MinimumVisibleContentPercentage
    {
        get => GetValue(MinimumVisibleContentPercentageProperty);
        set => SetValue(MinimumVisibleContentPercentageProperty, value);
    }

    /// <summary>
    /// Gets or sets the behavior when the control is resized.
    /// </summary>
    public ResizeBehaviorMode ResizeBehavior
    {
        get => GetValue(ResizeBehaviorProperty);
        set => SetValue(ResizeBehaviorProperty, value);
    }

    /// <summary>
    /// Gets or sets the default mouse wheel behavior (no modifiers).
    /// </summary>
    public WheelBehaviorMode WheelBehavior
    {
        get => GetValue(WheelBehaviorProperty);
        set => SetValue(WheelBehaviorProperty, value);
    }

    /// <summary>
    /// Gets or sets the mouse wheel behavior when Ctrl key is pressed.
    /// </summary>
    public WheelBehaviorMode WheelWithCtrl
    {
        get => GetValue(WheelWithCtrlProperty);
        set => SetValue(WheelWithCtrlProperty, value);
    }

    /// <summary>
    /// Gets or sets the mouse wheel behavior when Shift key is pressed.
    /// </summary>
    public WheelBehaviorMode WheelWithShift
    {
        get => GetValue(WheelWithShiftProperty);
        set => SetValue(WheelWithShiftProperty, value);
    }

    /// <summary>
    /// Gets or sets the zoom sensitivity for mouse wheel operations.
    /// </summary>
    public double WheelZoomSensitivity
    {
        get => GetValue(WheelZoomSensitivityProperty);
        set => SetValue(WheelZoomSensitivityProperty, value);
    }

    /// <summary>
    /// Gets or sets the pan sensitivity for mouse wheel operations.
    /// </summary>
    public double WheelPanSensitivity
    {
        get => GetValue(WheelPanSensitivityProperty);
        set => SetValue(WheelPanSensitivityProperty, value);
    }
}
