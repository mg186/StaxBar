using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace StaxBar;

public struct ProcessWatcher3
{
    public const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
    public const uint EVENT_OBJECT_DESTROY = 0x8001;
    public const uint WINEVENT_OUTOFCONTEXT = 0;
    public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd,
        int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

    [DllImport("user32.dll")]
    public static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr
        hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess,
        uint idThread, uint dwFlags);

    [DllImport("user32.dll")]
    public static extern bool UnhookWinEvent(IntPtr hWinEventHook);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    IntPtr hook = IntPtr.Zero;

    //GCHandle DelegateHandle;
    //IntPtr FunctionPointer;

    public ProcessWatcher3()
    {
        hook = IntPtr.Zero;
    }

    public void Init(string processName)
    {
        var p = System.Diagnostics.Process.GetProcessesByName(processName).FirstOrDefault();
        if (p != null)
        {
            //DelegateHandle = GCHandle.Alloc(new WinEventDelegate(WinEventProc));
            //FunctionPointer = Marshal.GetFunctionPointerForDelegate(new WinEventDelegate(WinEventProc));

            hook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND,
                IntPtr.Zero, new WinEventDelegate(WinEventProc), // FunctionPointer
                (uint)p.Id, 0, WINEVENT_OUTOFCONTEXT);
        }
    }

    public void CleanUp()
    {
        UnhookWinEvent(hook);
    }

    void WinEventProc(IntPtr hWinEventHook, uint eventType,
        IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        StringBuilder sb = new StringBuilder(500);
        GetWindowText(hwnd, sb, sb.Capacity);
        Debug.WriteLine("[Approche3] Opened: " + sb.ToString());
    }

}
