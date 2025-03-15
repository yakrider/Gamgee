namespace Gamma_Manager;

using System;
using System.Runtime.InteropServices;

internal class DimmingOverlay
{
    private const int TIMER_ID = unchecked((int)0xdeadbeef);
    private const int timerTickMs = 20;

    private IntPtr hwnd;
    private IntPtr bgBrush;
    private IntPtr timerId;
    private DisplayInfo monitor;

    private static bool isClassRegistered;
    private static WindowProc wndProcDelegate;

    private delegate IntPtr WindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    public DimmingOverlay(DisplayInfo monitor)
    {
        this.monitor = monitor;
        bgBrush = CreateSolidBrush(0x000000);

        if (!isClassRegistered)
        {
            RegisterWindowClass();
            isClassRegistered = true;
        }
        Update();
    }

    ~DimmingOverlay()
    {
        Disable();
        DeleteObject(bgBrush);
    }

    private void Disable()
    {
        KillTopEnforceTimer();
        if (hwnd == IntPtr.Zero) return;
        DestroyWindow(hwnd);
        hwnd = IntPtr.Zero;
    }

    public void Update()
    {
        if (!monitor.overlayEnabled || monitor.ovTransparency > 99.99f)
        {
            Disable();
            return;
        }
        var x = monitor.rect.Left;
        var y = monitor.rect.Top;
        var width = monitor.rect.Right - x;
        var height = monitor.rect.Bottom - y;

        if (hwnd == IntPtr.Zero)
        {
            hwnd = CreateWindowEx(
                WS_EX_LAYERED | WS_EX_TRANSPARENT | WS_EX_TOPMOST | WS_EX_TOOLWINDOW,
                "DimmingOverlayClass", 
                "DimmingOverlayWindow", 
                WS_POPUP,
                x, y, width, height,
                IntPtr.Zero, IntPtr.Zero, GetModuleHandle(null), IntPtr.Zero
            );
            if (hwnd == IntPtr.Zero) return;
        }

        // Calculate opacity (0-255) .. max at 150 i.e ~60%
        var opacity = (byte)Gamma.Clamp(255f * (1 - monitor.ovTransparency), 0, 150);

        SetLayeredWindowAttributes(hwnd, 0, opacity, LWA_ALPHA);
        SetWindowPos(hwnd, HWND_TOPMOST, x, y, width, height, SWP_SHOWWINDOW | SWP_NOACTIVATE);
        UpdateWindow(hwnd);
        
        // Start the timer if needed
        if (monitor.overlayEnforced)
        {
            StartTopEnforceTimer();
        }
    }

    private void StartTopEnforceTimer()
    {
        KillTopEnforceTimer();
        if (monitor.overlayEnforced && hwnd != IntPtr.Zero)
        {
            timerId = SetTimer(hwnd, TIMER_ID, timerTickMs, IntPtr.Zero);
        }
    }

    private void KillTopEnforceTimer()
    {
        if (timerId != IntPtr.Zero && hwnd != IntPtr.Zero)
        {
            KillTimer(hwnd, timerId);
            timerId = IntPtr.Zero;
        }
    }

    private static IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        const uint WM_PAINT = 0x000F;
        const uint WM_TIMER = 0x0113;
        
        switch (msg)
        {
            case WM_PAINT:
                var hdc = BeginPaint(hWnd, out var ps);
                FillRect(hdc, ref ps.rcPaint, CreateSolidBrush(0x000000));
                EndPaint(hWnd, ref ps);
                return IntPtr.Zero;
                
            case WM_TIMER:
                if (wParam.ToInt32() == TIMER_ID)
                {
                    SetWindowPos (hWnd, HWND_TOPMOST, 0, 0, 0, 0, 0x0002 | 0x0001); // SWP_NOMOVE | SWP_NOSIZE
                    return IntPtr.Zero;
                }
                break;
        }
        
        return DefWindowProc(hWnd, msg, wParam, lParam);
    }

    private void RegisterWindowClass()
    {
        // Create a static delegate that won't be garbage collected
        wndProcDelegate = WndProc;
        
        var wc = new WNDCLASS
        {
            style = 0,
            lpfnWndProc = Marshal.GetFunctionPointerForDelegate(wndProcDelegate),
            cbClsExtra = 0,
            cbWndExtra = 0,
            hInstance = GetModuleHandle(null),
            hIcon = IntPtr.Zero,
            hCursor = IntPtr.Zero,
            hbrBackground = bgBrush,
            lpszMenuName = null,
            lpszClassName = "DimmingOverlayClass"
        };

        RegisterClass(ref wc);
        // ^^ if its return bool indicates error .. err 1410 means the class was already registered .. maybe another instance running
    }

    // Window styles and constants
    private const int WS_EX_LAYERED = 0x80000;
    private const int WS_EX_TRANSPARENT = 0x20;
    private const int WS_EX_TOPMOST = 0x8;
    private const int WS_EX_TOOLWINDOW = 0x80;
    private const int WS_POPUP = unchecked((int)0x80000000);
    private const int LWA_ALPHA = 0x2;
    private const int HWND_TOPMOST = -1;
    private const int SWP_SHOWWINDOW = 0x0040;
    private const int SWP_NOACTIVATE = 0x0010;


    [StructLayout(LayoutKind.Sequential)]
    private struct WNDCLASS
    {
        public uint style;
        public IntPtr lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        public string lpszMenuName;
        public string lpszClassName;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct PAINTSTRUCT
    {
        public IntPtr hdc;
        public bool fErase;
        public RECT rcPaint;
        public bool fRestore;
        public bool fIncUpdate;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] rgbReserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }


    [DllImport("user32.dll")]
    private static extern IntPtr CreateWindowEx(int dwExStyle, string lpClassName, string lpWindowName, int dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

    [DllImport("user32.dll")]
    private static extern bool DestroyWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern IntPtr SetTimer(IntPtr hWnd, int nIDEvent, int uElapse, IntPtr lpTimerFunc);

    [DllImport("user32.dll")]
    private static extern bool KillTimer(IntPtr hWnd, IntPtr uIDEvent);

    [DllImport("user32.dll")]
    private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    private static extern bool UpdateWindow(IntPtr hWnd);

    [DllImport("gdi32.dll")]
    private static extern IntPtr CreateSolidBrush(int crColor);

    [DllImport("gdi32.dll")]
    private static extern bool DeleteObject(IntPtr hObject);

    [DllImport("user32.dll")]
    private static extern IntPtr DefWindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern ushort RegisterClass(ref WNDCLASS lpWndClass);

    [DllImport("user32.dll")]
    private static extern IntPtr BeginPaint(IntPtr hWnd, out PAINTSTRUCT lpPaint);

    [DllImport("user32.dll")]
    private static extern bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT lpPaint);

    [DllImport("user32.dll")]
    private static extern bool FillRect(IntPtr hdc, [In] ref RECT lprc, IntPtr hbr);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetModuleHandle(string lpModuleName);


}
