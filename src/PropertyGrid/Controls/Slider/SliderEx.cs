﻿using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using PropertyGrid.Controls.Helpers;
using ReactiveUI;

namespace PropertyGrid.Controls.Slider
{
    // This source file is adapted from the Windows Presentation Foundation project.
    // (https://github.com/dotnet/wpf/)

    /// <summary>
    /// slider with a tickbar
    /// 
    /// TODO AutoTooltip missing and some commented code.
    /// </summary>
    public class SliderEx : RangeBaseEx
    {
        private const string TrackName = "PART_Track";
        private const string SelectionRangeElementName = "PART_SelectionRange";

        internal Track Track { get; set; }

        internal TickBar.TickBar TopTickBar { get; private set; }
        internal TickBar.TickBar BottomTickBar { get; private set; }
        internal Border TrackBackground { get; private set; }
        internal AvaloniaObject SelectionRangeElement { get; set; }

        /// <summary>
        /// Get/Set Orientation property
        /// </summary>
        public Orientation Orientation
        {
            get { return GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly StyledProperty<Orientation> OrientationProperty =
            AvaloniaProperty.Register<SliderEx, Orientation>(nameof(Orientation), defaultValue: Orientation.Horizontal);

        //ScrollBar.OrientationProperty.AddOwner<SliderEx>();

        /// <summary>
        /// Get/Set IsDirectionReversed property
        /// </summary>
        public bool IsDirectionReversed
        {
            get { return GetValue(IsDirectionReversedProperty); }
            set { SetValue(IsDirectionReversedProperty, value); }
        }

        public static readonly StyledProperty<bool> IsDirectionReversedProperty =
            AvaloniaProperty.Register<SliderEx, bool>(nameof(IsDirectionReversed));

        /// <summary>
        /// Specifies the amount of time, in milliseconds, to wait before repeating begins.
        /// Must be non-negative.
        /// </summary>
        public int Delay
        {
            get { return GetValue(DelayProperty); }
            set { SetValue(DelayProperty, value); }
        }

        public static readonly StyledProperty<int> DelayProperty =
            RepeatButton.DelayProperty.AddOwner<SliderEx>();

        //AvaloniaProperty.Register<SliderEx, int>(nameof(Delay));

        /// <summary>
        /// Specifies the amount of time, in milliseconds, between repeats once repeating starts.
        /// Must be non-negative
        /// </summary>
        public int Interval
        {
            get { return GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        public static readonly StyledProperty<int> IntervalProperty =
            RepeatButton.IntervalProperty.AddOwner<SliderEx>();

        //AvaloniaProperty.Register<SliderEx, int>(nameof(Interval));

        /// <summary>
        ///     AutoToolTipPlacement property specifies the placement of the AutoToolTip
        /// </summary>
        public AutoToolTipPlacement AutoToolTipPlacement
        {
            get { return GetValue(AutoToolTipPlacementProperty); }
            set { SetValue(AutoToolTipPlacementProperty, value); }
        }

        public static readonly StyledProperty<AutoToolTipPlacement> AutoToolTipPlacementProperty =
            AvaloniaProperty.Register<SliderEx, AutoToolTipPlacement>(nameof(AutoToolTipPlacement)
                , defaultValue: AutoToolTipPlacement.None
            );

        /// <summary>
        ///     Get or set number of decimal digits of Slider's Value shown in AutoToolTip
        /// </summary>
        public int AutoToolTipPrecision
        {
            get { return GetValue(AutoToolTipPrecisionProperty); }
            set { SetValue(AutoToolTipPrecisionProperty, value); }
        }

        public static readonly StyledProperty<int> AutoToolTipPrecisionProperty =
            AvaloniaProperty.Register<SliderEx, int>(nameof(AutoToolTipPrecision), defaultValue: 0);

        /// <summary>
        ///     When 'true', Slider will automatically move the Thumb (and/or change current value) to the closest TickMark.
        /// </summary>
        public bool IsSnapToTickEnabled
        {
            get { return GetValue(IsSnapToTickEnabledProperty); }
            set { SetValue(IsSnapToTickEnabledProperty, value); }
        }

        public static readonly StyledProperty<bool> IsSnapToTickEnabledProperty =
            AvaloniaProperty.Register<SliderEx, bool>(nameof(IsSnapToTickEnabled), defaultValue: true);

        /// <summary>
        ///     Slider uses this value to determine where to show the Ticks.
        /// When Ticks is not 'null', Slider will ignore 'TickFrequency', and draw only TickMarks
        /// that specified in Ticks collection.
        /// </summary>
        public TickPlacement TickPlacement
        {
            get { return GetValue(TickPlacementProperty); }
            set { SetValue(TickPlacementProperty, value); }
        }

        public static readonly StyledProperty<TickPlacement> TickPlacementProperty =
            AvaloniaProperty.Register<SliderEx, TickPlacement>(nameof(TickPlacement)
                , defaultValue: TickPlacement.None);

        /// <summary>
        ///     Slider uses this value to determine where to show the Ticks.
        /// When Ticks is not 'null', Slider will ignore 'TickFrequency', and draw only TickMarks
        /// that specified in Ticks collection.
        /// </summary>
        public double TickFrequency
        {
            get { return GetValue(TickFrequencyProperty); }
            set { SetValue(TickFrequencyProperty, value); }
        }

        public static readonly StyledProperty<double> TickFrequencyProperty =
            AvaloniaProperty.Register<SliderEx, double>(nameof(TickFrequency), defaultValue: 1.0);

        /// <summary>
        ///     Slider uses this value to determine where to show the Ticks.
        /// When Ticks is not 'null', Slider will ignore 'TickFrequency', and draw only TickMarks
        /// that specified in Ticks collection.
        /// </summary>
        public DoubleCollection Ticks
        {
            get { return GetValue(TicksProperty); }
            set { SetValue(TicksProperty, value); }
        }

        public static readonly StyledProperty<DoubleCollection> TicksProperty =
            AvaloniaProperty.Register<SliderEx, DoubleCollection>(nameof(Ticks),
                defaultValue: DoubleCollection.Empty());

        /// <summary>
        ///     Enable or disable selection support on Slider
        /// </summary>
        public bool IsSelectionRangeEnabled
        {
            get { return GetValue(IsSelectionRangeEnabledProperty); }
            set { SetValue(IsSelectionRangeEnabledProperty, value); }
        }

        public static readonly StyledProperty<bool> IsSelectionRangeEnabledProperty =
            AvaloniaProperty.Register<SliderEx, bool>(nameof(IsSelectionRangeEnabled));

        /// <summary>
        ///     Get or set starting value of selection.
        /// </summary>
        public double SelectionStart
        {
            get { return GetValue(SelectionStartProperty); }
            set { SetValue(SelectionStartProperty, value); }
        }

        public static readonly StyledProperty<double> SelectionStartProperty =
            AvaloniaProperty.Register<SliderEx, double>(nameof(SelectionStart)
                , defaultValue: 0.0d, defaultBindingMode: global::Avalonia.Data.BindingMode.TwoWay);

        /// <summary>
        ///     Get or set dending value of selection.
        /// </summary>
        public double SelectionEnd
        {
            get { return GetValue(SelectionEndProperty); }
            set { SetValue(SelectionEndProperty, value); }
        }

        public static readonly StyledProperty<double> SelectionEndProperty =
            AvaloniaProperty.Register<SliderEx, double>(nameof(SelectionEnd)
                , defaultValue: 0.0d, defaultBindingMode: global::Avalonia.Data.BindingMode.TwoWay);

        /// <summary>
        ///     Enable or disable Move-To-Point support on Slider.
        ///     Move-To-Point feature, enables Slider to immediately move the Thumb directly to the location where user
        /// clicked the Mouse.
        /// </summary>
        public bool IsMoveToPointEnabled
        {
            get { return GetValue(IsMoveToPointEnabledProperty); }
            set { SetValue(IsMoveToPointEnabledProperty, value); }
        }

        public static readonly StyledProperty<bool> IsMoveToPointEnabledProperty =
            AvaloniaProperty.Register<SliderEx, bool>(nameof(IsMoveToPointEnabled));

        /// <summary>
        /// initilize the command 
        /// and pseudoclass
        /// </summary>
        public SliderEx()
        {
            InitializeCommands();

            UpdatePseudoClassesForOrientation(Orientation);
        }

        /// This is the static constructor for the Slider class.  It
        /// simply registers the appropriate class handlers for the input
        /// devices, and defines a default style sheet.
        static SliderEx()
        {
            Thumb.DragStartedEvent.AddClassHandler<SliderEx>((o, e) => OnThumbDragStarted(o, e),
                RoutingStrategies.Bubble);
            Thumb.DragDeltaEvent.AddClassHandler<SliderEx>((o, e) => OnThumbDragDelta(o, e), RoutingStrategies.Bubble);
            Thumb.DragCompletedEvent.AddClassHandler<SliderEx>((o, e) => OnThumbDragCompleted(o, e),
                RoutingStrategies.Bubble);

            SelectionStartProperty.Changed.AddClassHandler<SliderEx>(
                (o, e) => OnSelectionStartChanged(o, e));
            SelectionEndProperty.Changed.AddClassHandler<SliderEx>(
                (o, e) => OnSelectionEndChanged(o, e));
            ValueProperty.Changed.AddClassHandler<SliderEx>(
                (o, e) => OnValueChanged(o, e));
            TickPlacementProperty.Changed.AddClassHandler<SliderEx>(
                (o, e) => OnTickPlacementChanged(o, e));
        }


        private static void OnTickPlacementChanged(SliderEx slider, AvaloniaPropertyChangedEventArgs e)
        {
            if (slider.TopTickBar == null || slider.BottomTickBar == null)
                return;

            //strange behaviour in the view
            //right now we skip this
            if (slider.Orientation == Orientation.Vertical)
                return;

            //TopTickBar.Width = this.Width;
            //BottomTickBar.Width = this.Width;

            if (slider.Track?.Thumb != null)
            {
                switch (slider.Orientation)
                {
                    case Orientation.Horizontal:
                        if (DoubleUtil.IsDoubleFinite(slider.Track.Thumb.Width))
                            slider.TopTickBar.ReservedSpace = slider.Track.Thumb.Width;
                        break;

                    case Orientation.Vertical:
                        if (DoubleUtil.IsDoubleFinite(slider.Track.Thumb.Height))
                            slider.TopTickBar.ReservedSpace = slider.Track.Thumb.Height;
                        break;
                }
            }

            if (slider.Orientation == Orientation.Horizontal)
            {
                slider.Track.Thumb.Classes.Set("HorizontalSliderUpThumbStyle",
                    slider.TickPlacement == TickPlacement.TopLeft);
                slider.Track.Thumb.Classes.Set("HorizontalSliderDownThumbStyle",
                    slider.TickPlacement == TickPlacement.BottomRight);
            }
            else
            {
                slider.Track.Thumb.Classes.Set("VerticalSliderLeftThumbStyle",
                    slider.TickPlacement == TickPlacement.TopLeft);
                slider.Track.Thumb.Classes.Set("VerticalSliderRightThumbStyle",
                    slider.TickPlacement == TickPlacement.BottomRight);
            }

            TickPlacement tickBarPlacement = (TickPlacement)e.NewValue;
            switch (tickBarPlacement)
            {
                case TickPlacement.None:
                    slider.TopTickBar.IsVisible = false;
                    slider.BottomTickBar.IsVisible = false;
                    break;

                case TickPlacement.TopLeft:
                    slider.TopTickBar.IsVisible = true;
                    if (slider.Orientation == Orientation.Horizontal)
                    {
                        slider.TrackBackground.Margin = new Thickness(5, 2, 5, 0);
                    }
                    else
                    {
                        slider.TrackBackground.Margin = new Thickness(2, 5, 0, 5);
                    }

                    break;

                case TickPlacement.BottomRight:
                    slider.BottomTickBar.IsVisible = true;
                    if (slider.Orientation == Orientation.Horizontal)
                    {
                        slider.TrackBackground.Margin = new Thickness(5, 2, 5, 0);
                    }
                    else
                    {
                        slider.TrackBackground.Margin = new Thickness(0, 5, 2, 5);
                    }

                    break;

                case TickPlacement.Both:
                    slider.TopTickBar.IsVisible = true;
                    slider.BottomTickBar.IsVisible = true;
                    break;
            }
        }

        private static void OnValueChanged(SliderEx slider, AvaloniaPropertyChangedEventArgs e)
        {
            slider.UpdateSelectionRangeElementPositionAndSize();

            double? oldValue = e.OldValue as double?;
            double? newValue = e.NewValue as double?;

            slider.OnValueChanged(oldValue.HasValue ? oldValue.Value : 0.0,
                newValue.HasValue ? newValue.Value : 0.0);
        }

        private static void OnSelectionEndChanged(SliderEx slider, AvaloniaPropertyChangedEventArgs e)
        {
            slider.UpdateSelectionRangeElementPositionAndSize();
        }

        private static void OnSelectionStartChanged(SliderEx slider, AvaloniaPropertyChangedEventArgs e)
        {
            double oldValue = (double)e.OldValue;
            double newValue = (double)e.NewValue;
            slider.UpdateSelectionRangeElementPositionAndSize();
        }

        #region Commands

        /// <summary>
        /// Increase Slider value
        /// </summary>
        public static ICommand IncreaseLargeCommand { get; private set; }

        /// <summary>
        /// Increase Slider value
        /// </summary>
        public static ICommand IncreaseSmallCommand { get; private set; }

        /// <summary>
        /// Decrease Slider value
        /// </summary>
        public static ICommand DecreaseLargeCommand { get; private set; }

        /// <summary>
        /// Decrease Slider value
        /// </summary>
        public static ICommand DecreaseSmallCommand { get; private set; }

        /// <summary>
        /// Set Slider value to mininum
        /// </summary>
        public static ICommand MinimizeValueCommand { get; private set; }

        /// <summary>
        /// Set Slider value to maximum
        /// </summary>
        public static ICommand MaximizeValueCommand { get; private set; }

        private void InitializeCommands()
        {
            IncreaseLargeCommand = ReactiveCommand.Create<object>(x => OnIncreaseLargeCommand(x),
                outputScheduler: RxApp.MainThreadScheduler);
            IncreaseSmallCommand = ReactiveCommand.Create<object>(x => OnIncreaseSmallCommand(x),
                outputScheduler: RxApp.MainThreadScheduler);

            DecreaseLargeCommand = ReactiveCommand.Create<object>(x => OnDecreaseLargeCommand(x),
                outputScheduler: RxApp.MainThreadScheduler);
            DecreaseSmallCommand = ReactiveCommand.Create<object>(x => OnDecreaseSmallCommand(x),
                outputScheduler: RxApp.MainThreadScheduler);

            MinimizeValueCommand = ReactiveCommand.Create<object>(x => OnMinimizeValueCommand(x),
                outputScheduler: RxApp.MainThreadScheduler);
            MaximizeValueCommand = ReactiveCommand.Create<object>(x => OnMaximizeValueCommand(x),
                outputScheduler: RxApp.MainThreadScheduler);
        }

        private void OnMinimizeValueCommand(object x)
        {
            OnMinimizeValue();
        }

        private void OnMaximizeValueCommand(object x)
        {
            OnMaximizeValue();
        }

        private void OnDecreaseSmallCommand(object x)
        {
            OnDecreaseSmall();
        }

        private void OnIncreaseSmallCommand(object x)
        {
            OnIncreaseSmall();
        }

        private void OnDecreaseLargeCommand(object x)
        {
            OnDecreaseLarge();
        }

        private void OnIncreaseLargeCommand(object x)
        {
            OnIncreaseLarge();
        }

        #endregion Commands

        /// <summary>
        /// updates the pseudo classes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="priority"></param>
        protected void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == OrientationProperty && e.NewValue is Orientation newValue)
            {
                UpdatePseudoClassesForOrientation(newValue);
            }
        }


        /// <summary>
        /// sets the pseudo class for orientation
        /// </summary>
        /// <param name="o"></param>
        private void UpdatePseudoClassesForOrientation(Orientation o)
        {
            PseudoClasses.Set(":vertical", o == Orientation.Vertical);
            PseudoClasses.Set(":horizontal", o == Orientation.Horizontal);
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            var result = e.GetCurrentPoint(this);

            if (IsMoveToPointEnabled && Track != null &&
                Track.Thumb != null && Track.Thumb.IsPointerOver == false)
            {
                Point pt = e.GetPosition(Track);
                double newValue = Track.ValueFromPoint(pt);
                if (DoubleUtil.IsDoubleFinite(newValue))
                {
                    UpdateValue(newValue);
                }

                e.Handled = true;
            }

            if (result.Properties.IsLeftButtonPressed)
            {
                _OnMouseLeftButtonDown(e);
            }

            base.OnPointerPressed(e);
        }

        private void UpdateValue(double value)
        {
            double snappedValue = SnapToTick(value);

            if (snappedValue != Value)
            {
                SetValue(ValueProperty, Math.Max(Minimum, Math.Min(Maximum, snappedValue)));
            }
        }

        private static void OnThumbDragCompleted(SliderEx sliderEx, VectorEventArgs args)
        {
            sliderEx.OnThumbDragCompleted(args);
        }

        private static void OnThumbDragDelta(SliderEx sliderEx, VectorEventArgs args)
        {
            sliderEx.OnThumbDragDelta(args);
        }

        private static void OnThumbDragStarted(SliderEx sliderEx, VectorEventArgs args)
        {
            sliderEx.OnThumbDragStarted(args);
        }

        protected virtual void OnThumbDragStarted(VectorEventArgs args)
        {
            // Show AutoToolTip if needed.
            Thumb thumb = args.Source as Thumb;

            if ((thumb == null) || (AutoToolTipPlacement == AutoToolTipPlacement.None))
            {
                return;
            }

            // Save original tooltip
            //_thumbOriginalToolTip = thumb.ToolTip;
            //if (_autoToolTip == null)
            //{
            //    _autoToolTip = new ToolTip();
            //    _autoToolTip.Placement = PlacementMode.Custom;
            //    _autoToolTip.PlacementTarget = thumb;
            //    _autoToolTip.CustomPopupPlacementCallback = new CustomPopupPlacementCallback(this.AutoToolTipCustomPlacementCallback);
            //}

            //thumb.ToolTip = _autoToolTip;
            //_autoToolTip.Content = GetAutoToolTipNumber();
            //_autoToolTip.IsOpen = true;
            //((Popup)_autoToolTip.Parent).Reposition();
        }

        protected virtual void OnThumbDragDelta(VectorEventArgs e)
        {
            Thumb thumb = e.Source as Thumb;
            // Convert to Track's co-ordinate
            if (Track != null && thumb == Track.Thumb)
            {
                double newValue = Value + Track.ValueFromDistance(e.Vector.X, e.Vector.Y);
                if (DoubleUtil.IsDoubleFinite(newValue))
                {
                    UpdateValue(newValue);
                }

                // Show AutoToolTip if needed
                //if (this.AutoToolTipPlacement != AutoToolTipPlacement.None)
                //{
                //    if (_autoToolTip == null)
                //    {
                //        _autoToolTip = new ToolTip();
                //    }

                //    _autoToolTip.Content = GetAutoToolTipNumber();

                //    if (thumb.ToolTip != _autoToolTip)
                //    {
                //        thumb.ToolTip = _autoToolTip;
                //    }

                //    if (!_autoToolTip.IsOpen)
                //    {
                //        _autoToolTip.IsOpen = true;
                //    }
                //    ((Popup)_autoToolTip.Parent).Reposition();
                //}
            }
        }

        protected virtual void OnThumbDragCompleted(VectorEventArgs e)
        {
            // Show AutoToolTip if needed.
            Thumb thumb = e.Source as Thumb;

            if ((thumb == null) || (AutoToolTipPlacement == AutoToolTipPlacement.None))
            {
                return;
            }

            //if (_autoToolTip != null)
            //{
            //    _autoToolTip.IsOpen = false;
            //}

            //thumb.ToolTip = _thumbOriginalToolTip;
        }

        /// <summary>
        /// Resize and resposition the SelectionRangeElement.
        /// </summary>
        private void UpdateSelectionRangeElementPositionAndSize()
        {
            Size trackSize = new Size(0d, 0d);
            Size thumbSize = new Size(0d, 0d);

            if (Track == null || DoubleUtil.LessThan(SelectionEnd, SelectionStart))
            {
                return;
            }

            trackSize = Track.DesiredSize; //Track.RenderSize;
            thumbSize = (Track.Thumb != null)
                ?
                /*Track.Thumb.RenderSize*/
                Track.Thumb.DesiredSize
                : new Size(0d, 0d);

            double range = Maximum - Minimum;
            double valueToSize;

            Layoutable rangeElement = SelectionRangeElement as Layoutable;

            if (rangeElement == null)
            {
                return;
            }

            if (Orientation == Orientation.Horizontal)
            {
                // Calculate part size for HorizontalSlider
                if (DoubleUtil.AreClose(range, 0d) || (DoubleUtil.AreClose(trackSize.Width, thumbSize.Width)))
                {
                    valueToSize = 0d;
                }
                else
                {
                    valueToSize = Math.Max(0.0, (trackSize.Width - thumbSize.Width) / range);
                }

                rangeElement.Width = ((SelectionEnd - SelectionStart) * valueToSize);
                if (IsDirectionReversed)
                {
                    Canvas.SetLeft(rangeElement,
                        (thumbSize.Width * 0.5) + Math.Max(Maximum - SelectionEnd, 0) * valueToSize);
                }
                else
                {
                    Canvas.SetLeft(rangeElement,
                        (thumbSize.Width * 0.5) + Math.Max(SelectionStart - Minimum, 0) * valueToSize);
                }
            }
            else
            {
                // Calculate part size for VerticalSlider
                if (DoubleUtil.AreClose(range, 0d) || (DoubleUtil.AreClose(trackSize.Height, thumbSize.Height)))
                {
                    valueToSize = 0d;
                }
                else
                {
                    valueToSize = Math.Max(0.0, (trackSize.Height - thumbSize.Height) / range);
                }

                rangeElement.Height = ((SelectionEnd - SelectionStart) * valueToSize);
                if (IsDirectionReversed)
                {
                    Canvas.SetTop(rangeElement,
                        (thumbSize.Height * 0.5) + Math.Max(SelectionStart - Minimum, 0) * valueToSize);
                }
                else
                {
                    Canvas.SetTop(rangeElement,
                        (thumbSize.Height * 0.5) + Math.Max(Maximum - SelectionEnd, 0) * valueToSize);
                }
            }
        }

        /// <summary>
        /// Snap the input 'value' to the closest tick.
        /// If input value is exactly in the middle of 2 surrounding ticks, it will be snapped to the tick that has greater value.
        /// </summary>
        /// <param name="value">Value that want to snap to closest Tick.</param>
        /// <returns>Snapped value if IsSnapToTickEnabled is 'true'. Otherwise, returns un-snaped value.</returns>
        private double SnapToTick(double value)
        {
            if (IsSnapToTickEnabled)
            {
                double previous = Minimum;
                double next = Maximum;

                // This property is rarely set so let's try to avoid the GetValue
                // caching of the mutable default value
                DoubleCollection ticks = null;

                if (GetValue(TicksProperty)
                    != null)
                {
                    ticks = Ticks;
                }

                // If ticks collection is available, use it.
                // Note that ticks may be unsorted.
                if ((ticks != null) && (ticks.Count > 0))
                {
                    for (int i = 0; i < ticks.Count; i++)
                    {
                        double tick = ticks[i];
                        if (DoubleUtil.AreClose(tick, value))
                        {
                            return value;
                        }

                        if (DoubleUtil.LessThan(tick, value) && DoubleUtil.GreaterThan(tick, previous))
                        {
                            previous = tick;
                        }
                        else if (DoubleUtil.GreaterThan(tick, value) && DoubleUtil.LessThan(tick, next))
                        {
                            next = tick;
                        }
                    }
                }
                else if (DoubleUtil.GreaterThan(TickFrequency, 0.0))
                {
                    previous = Minimum + (Math.Round(((value - Minimum) / TickFrequency)) * TickFrequency);
                    next = Math.Min(Maximum, previous + TickFrequency);
                }

                // Choose the closest value between previous and next. If tie, snap to 'next'.
                value = DoubleUtil.GreaterThanOrClose(value, (previous + next) * 0.5) ? next : previous;
            }

            return value;
        }

        // Sets Value = SnapToTick(value+direction), unless the result of SnapToTick is Value,
        // then it searches for the next tick greater(if direction is positive) than value
        // and sets Value to that tick
        private void MoveToNextTick(double direction)
        {
            if (direction != 0.0)
            {
                double value = Value;

                // Find the next value by snapping
                double next = SnapToTick(Math.Max(Minimum, Math.Min(Maximum, value + direction)));

                bool greaterThan = direction > 0; //search for the next tick greater than value?

                // If the snapping brought us back to value, find the next tick point
                if (next == value
                    && !(greaterThan && value == Maximum) // Stop if searching up if already at Max
                    && !(!greaterThan && value == Minimum)) // Stop if searching down if already at Min
                {
                    // This property is rarely set so let's try to avoid the GetValue
                    // caching of the mutable default value
                    DoubleCollection ticks = null;

                    if (GetValue(TicksProperty)
                        != null)
                    {
                        ticks = Ticks;
                    }

                    // If ticks collection is available, use it.
                    // Note that ticks may be unsorted.
                    if ((ticks != null) && (ticks.Count > 0))
                    {
                        for (int i = 0; i < ticks.Count; i++)
                        {
                            double tick = ticks[i];

                            // Find the smallest tick greater than value or the largest tick less than value
                            if ((greaterThan && DoubleUtil.GreaterThan(tick, value) &&
                                 (DoubleUtil.LessThan(tick, next) || next == value))
                                || (!greaterThan && DoubleUtil.LessThan(tick, value) &&
                                    (DoubleUtil.GreaterThan(tick, next) || next == value)))
                            {
                                next = tick;
                            }
                        }
                    }
                    else if (DoubleUtil.GreaterThan(TickFrequency, 0.0))
                    {
                        // Find the current tick we are at
                        double tickNumber = Math.Round((value - Minimum) / TickFrequency);

                        if (greaterThan)
                            tickNumber += 1.0;
                        else
                            tickNumber -= 1.0;

                        next = Minimum + tickNumber * TickFrequency;
                    }
                }

                // Update if we've found a better value
                if (next != value)
                {
                    SetValue(ValueProperty, next);
                }
            }
        }

        private void _OnMouseLeftButtonDown(PointerPressedEventArgs e)
        {
            SliderEx sliderEx = e.Source as SliderEx;

            // When someone click on the Slider's part, and it's not focusable
            // Slider need to take the focus in order to process keyboard correctly

            //if (!sliderEx.IsKeyboardFocusWithin)
            //{
            //    e.Handled = sliderEx.IsFocused || e.Handled;
            //}
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size size = base.ArrangeOverride(finalSize);

            if (TopTickBar != null)
            {
                TopTickBar.Width = finalSize.Width;
            }

            if (BottomTickBar != null)
            {
                BottomTickBar.Width = finalSize.Width;
            }

            UpdateSelectionRangeElementPositionAndSize();

            return size;
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            SelectionRangeElement = e.NameScope.Find<AvaloniaObject>(SelectionRangeElementName);
            Track = e.NameScope.Find<Track>(TrackName);
            TopTickBar = e.NameScope.Find<TickBar.TickBar>("TopTick");
            BottomTickBar = e.NameScope.Find<TickBar.TickBar>("BottomTick");

            TrackBackground = e.NameScope.Find<Border>("TrackBackground");

            //if (_autoToolTip != null)
            //{
            //    _autoToolTip.PlacementTarget = Track != null ? Track.Thumb : null;
            //}

            RaisePropertyChanged(TickPlacementProperty, TickPlacement.None, TickPlacement);
            base.OnApplyTemplate(e);
        }

        /// <summary>
        /// Call when Slider.IncreaseLarge command is invoked.
        /// </summary>
        protected virtual void OnIncreaseLarge()
        {
            MoveToNextTick(LargeChange);
        }

        /// <summary>
        /// Call when Slider.DecreaseLarge command is invoked.
        /// </summary>
        protected virtual void OnDecreaseLarge()
        {
            MoveToNextTick(-LargeChange);
        }

        /// <summary>
        /// Call when Slider.IncreaseSmall command is invoked.
        /// </summary>
        protected virtual void OnIncreaseSmall()
        {
            MoveToNextTick(SmallChange);
        }

        /// <summary>
        /// Call when Slider.DecreaseSmall command is invoked.
        /// </summary>
        protected virtual void OnDecreaseSmall()
        {
            MoveToNextTick(-SmallChange);
        }

        /// <summary>
        /// Call when Slider.MaximizeValue command is invoked.
        /// </summary>
        protected virtual void OnMaximizeValue()
        {
            SetValue(ValueProperty, Maximum);
        }

        /// <summary>
        /// Call when Slider.MinimizeValue command is invoked.
        /// </summary>
        protected virtual void OnMinimizeValue()
        {
            SetValue(ValueProperty, Minimum);
        }
    }
}