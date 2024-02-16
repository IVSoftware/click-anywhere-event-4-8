using System;
using System.Drawing;
using System.Windows.Forms;

namespace click_anywhere_event_4_8
{
    public partial class MainForm : Form, IMessageFilter
    {
        public MainForm()
        {
            InitializeComponent();
            Application.AddMessageFilter(this);
            Disposed += (sender, e) =>
            {
                Application.RemoveMessageFilter(this);
                OtherForm.Dispose();
            };
            ClickAnywhere += (sender, e) =>
            {
                if (sender is Control clickedOn)
                {
                    switch (clickedOn?.TopLevelControl?.GetType().Name)
                    {
                        case nameof(MainForm): richTextBox.SelectionColor = Color.Green; break;
                        case nameof(OtherForm): richTextBox.SelectionColor = Color.Blue; break;
                        default: richTextBox.SelectionColor = SystemColors.ControlText; break;
                    }
                    string message = $"{clickedOn?.TopLevelControl?.Name}.{clickedOn.Name}{Environment.NewLine}";
                    richTextBox.AppendText(message);
                }
            };
            buttonShowOther.Click += (sender, e) =>
            {
                if (!OtherForm.Visible)
                {
                    OtherForm.Show(this); // Pass 'this' to keep child form on top.
                    OtherForm.Location = new Point(Left + 100, Top + 100);
                }
            };
        }
        OtherForm OtherForm = new OtherForm();

        private const int WM_LBUTTONDOWN = 0x0201;
        public bool PreFilterMessage(ref Message m)
        {
            if(m.Msg == WM_LBUTTONDOWN && Control.FromHandle(m.HWnd) is Control control)
            {
                BeginInvoke((MethodInvoker)delegate
                {
                    ClickAnywhere?.Invoke(control, EventArgs.Empty);
                });
            }
            return false;
        }
        public event EventHandler ClickAnywhere;
    }
}
