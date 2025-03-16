using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Text;
using System.Drawing.Drawing2D;

namespace Gamgee;

public class CustomCheckBox : CheckBox
{
    // Muted colors for dark mode
    private static readonly Color CheckedBackColor = Color.FromArgb(60, 80, 100);
    private static readonly Color UncheckedBackColor = Color.FromArgb(60, 60, 60);
    private static readonly Color CheckedBorderColor = Color.FromArgb(100, 130, 160);
    private static readonly Color UncheckedBorderColor = Color.FromArgb(120,120,120);
    private static readonly Color CheckMarkColor = Color.FromArgb(0, 240, 240);
    private static readonly Color TextColor = Color.FromArgb(240, 240, 240);
    private static readonly Color DisabledTextColor = Color.FromArgb(140, 140, 140);

    private bool _isHovered = false;
    private const int _boxSize = 13;

    public CustomCheckBox()
    {
        const ControlStyles styleFlags = ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer;
        SetStyle(styleFlags, true);
        AutoSize = true;
        ForeColor = TextColor;
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
    protected override void OnCheckedChanged(EventArgs e)
    {
        base.OnCheckedChanged(e);
        Invalidate();
    }
    protected override void OnPaint(PaintEventArgs e)
    {
        // Don't call base.OnPaint as we're completely custom drawing

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        e.Graphics.Clear(BackColor);

        var backColor = Checked ? CheckedBackColor : UncheckedBackColor;
        var borderColor = Checked ? CheckedBorderColor : UncheckedBorderColor;

        if (_isHovered && Enabled)
        {
            backColor = Lighten(backColor, 20);
            borderColor = Lighten(borderColor, 20);
        }
        if (!Enabled)
        {
            backColor = Darken(backColor, 20);
            borderColor = Darken(borderColor, 20);
        }
        var boxRect = new Rectangle(0, (Height - _boxSize) / 2, _boxSize, _boxSize);
        using (var brush = new SolidBrush(backColor))
        {
            e.Graphics.FillRectangle(brush, boxRect);
        }
        using (var pen = new Pen(borderColor))
        {
            e.Graphics.DrawRectangle(pen, boxRect);
        }
        if (Checked)
        {
            using var pen = new Pen(CheckMarkColor, 2f);
            var checkmarkPoints = new Point[]
            {
                new (boxRect.Left + 1, boxRect.Top + 7),
                new (boxRect.Left + 5, boxRect.Top + 12),
                new (boxRect.Right - 1, boxRect.Top + 1)
            };
            e.Graphics.DrawLines(pen, checkmarkPoints);
        }

        if (string.IsNullOrEmpty(Text)) return;
        var textRect = new Rectangle(boxRect.Right + 2, 0, Width - boxRect.Right + 2, Height);
        TextRenderer.DrawText(
            e.Graphics, Text, Font, textRect,
            Enabled ? TextColor : DisabledTextColor,
            TextFormatFlags.VerticalCenter | TextFormatFlags.Left
        );
    }

    private static Color Lighten(Color color, int amount)
    {
        return Color.FromArgb(
            color.A,
            Math.Min(color.R + amount, 255),
            Math.Min(color.G + amount, 255),
            Math.Min(color.B + amount, 255)
        );
    }
    private static Color Darken(Color color, int amount)
    {
        return Color.FromArgb(
            color.A,
            Math.Max(color.R - amount, 0),
            Math.Max(color.G - amount, 0),
            Math.Max(color.B - amount, 0)
        );
    }
}
