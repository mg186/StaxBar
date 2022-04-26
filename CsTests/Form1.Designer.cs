namespace StaxBar;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    private List<Button> btnProcesses = new List<Button>();

    ProcessesService processesService = new ProcessesService();

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {

        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }


    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        ProcessInfo[] processes = this.processesService.GetApplications();

        int counter = 0;
        foreach (ProcessInfo process in processes)
        {
            foreach (var window in process.windows)
            {
                if (string.IsNullOrWhiteSpace(window.WinTitle))
                    continue;

                /*
                // TODO: Peut-on éliminer ces patches?
                if (process.title == "Switch USB")
                    continue;

                if (process.processName == "RtWlan")
                    continue;

                if (window.WinTitle.StartsWith("MSCTFIME"))
                    continue;

                if (window.WinTitle == "Default IME")
                    continue;

                if (window.WinTitle == "MediaContextNotificationWindow")
                    continue;

                if (window.WinTitle == "Microsoft Text Input Application")
                    continue;

                if (window.WinTitle == "SystemResourceNotifyWindow")
                    continue;

                if (window.WinTitle == "Hidden Window")
                    continue;

                if (window.WinTitle == "CiceroUIWndFrame")
                    continue;

                if (window.WinTitle == "VCL ImplGetDefaultWindow")
                    continue;

                if (window.WinTitle.StartsWith("GDI+ Window"))
                    continue;

                if (window.WinTitle == "DDE Server Window")
                    continue;

                if (window.WinTitle.StartsWith(".NET-BroadcastEventWindow"))
                    continue;

                if (window.WinTitle == "SleepDetectorServiceWindowClass")
                    continue;

                if (window.WinTitle == "NVIDIA GeForce Overlay")
                    continue;

                if (window.WinTitle == "Visual Studio Application Management Window")
                    continue;

                if (process.processName == "ApplicationFrameHost")
                    continue;
                
                */
                if (window.toolWindow && !window.appWindow)
                    continue;

                if (window.noactivate && !window.appWindow)
                    continue;

                if (window.child && !window.appWindow)
                    continue;

                if (window.hasowner && !window.appWindow)
                    continue;

                if (!window.isTopLevel && !window.appWindow)
                    continue;

                if (!window.tabStop && !window.appWindow && !window.dlgFrame && !window.caption)
                    continue;

                if (!window.visible && !window.appWindow)
                    continue;


                /*
                DETERMINE IF WINDOW HAS TASKBAR BUTTON
                Toplevel window
                WS_EX_APPWINDOW -> taskbar, no matter the other styles!
                OWNER must be NULL (GetWindow(window, GW_OWNER))
                no: WS_EX_NOACTIVATE or WS_EX_TOOLWINDOW:
                 */

                var button = new System.Windows.Forms.Button();
                button.Location = new System.Drawing.Point(5 + (500 * (counter % 3)), (counter / 3) * 60);
                button.Name = $"btnProcess_{counter}";
                button.Size = new System.Drawing.Size(500, 60);
                button.TabIndex = 0;
                button.Text = window.WindowHandle + " p[" + process.processName + "] t[" + process.title
                    + "]\r\nw[" + window.WinTitle + "] tw:" + (window.toolWindow ? "1" : "0")
                    + " noa:" + (window.noactivate ? "1" : "0")
                    + " chld:" + (window.child ? "1" : "0")
                    + " own:" + (window.hasowner ? "1" : "0")
                    + " aw:" + (window.appWindow ? "1" : "0")
                    + " tlv:" + (window.isTopLevel ? "1" : "0")
                    + " cpt:" + (window.caption ? "1" : "0")
                    + " dgf:" + (window.dlgFrame ? "1" : "0")
                    + " dbl:" + (window.disabled ? "1" : "0")
                    + " tsp:" + (window.tabStop ? "1" : "0")
                    + " vbl:" + (window.visible ? "1" : "0");
                button.TextAlign = System.Drawing.ContentAlignment.TopLeft;
                button.UseVisualStyleBackColor = true;
                button.Click += new System.EventHandler((object sender, EventArgs e) =>
                {
                    this.processesService.BringToFront(process.idProcess, window.WindowHandle);
                });

                if (process.ico != null)
                {
                    button.Image = process.ico.ToBitmap();
                }

                this.Controls.Add(button);
                this.btnProcesses.Add(button);

                counter++;
            }
        }

        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(1000, 800);
        this.Name = "Form1";
        this.Text = "Form1";
        this.ResumeLayout(false);

    }

}
