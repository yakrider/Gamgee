using System;
using System.Management;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Gamma_Manager;



internal class DisplayInfo
{
    public int numDisplay;
    public string displayName;
    public string displayLink;
    public bool isExternal;
    public IntPtr PhysicalHandle;
    public Display.RECT rect;

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




internal static class Display
{

    
    public static List<DisplayInfo> QueryDisplayDevices()
    {
        var monitors = new List<DisplayInfo>();

        var MonitorEnumProc = new MonitorEnumDelegate (
            (IntPtr hMonitor, IntPtr _hdcMonitor, ref RECT _lprcMonitor, IntPtr _dwData) =>
            {
                var monitorInfo = new MONITORINFOEX { Size = Marshal.SizeOf(typeof(MONITORINFOEX)) };

                var monitor = new DisplayInfo();
                if (GetMonitorInfo(hMonitor, ref monitorInfo))
                {
                    var device = new DISPLAY_DEVICE();
                    device.Initialize();
                    if (EnumDisplayDevices(monitorInfo.DeviceName.ToLPTStr(), 0, ref device, 0))
                    {
                        monitor.displayLink = monitorInfo.DeviceName;
                    }
                    var DName = device.DeviceID;
                    DName = DName.Substring(DName.IndexOf("\\") + 1);
                    DName = DName.Substring(0, DName.IndexOf("\\"));
                    monitor.displayName = DName;
                    monitor.rect = monitorInfo.Monitor;

                }

                uint physicalMonitorsCount = 0;

                if (!GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, ref physicalMonitorsCount)) {
                    return true; // Cannot get monitor count
                }

                var physicalMonitors = new PHYSICAL_MONITOR[physicalMonitorsCount];
                if (!GetPhysicalMonitorsFromHMONITOR(hMonitor, physicalMonitorsCount, physicalMonitors)) {
                    return true; // Cannot get physical monitor handle
                }

                foreach (var physicalMonitor in physicalMonitors)
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
                    monitor.monitorBrightness = External.GetBrightness(monitor.PhysicalHandle);
                    monitor.monitorContrast = External.GetContrast(monitor.PhysicalHandle);
                } else {
                    monitor.monitorBrightness = Internal.GetBrightness();
                    monitor.monitorContrast = -1;
                }

                monitor.overlay = new DimmingOverlay(monitor);

                monitors.Add(monitor);
                return true;
            }
        );

        EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, MonitorEnumProc, IntPtr.Zero);
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
    
    
    
    


    public static class External
    {
        // External monitors can be handled via MS DirectX APIs

        public static void SetBrightness(IntPtr hPhysicalMonitor, uint brightness)
        {
            var realNewValue = 100 * brightness / 100;
            SetMonitorBrightness(hPhysicalMonitor, realNewValue);
        }

        public static int GetBrightness(IntPtr hPhysicalMonitor)
        {
            uint cur = 0, min = 0, max = 0;
            GetMonitorBrightness(hPhysicalMonitor, ref min, ref cur, ref max);

            return (int)cur;
        }

        public static void SetContrast(IntPtr hPhysicalMonitor, uint contrast)
        {
            var realNewValue = 100 * contrast / 100;
            SetMonitorContrast(hPhysicalMonitor, realNewValue);
        }

        public static int GetContrast(IntPtr hPhysicalMonitor)
        {
            uint cur = 0, min = 0, max = 0;
            GetMonitorContrast(hPhysicalMonitor, ref min, ref cur, ref max);

            return (int)cur;
        }
    }


    
    
    

    public static class Internal
    {
        // Internal monitors need to be handled via WMI (Windows Management Instrumentation)

        public static int GetBrightness()
        {
            var s = new ManagementScope("root\\WMI");
            var q = new SelectQuery("WmiMonitorBrightness");
            var mos = new ManagementObjectSearcher(s, q);
            var moc = mos.Get();

            byte curBrightness = 0;
            foreach (var managementBaseObject in moc)
            {
                var o = (ManagementObject)managementBaseObject;
                curBrightness = (byte)o.GetPropertyValue("CurrentBrightness");
                break;
                // ^^ only work on the first object
            }

            moc.Dispose();
            mos.Dispose();

            return curBrightness;
        }

        public static void SetBrightness(byte targetBrightness)
        {
            var s = new ManagementScope("root\\WMI");
            var q = new SelectQuery("WmiMonitorBrightnessMethods");
            var mos = new ManagementObjectSearcher(s, q);
            var moc = mos.Get();

            foreach (var managementBaseObject in moc)
            {
                var o = (ManagementObject)managementBaseObject;
                o.InvokeMethod("WmiSetBrightness", [UInt32.MaxValue, targetBrightness]);
                // ^^ note the reversed order - won't work otherwise!
                break;
                // ^^ only work on the first object
            }

            moc.Dispose();
            mos.Dispose();
        }
    }





    // simple helper fn
    private static byte[] ToLPTStr(this string str)
    {
        var lptArray = new byte[str.Length + 1];

        var index = 0;
        foreach (char c in str)
            lptArray[index++] = Convert.ToByte(c);

        lptArray[index] = Convert.ToByte('\0');

        return lptArray;
    }





    [StructLayout(LayoutKind.Sequential)]
    private struct DISPLAY_DEVICE
    {
        public int cb;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string DeviceName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceString;
        public DisplayDeviceStateFlags StateFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceKey;

        public void Initialize()
        {
            cb = 0;
            DeviceName = new string((char)32, 32);
            DeviceString = new string((char)32, 128);
            DeviceID = new string((char)32, 128);
            DeviceKey = new string((char)32, 128);
            cb = Marshal.SizeOf(this);
        }
    }



    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct PHYSICAL_MONITOR
    {
        public IntPtr hPhysicalMonitor;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szPhysicalMonitorDescription;
    }



    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct MONITORINFOEX
    {
        public int Size;
        public RECT Monitor;
        public RECT WorkArea;
        public uint Flags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string DeviceName;
    }


    [Flags]
    private enum DisplayDeviceStateFlags
    {
        /// <summary>The device is part of the desktop.</summary>
        AttachedToDesktop = 0x1,
        MultiDriver = 0x2,
        /// <summary>The device is part of the desktop.</summary>
        PrimaryDevice = 0x4,
        /// <summary>Represents a pseudo device used to mirror application drawing for remoting or other purposes.</summary>
        MirroringDriver = 0x8,
        /// <summary>The device is VGA compatible.</summary>
        VGACompatible = 0x10,
        /// <summary>The device is removable; it cannot be the primary display.</summary>
        Removable = 0x20,
        /// <summary>The device has more display modes than its output devices support.</summary>
        ModesPruned = 0x8000000,
        Remote = 0x4000000,
        Disconnect = 0x2000000,
    }


    private delegate bool MonitorEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

    [DllImport("user32.dll")]
    private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumDelegate lpfnEnum, IntPtr dwData);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFOEX lpmi);

    [DllImport("User32.dll")]
    private static extern bool EnumDisplayDevices(byte[] lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, int dwFlags);

    [DllImport("dxva2.dll", EntryPoint = "GetNumberOfPhysicalMonitorsFromHMONITOR")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, ref uint pdwNumberOfPhysicalMonitors);

    [DllImport("dxva2.dll", EntryPoint = "GetPhysicalMonitorsFromHMONITOR")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, uint dwPhysicalMonitorArraySize, [Out] PHYSICAL_MONITOR[] pPhysicalMonitorArray);

    [DllImport("dxva2.dll", EntryPoint = "DestroyPhysicalMonitor")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DestroyPhysicalMonitor(IntPtr hMonitor);

    [DllImport("dxva2.dll", EntryPoint = "DestroyPhysicalMonitors")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DestroyPhysicalMonitors(uint dwPhysicalMonitorArraySize, [In] PHYSICAL_MONITOR[] pPhysicalMonitorArray);

    // External monitor control imports
    [DllImport("dxva2.dll", EntryPoint = "GetMonitorBrightness")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetMonitorBrightness(IntPtr handle, ref uint minimumBrightness, ref uint currentBrightness, ref uint maxBrightness);

    [DllImport("dxva2.dll", EntryPoint = "SetMonitorBrightness")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetMonitorBrightness(IntPtr handle, uint newBrightness);

    [DllImport("dxva2.dll", EntryPoint = "GetMonitorContrast")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetMonitorContrast(IntPtr handle, ref uint minimumContrast, ref uint currentContrast, ref uint maxContrast);

    [DllImport("dxva2.dll", EntryPoint = "SetMonitorContrast")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetMonitorContrast(IntPtr handle, uint newContrast);



}
