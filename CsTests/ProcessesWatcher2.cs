using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace StaxBar
{
    public class ProcessesWatcher2
    {
        public void Init(string processName)
        {
            var notepad = System.Diagnostics.Process.GetProcessesByName(processName).FirstOrDefault();

            if (notepad != null)
            {
                var notepadMainWindow = notepad.MainWindowHandle;
                AutomationElement notepadElement = AutomationElement.FromHandle(notepadMainWindow);

                Automation.AddAutomationEventHandler(
                    WindowPattern.WindowOpenedEvent, notepadElement,
                    TreeScope.Subtree, (s1, e1) =>
                    {
                        AutomationElement? element = s1 as AutomationElement;
                        if (element != null)
                        {
                            Debug.WriteLine("Opened: " + element.Current.Name);

                            Automation.AddAutomationEventHandler(
                                WindowPattern.WindowClosedEvent, element,
                                TreeScope.Subtree, (s2, e2) =>
                                {
                                    Debug.WriteLine("Closed");
                                });
                        }
                    });

                Automation.AddAutomationEventHandler(
                    WindowPattern.WindowClosedEvent, notepadElement,
                    TreeScope.Subtree, (s2, e2) =>
                    {
                        Debug.WriteLine("(Global) Closed");
                    });
            }
        }

        public void Cleanup()
        {
            Automation.RemoveAllEventHandlers();
        }
    }
}
