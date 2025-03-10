using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Gamma_Manager;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        const string appGuid = "{E87313A6-C2A9-4C04-82A2-6999C81A32E3}";

        using var mutex = new Mutex(false, "Global\\" + appGuid);
        if (!mutex.WaitOne(0,false))
        {
            BringExistingInstanceToFront();
            return;
        }
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new Window());
    }


    private static void BringExistingInstanceToFront()
    {
        var currentProcess = Process.GetCurrentProcess();
        var existingProcess = Array.Find (
            Process.GetProcessesByName(currentProcess.ProcessName),
            p => p.Id != currentProcess.Id
        );

        if (existingProcess == null) return;

        var hWnd = FindMainWindow(existingProcess.Id);
        if (hWnd == IntPtr.Zero) return;

        ShowWindowAsync(hWnd, SW_NORMAL);
        // the following can sometime help bring it up even if its been minimized etc .. (but still not fully robustly)
        if (IsIconic(hWnd)) {
            ShowWindowAsync(hWnd, SW_RESTORE);
        } else {
            ShowWindowAsync(hWnd, SW_SHOW);
        }

        SetWindowPos (hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW | SWP_ASYNCWINDOWPOS);

        SetForegroundWindow(hWnd);
    }

    private static IntPtr FindMainWindow(int processId)
    {
        var foundHandle = IntPtr.Zero;
        EnumWindows ( (hWnd, lParam) =>
        {
            GetWindowThreadProcessId (hWnd, out var windowProcessId);
            if (windowProcessId != processId) return true;

            foundHandle = hWnd;
            return false;
        }, IntPtr.Zero);

        return foundHandle;
    }




    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool IsIconic(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern int EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int processId);

    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

    private const int SW_NORMAL = 0x01;
    private const int SW_SHOW = 0x05;
    private const int SW_RESTORE = 0x09;

    [DllImport("user32.dll")]
    private static extern bool BringWindowToTop(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern void SwitchToThisWindow(IntPtr hWnd, bool unknown);

    [DllImport("user32.dll")]
    private static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
    private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_SHOWWINDOW = 0x0040;
    private const uint SWP_ASYNCWINDOWPOS = 0x4000;

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public static extern void OutputDebugString(string message);
}
