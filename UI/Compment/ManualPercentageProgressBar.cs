using System;
using System.Drawing;
using System.Windows.Forms;

// 自定义进度条控件
public class DraggableProgressBar : ProgressBar
{
    private int _percentage = 0;

    // 百分比属性
    public int Percentage
    {
        get => _percentage;
        set
        {
            // 限制在0-100范围内
            if (value < 0) value = 0;
            if (value > 100) value = 100;

            if (_percentage != value)
            {
                _percentage = value;
                Invalidate(); // 刷新控件
                PercentageChanged?.Invoke(this, EventArgs.Empty); // 触发事件
            }
        }
    }

    // 百分比变化事件
    public event EventHandler PercentageChanged;

    public DraggableProgressBar()
    {
        // 启用双缓冲，减少闪烁
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer, true);
        Height = 30; // 增加高度，便于点击
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        // 显示文本
        string text = $"{Percentage}%";
        Graphics g = e.Graphics;
        Rectangle rect = ClientRectangle;

        // 绘制背景
        using (Brush backBrush = new SolidBrush(BackColor))
        {
            g.FillRectangle(backBrush, rect);
        }

        // 绘制进度
        int progressWidth = (int)(rect.Width * (Percentage / 100.0));
        Rectangle progressRect = new Rectangle(rect.X, rect.Y, progressWidth, rect.Height);
        using (Brush progressBrush = new SolidBrush(ForeColor))
        {
            g.FillRectangle(progressBrush, progressRect);
        }

        // 绘制文本
        StringFormat sf = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        // 根据进度调整文本颜色
        Brush textBrush = progressWidth > rect.Width / 2
            ? new SolidBrush(Color.White)
            : new SolidBrush(Color.Black);

        using (Font font = new Font("Segoe UI", 9f, FontStyle.Bold))
        {
            g.DrawString(text, font, textBrush, rect, sf);
        }

        textBrush.Dispose();
    }

    // 支持鼠标点击直接设置百分比
    protected override void OnMouseClick(MouseEventArgs e)
    {
        base.OnMouseClick(e);

        if (e.Button == MouseButtons.Left)
        {
            // 根据点击位置计算百分比
            int newPercentage = (int)((double)e.X / Width * 100);
            Percentage = newPercentage;
        }
        // 右键点击弹出输入框
        else if (e.Button == MouseButtons.Right)
        {
            ShowCustomInputDialog();
        }
    }

    // 自定义输入对话框
    private void ShowCustomInputDialog()
    {
        // 创建一个弹窗窗体
        using (Form inputForm = new Form())
        {
            inputForm.Text = "设置百分比";
            inputForm.Size = new Size(300, 150);
            inputForm.StartPosition = FormStartPosition.CenterParent;
            inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            inputForm.MaximizeBox = false;
            inputForm.MinimizeBox = false;

            // 添加标签
            Label label = new Label();
            label.Text = "请输入百分比值 (0-100):";
            label.Location = new Point(20, 20);
            label.Width = 240;
            inputForm.Controls.Add(label);

            // 添加输入框
            TextBox inputTextBox = new TextBox();
            inputTextBox.Text = this.Percentage.ToString();
            inputTextBox.Location = new Point(20, 50);
            inputTextBox.Width = 240;
            inputTextBox.KeyPress += (s, e) =>
            {
                // 只允许输入数字和退格键
                if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                {
                    e.Handled = true;
                }
            };
            inputForm.Controls.Add(inputTextBox);

            // 添加确认按钮
            Button okButton = new Button();
            okButton.Text = "确定";
            okButton.Location = new Point(20, 80);
            okButton.Width = 100;
            okButton.DialogResult = DialogResult.OK;
            inputForm.Controls.Add(okButton);

            // 添加取消按钮
            Button cancelButton = new Button();
            cancelButton.Text = "取消";
            cancelButton.Location = new Point(160, 80);
            cancelButton.Width = 100;
            cancelButton.DialogResult = DialogResult.Cancel;
            inputForm.Controls.Add(cancelButton);

            // 设置默认按钮
            inputForm.AcceptButton = okButton;
            inputForm.CancelButton = cancelButton;

            // 显示对话框
            if (inputForm.ShowDialog(this) == DialogResult.OK)
            {
                // 验证输入
                if (int.TryParse(inputTextBox.Text, out int value))
                {
                    Percentage = value; // 自动处理范围限制
                }
                else
                {
                    MessageBox.Show("请输入有效的整数！", "输入错误",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}

// 包含拖动功能的用户控件
public class PercentageControl : UserControl
{
    private DraggableProgressBar progressBar;
    private TrackBar trackBar;

    public int Percentage
    {
        get => progressBar.Percentage;
        set
        {
            progressBar.Percentage = value;
            trackBar.Value = value;
        }
    }

    public PercentageControl()
    {
        InitializeComponents();
        SetupEventHandlers();
    }

    private void InitializeComponents()
    {
        // 设置用户控件
        this.Dock = DockStyle.Top;
        this.Height = 60;
        this.Padding = new Padding(10);

        // 初始化进度条
        progressBar = new DraggableProgressBar();
        progressBar.Dock = DockStyle.Top;
        progressBar.ForeColor = Color.RoyalBlue;
        progressBar.BackColor = Color.LightGray;
        progressBar.Percentage = 0;

        // 初始化跟踪条
        trackBar = new TrackBar();
        trackBar.Dock = DockStyle.Top;
        trackBar.Top = progressBar.Bottom + 10;
        trackBar.Minimum = 0;
        trackBar.Maximum = 100;
        trackBar.Value = 0;
        trackBar.TickFrequency = 10;
        trackBar.Height = 20;

        // 添加控件
        this.Controls.Add(trackBar);
        this.Controls.Add(progressBar);
    }

    private void SetupEventHandlers()
    {
        // 跟踪条拖动时更新进度条
        trackBar.ValueChanged += (s, e) =>
        {
            progressBar.Percentage = trackBar.Value;
        };

        // 进度条变化时更新跟踪条
        progressBar.PercentageChanged += (s, e) =>
        {
            if (trackBar.Value != progressBar.Percentage)
            {
                trackBar.Value = progressBar.Percentage;
            }
        };
    }
}
