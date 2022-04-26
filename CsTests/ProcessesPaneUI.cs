namespace StaxBar;

public class ProcessesPaneUI : Form
{

    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    private List<Button> btnProcesses = new List<Button>();

    private ProcessesService processesService = new ProcessesService();

    private Button startButton;

    public ProcessesPaneUI()
    {
        InitializeComponent();
    }

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

    protected int ItemHeight => 30;


    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.AutoScaleDimensions = new SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(400, 800);
        this.Name = "StaxBar11";
        this.Text = "StaxBar11";
        this.AutoScroll = true;
        //this.SetStyle(ControlStyles.
        this.FormBorderStyle = FormBorderStyle.SizableToolWindow;

        ProcessInfo[] processes = this.processesService.GetApplications();

        int counter = 0;

        startButton = new Button();
        //button.Location = new System.Drawing.Point(5 + (500 * (counter % 3)), (counter / 3) * 60);
        startButton.Location = new Point(0, counter * ItemHeight);
        startButton.Name = $"btnStart";
        startButton.Size = new Size(350, ItemHeight);
        startButton.Margin = new Padding(0);
        startButton.TabIndex = 0;
        startButton.Text = "START";

        startButton.UseVisualStyleBackColor = true;
        startButton.Click += new System.EventHandler((object sender, EventArgs e) =>
        {
            Win32Helpers.ShowStartMenu();
        });

        startButton.TextAlign = ContentAlignment.MiddleLeft;

        startButton.Padding = new Padding(0, 0, 0, 0);

        this.Controls.Add(startButton);

        foreach (ProcessInfo process in processes)
        {
            foreach (var window in process.windows)
            {
                if (string.IsNullOrWhiteSpace(window.WinTitle))
                    continue;

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
                button.Location = new System.Drawing.Point(0, counter * ItemHeight);
                button.Name = $"btnProcess_{counter}";
                button.Size = new System.Drawing.Size(350, ItemHeight);
                button.TabIndex = 0;
                button.Text = window.WinTitle;
                
                /*
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
                    + " vbl:" + (window.visible ? "1" : "0");*/
                button.TextAlign = System.Drawing.ContentAlignment.TopLeft;
                button.UseVisualStyleBackColor = true;
                button.Click += new System.EventHandler((object sender, EventArgs e) =>
                {
                    this.processesService.BringToFront(process.idProcess, window.WindowHandle);
                });

                if (process.ico != null)
                {
                    button.Image = process.ico.ToBitmap();
                    button.ImageAlign = ContentAlignment.MiddleLeft;
                    button.TextImageRelation = TextImageRelation.ImageBeforeText;
                }

                button.TextAlign = ContentAlignment.MiddleLeft;

                button.Padding = new Padding(0, 0, 0, 0);

                this.Controls.Add(button);
                this.btnProcesses.Add(button);

                counter++;
            }
        }

        this.ResumeLayout(false);

    }

}
