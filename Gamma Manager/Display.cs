using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Gamma_Manager;

internal class Display
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct PHYSICAL_MONITOR
    {
        public IntPtr hPhysicalMonitor;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szPhysicalMonitorDescription;
    }

    public class DisplayInfo
    {
        public int numDisplay;
        public string displayName;
        public string displayLink;
        public bool isExternal;
        public IntPtr PhysicalHandle;
        public WinApi.RECT rect;

        public RampGbc ramp_gbc = RampGbc.GetDefaultRampGbc();

        public int monitorBrightness;
        public int monitorContrast;

        public bool overlayEnabled = true;
        public bool overlayEnforced = false;
        public float ovTransparency = 1.0f;
        public DimmingOverlay overlay;

        public bool colorTempEnabled = false;
        public bool colorTempBlendEnabled = true;
        public int colorTemp = 6500;
    }

    [DllImport("dxva2.dll", EntryPoint = "GetNumberOfPhysicalMonitorsFromHMONITOR")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, ref uint pdwNumberOfPhysicalMonitors);

    [DllImport("dxva2.dll", EntryPoint = "GetPhysicalMonitorsFromHMONITOR")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, uint dwPhysicalMonitorArraySize, [Out] PHYSICAL_MONITOR[] pPhysicalMonitorArray);

    [DllImport("dxva2.dll", EntryPoint = "GetMonitorBrightness")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetMonitorBrightness(IntPtr handle, ref uint minimumBrightness, ref uint currentBrightness, ref uint maxBrightness);

    [DllImport("dxva2.dll", EntryPoint = "DestroyPhysicalMonitor")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DestroyPhysicalMonitor(IntPtr hMonitor);

    [DllImport("dxva2.dll", EntryPoint = "DestroyPhysicalMonitors")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DestroyPhysicalMonitors(uint dwPhysicalMonitorArraySize, [In] PHYSICAL_MONITOR[] pPhysicalMonitorArray);


    public static List<DisplayInfo> QueryDisplayDevices()
    {
        List<DisplayInfo> monitors = new List<DisplayInfo>();

        WinApi.MonitorEnumDelegate MonitorEnumProc = new WinApi.MonitorEnumDelegate (
            (IntPtr hMonitor, IntPtr hdcMonitor, ref WinApi.RECT lprcMonitor, IntPtr dwData) =>
            {
                WinApi.MONITORINFOEX monitorInfo = new WinApi.MONITORINFOEX() { Size = Marshal.SizeOf(typeof(WinApi.MONITORINFOEX)) };

                DisplayInfo monitor = new DisplayInfo();
                if (WinApi.GetMonitorInfo(hMonitor, ref monitorInfo))
                {
                    WinApi.DISPLAY_DEVICE device = new WinApi.DISPLAY_DEVICE();
                    device.Initialize();
                    if (WinApi.EnumDisplayDevices(monitorInfo.DeviceName.ToLPTStr(), 0, ref device, 0))
                    {
                        monitor.displayLink = monitorInfo.DeviceName;
                    }
                    string DName = device.DeviceID;
                    DName = DName.Substring(DName.IndexOf("\\") + 1);
                    DName = DName.Substring(0, DName.IndexOf("\\"));
                    monitor.displayName = DName;
                    monitor.rect = monitorInfo.Monitor;

                    /*Console.WriteLine("Left: " + lprcMonitor.Left);
                    Console.WriteLine("Right: " + lprcMonitor.Right);
                    Console.WriteLine("Top: " + lprcMonitor.Top);
                    Console.WriteLine("Bottom: " + lprcMonitor.Bottom);*/

                }

                uint physicalMonitorsCount = 0;

                if (!GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, ref physicalMonitorsCount)) {
                    return true; // Cannot get monitor count
                }

                var physicalMonitors = new PHYSICAL_MONITOR[physicalMonitorsCount];
                if (!GetPhysicalMonitorsFromHMONITOR(hMonitor, physicalMonitorsCount, physicalMonitors)) {
                    return true; // Cannot get physical monitor handle
                }

                foreach (PHYSICAL_MONITOR physicalMonitor in physicalMonitors)
                {
                    uint minValue = 0, currentValue = 0, maxValue = 0;
                    monitor.isExternal = true;
                    if (!GetMonitorBrightness(physicalMonitor.hPhysicalMonitor, ref minValue, ref currentValue, ref maxValue))
                    {
                        monitor.isExternal = false;
                        monitor.PhysicalHandle = (IntPtr)(-1);
                        DestroyPhysicalMonitor(physicalMonitor.hPhysicalMonitor);
                        continue;
                    }
                    monitor.PhysicalHandle = physicalMonitor.hPhysicalMonitor;
                }
                if (monitor.isExternal) {
                    monitor.monitorBrightness = ExternalMonitor.GetBrightness(monitor.PhysicalHandle);
                    monitor.monitorContrast = ExternalMonitor.GetContrast(monitor.PhysicalHandle);
                } else {
                    monitor.monitorBrightness = InternalMonitor.GetBrightness();
                    monitor.monitorContrast = -1;
                }

                monitor.overlay = new DimmingOverlay(monitor);

                monitors.Add(monitor);
                return true;
            }
        );

        WinApi.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, MonitorEnumProc, IntPtr.Zero);
        return monitors;
    }

    /*public static void DisposeMonitors(IEnumerable<DisplayInfo> monitors)
    {
        if (monitors?.Any() == true)
        {
            PHYSICAL_MONITOR[] monitorArray = monitors.Select(m => new PHYSICAL_MONITOR { hPhysicalMonitor = m.PhysicalHandle }).ToArray();
            DestroyPhysicalMonitors((uint)monitorArray.Length, monitorArray);
        }
    }*/

}
