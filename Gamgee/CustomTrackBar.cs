using System;
using System.Drawing;
using System.Windows.Forms;

namespace Gamgee
{
    public class CustomTrackBar : TrackBar
    {
        private static readonly Color TrackColor_Normal = Color.FromArgb(140, 140, 140);
        private static readonly Color TrackColor_Hovered = Color.FromArgb(200, 200, 200);

        private static readonly Color ThumbColor_Normal = Color.Aqua;
        private static readonly Color ThumbColor_Follower = Color.Peru;
        private static readonly Color ThumbColor_ErrState = Color.Red;
        private static readonly Color ThumbColor_Disabled = TrackColor_Normal;

        private bool _isHovered;
        private bool _isDragging;

        // our bar can be either driving data, or just following/reflecting external changes
        public bool IsFollower { get; set; }

        public CustomTrackBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            Size = new Size(180, 0);
            AutoSize = false;
        }

        protected override void OnValueChanged(EventArgs e)
        {
            base.OnValueChanged(e);
            Invalidate();
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _isHovered = true;
            Invalidate();
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _isHovered = false;
            Invalidate();
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button != MouseButtons.Left) return;
            _isDragging = true;
            UpdateValueFromMousePosition(e.X);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!_isDragging) return;
            UpdateValueFromMousePosition(e.X);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button != MouseButtons.Left) return;
            _isDragging = false;
            Invalidate();
        }

        private void UpdateValueFromMousePosition(int mouseX)
        {
            const int thumbSize = 10;

            // Constrain mouse position to valid range
            var effectiveX = Math.Max(0, Math.Min(mouseX, Width - thumbSize));
            
            // Calculate the new value based on constrained mouse position
            var ratio = (double)effectiveX / (Width - thumbSize);
            var newValue = Minimum + (int)Math.Round(ratio * (Maximum - Minimum));

            // Ensure value is within valid range
            newValue = Math.Max(Minimum, Math.Min(newValue, Maximum));
            
            // Update the value if it has changed
            if (newValue != Value)
            {
                Value = newValue;
                // Invalidate is called in OnValueChanged
            }
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(BackColor);

            const int trackHeight = 1;
            const int thumbSize = 10;

            var trackY = Height / 2 - trackHeight / 2;
            var thumbX = (int)((float)(Value - Minimum) / (Maximum - Minimum) * (Width - thumbSize));

            using Brush trackBrush = new SolidBrush(_isHovered ? TrackColor_Hovered : TrackColor_Normal);

            using Brush thumbBrush = new SolidBrush(
                (Enabled, IsFollower, Window.gaammaErrState) switch
                {
                    (false, _, _) => ThumbColor_Disabled,
                    (true, _, true) => ThumbColor_ErrState,
                    (true, false, false) => ThumbColor_Normal,
                    (true, true, false) => ThumbColor_Follower
                }
            );
            
            e.Graphics.FillRectangle(trackBrush, new Rectangle(0, trackY, Width, trackHeight));
            e.Graphics.FillEllipse(thumbBrush, new Rectangle(thumbX, Height / 2 - thumbSize / 2, thumbSize, thumbSize));
        }
    }
} 
