namespace StaxBar;

public class ProcessesPaneUI : Form
{

    private ProcessesService processesService = new ProcessesService();


    private (ProcessInfo, WinStruct)[] processWindows = new (ProcessInfo, WinStruct)[0];

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
        base.Dispose(disposing);
    }

    protected int ItemHeight => 25;
    protected int IconWidth => 20;
    protected int IconHeight => 20;
    protected int MarginLeft => 10;
    protected int MarginTop => 10;
    protected int FontSize => 11;
    protected Color TextColor => Color.Black;
    protected string FontName => "Arial";


    private void UpdateProcessList()
    {
        this.processWindows = this.processesService.GetTopLevelWindows().ToArray();
    }

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
        this.FormBorderStyle = FormBorderStyle.SizableToolWindow;

        UpdateProcessList();

        this.Click += ProcessesPaneUI_Click;

        this.ResumeLayout(false);

    }

    private void ProcessesPaneUI_Click(object? sender, EventArgs e)
    {
        MouseEventArgs mouseEvent = (MouseEventArgs)e;
        int shiftedY = (mouseEvent.Y - this.MarginTop - this.ItemHeight);
        if (shiftedY < 0)
        {
            Win32Helpers.ShowStartMenu();
        }
        else
        {
            int index = shiftedY / this.ItemHeight;

            if (index >= 0 && index < this.processWindows.Length)
            {
                (ProcessInfo process, WinStruct window) = this.processWindows[index];
                this.processesService.BringToFront(process.idProcess, window.WindowHandle);
            }
        }
    }

    public void RefreshData()
    {
        this.Invoke((MethodInvoker)delegate
        {
            UpdateProcessList();

            this.Refresh();
        });
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        var font = new Font(this.FontName, this.FontSize);
        var brush = new SolidBrush(this.TextColor);

        int startY = this.MarginTop;
        var startCoord = new PointF(this.MarginLeft + this.IconWidth + this.MarginLeft, startY);
        e.Graphics.DrawString("== START ==", font, brush, startCoord);

        int i = 1;
        foreach ((ProcessInfo process, WinStruct window) in this.processWindows)
        {
            int y = this.MarginTop + i * this.ItemHeight;
            var coord = new PointF(this.MarginLeft + this.IconWidth + this.MarginLeft, y);
            e.Graphics.DrawString(window.WinTitle, font, brush, coord);

            if (process.ico != null)
            {
                Bitmap image = process.ico.ToBitmap();
                e.Graphics.DrawIcon(process.ico, new Rectangle(this.MarginLeft, y, this.IconWidth, this.IconHeight));
            }

            i++;
        }

    }

}
