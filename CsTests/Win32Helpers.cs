using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace StaxBar
{
    internal class Win32Helpers
    {

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool ShowWindow(IntPtr handle, int nCmdShow);

        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool IsIconic(IntPtr handle);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

        const int SW_RESTORE = 9;


        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(CallBackPtr lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        const long WS_EX_APPWINDOW = 0x00040000L;

        const long WS_EX_TOOLWINDOW = 0x00000080L;

        const long WS_EX_NOACTIVATE = 0x08000000L;

        const long WS_EX_WINDOWEDGE = 0x00000100L;

        const long WS_CHILD = 0x40000000L;

        const long WS_CAPTION = 0x00C00000L;

        const long WS_DISABLED = 0x08000000L;

        const long WS_DLGFRAME = 0x00400000L;

        const long WS_TABSTOP = 0x00010000L;

        const long WS_VISIBLE = 0x10000000L;

        public const uint GW_OWNER = 4;
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        /// <summary>
        /// Retrieves the handle to the ancestor of the specified window.
        /// </summary>
        /// <param name="hwnd">A handle to the window whose ancestor is to be retrieved.
        /// If this parameter is the desktop window, the function returns NULL. </param>
        /// <param name="flags">The ancestor to be retrieved.</param>
        /// <returns>The return value is the handle to the ancestor window.</returns>
        [DllImport("user32.dll", ExactSpelling = true)]
        static extern IntPtr GetAncestor(IntPtr hwnd, uint flags);

        private const uint GA_PARENT = 1; // Retrieves the parent window.This does not include the owner, as it does with the GetParent function.

        private const uint GA_ROOT = 2; // Retrieves the root window by walking the chain of parent windows.

        private const uint GA_ROOTOWNER = 3; // Retrieves the owned root window by walking the chain of parent and owner windows returned by GetParent.

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

        public static void BringToFront(int idProcess, IntPtr handle)
        {
            //IntPtr handle = process.MainWindowHandle;

            if (IsIconic(handle))
            {
                ShowWindow(handle, SW_RESTORE);
            }

            SetForegroundWindow(handle);
        }

        private delegate bool CallBackPtr(int hwnd, int lParam);
        private static CallBackPtr callBackPtr = Callback;
        private static List<WinStruct> _WinStructList = new List<WinStruct>();

        /*
        void WinEventProc(IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            StringBuilder sb = new StringBuilder();
            GetWindowText(hwnd, sb, sb.Capacity);
            Debug.WriteLine($"Hook: forground window is : {sb}");
        }
        */

        private static bool Callback(int hWnd, int lparam)
        {
            StringBuilder sb = new StringBuilder(256);
            int res = GetWindowText((IntPtr)hWnd, sb, 256);
            uint res2 = GetWindowThreadProcessId((IntPtr)hWnd, out uint processId);

            IntPtr longStyle = GetWindowLongPtr((IntPtr)hWnd, (int)GWL.GWL_STYLE);
            IntPtr longStyleEx = GetWindowLongPtr((IntPtr)hWnd, (int)GWL.GWL_EXSTYLE);
            bool appWindow = (longStyleEx.ToInt64() & WS_EX_APPWINDOW) > 0;
            bool toolWindow = (longStyleEx.ToInt64() & WS_EX_TOOLWINDOW) > 0;
            bool noactive = (longStyleEx.ToInt64() & WS_EX_NOACTIVATE) > 0;
            bool windowedge = (longStyleEx.ToInt64() & WS_EX_WINDOWEDGE) > 0;
            bool child = (longStyle.ToInt64() & WS_CHILD) > 0;
            bool caption = (longStyle.ToInt64() & WS_CAPTION) > 0;
            bool disabled = (longStyle.ToInt64() & WS_DISABLED) > 0;
            bool dlgFrame = (longStyle.ToInt64() & WS_DLGFRAME) > 0;
            bool tabStop = (longStyle.ToInt64() & WS_TABSTOP) > 0;
            bool visible = (longStyle.ToInt64() & WS_VISIBLE) > 0;

            IntPtr ancestor = GetAncestor((IntPtr)hWnd, GA_ROOT);
            bool isTopLevel = (ancestor == (IntPtr)hWnd);

            IntPtr ownerHandle = GetWindow((IntPtr)hWnd, GW_OWNER);
            bool hasowner = (ownerHandle.ToInt64() > 0);

            //Debug.WriteLine($"{sb} - appWindow={appWindow} - toolWindow={toolWindow} - noactive={noactive} - windowedge={windowedge} - child={child} - longStyleEx ={longStyleEx.ToInt64()}");

            _WinStructList.Add(new WinStruct
            {
                WindowHandle = (IntPtr)hWnd,
                WinTitle = sb.ToString(),
                idProcess = processId,
                appWindow = appWindow,
                noactivate = noactive,
                toolWindow = toolWindow,
                windowedge = windowedge,
                child = child,
                hasowner = hasowner,
                isTopLevel = isTopLevel,
                caption = caption,
                disabled = disabled,
                dlgFrame = dlgFrame,
                tabStop = tabStop,
                visible = visible
            });
            return true;
        }

        public static List<WinStruct> GetWindows()
        {
            _WinStructList = new List<WinStruct>();
            EnumWindows(callBackPtr, IntPtr.Zero);
            return _WinStructList;
        }

        /*
        public void GetProcesses()
        {
            StringBuilder sb = new StringBuilder();
            ManagementClass MgmtClass = new ManagementClass("Win32_Process");

            foreach (ManagementObject mo in MgmtClass.GetInstances())
                Console.WriteLine("Name:" + mo["Name"] + "ID:" + mo["ProcessId"]);

            Console.WriteLine();
        }
        */


    }
}
