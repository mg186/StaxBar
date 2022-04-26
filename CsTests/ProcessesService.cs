using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace StaxBar;

public record ProcessInfo(string title, string processName, int idProcess, List<WinStruct> windows, Icon? ico)
{

}

public class WinStruct
{
    public string WinTitle { get; set; }
    public IntPtr WindowHandle { get; set; }
    public uint idProcess { get; set; }

    public bool appWindow { get; set; }
    public bool toolWindow { get; set; }
    public bool noactivate { get; set; }
    public bool windowedge { get; set; }
    public bool child { get; set; }
    public bool hasowner { get; set; }
    public bool isTopLevel { get; set; }
    public bool caption { get; set; }
    public bool disabled { get; set; }
    public bool dlgFrame { get; set; }
    public bool tabStop { get; set; }
    public bool visible { get; set; }
}

public enum GWL
{
    GWL_WNDPROC = (-4),
    GWL_HINSTANCE = (-6),
    GWL_HWNDPARENT = (-8),
    GWL_STYLE = (-16),
    GWL_EXSTYLE = (-20),
    GWL_USERDATA = (-21),
    GWL_ID = (-12)
}

public class ProcessesService
{
    public ProcessInfo[] GetApplications()
    {
        List<WinStruct> windows = Win32Helpers.GetWindows();
        var windowsByProcess = new Dictionary<uint, List<WinStruct>>();

        foreach (WinStruct win in windows)
        {
            if (windowsByProcess.ContainsKey(win.idProcess))
                windowsByProcess[win.idProcess].Add(win);
            else
                windowsByProcess.Add(win.idProcess, new List<WinStruct>() { win });
        }

        var result = new List<ProcessInfo>();
        foreach (Process p in Process.GetProcesses("."))
        {
            try
            {
                if (p.MainWindowTitle.Length > 0 || p.ProcessName == "explorer")
                {
                    // RegisterHook((uint)p.Id);

                    Icon? ico = null;
                    if (p.MainModule?.FileName != null)
                        ico = Icon.ExtractAssociatedIcon(p.MainModule.FileName);

                    var processInfo = new ProcessInfo(p.MainWindowTitle, p.ProcessName, p.Id, new List<WinStruct>(), ico);

                    if (windowsByProcess.ContainsKey((uint)p.Id))
                    {
                        processInfo = processInfo with { windows = windowsByProcess[(uint)p.Id] };
                        windowsByProcess.Remove((uint)p.Id);
                    }

                    result.Add(processInfo);
                }
                /*
                else
                {
                    Debug.WriteLine($"- Process {p.Id} - ProcessName={p.ProcessName} - SessionId={p.SessionId}");
                }
                */
            }
            catch (Exception ex)
            {
                // TODO: Err handling
                Console.WriteLine($"{ex.GetType().Name} - {ex.Message} - {ex.StackTrace}");
                Debug.WriteLine($"{ex.GetType().Name} - {ex.Message} - {ex.StackTrace}");
            }
        }

        string json = JsonConvert.SerializeObject(result, Formatting.Indented);
        Debug.WriteLine(json);

        //Debug.WriteLine("======== remainingWindows =========");
        //string remainingWindows = JsonConvert.SerializeObject(windowsByProcess, Formatting.Indented);
        //Debug.WriteLine(remainingWindows);

        return result.ToArray();
    }

    public void BringToFront(int idProcess, IntPtr handle)
    {
        Win32Helpers.BringToFront(idProcess, handle);
    }

    public void RegisterHook(uint idProcess)
    {
        /*
        var hook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND,
            IntPtr.Zero, new WinEventDelegate(WinEventProc),
            idProcess, 0, WINEVENT_OUTOFCONTEXT);
        */
        /*
        IntPtr handle;
        var notepadElement = AutomationElement.FromHandle(handle);
        Automation.AddAutomationEventHandler(
            WindowPattern.WindowOpenedEvent, notepadElement,
            TreeScope.Subtree, (s1, e1) =>
            {
                var element = s1 as AutomationElement;
                if (element.Current.Name == "Page Setup")
                {
                    //Page setup opened.
                    this.Invoke(new Action(() =>
                    {
                        this.Text = "Page Setup Opened";
                    }));
                    Automation.AddAutomationEventHandler(
                        WindowPattern.WindowClosedEvent, element,
                        TreeScope.Subtree, (s2, e2) =>
                        {
                            //Page setup closed.
                            this.Invoke(new Action(() =>
                            {
                                this.Text = "Closed";
                            }));
                        });
                }
            });
        */
    }

    

}
