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
                if (sender is Control parent)
                {
                    string message;
                    switch (parent.GetType().Name)
                    {
                        case nameof(MainForm): richTextBox.SelectionColor = Color.Green; break;
                        case nameof(OtherForm): richTextBox.SelectionColor = Color.Blue; break;
                        default: richTextBox.SelectionColor = SystemColors.ControlText; break;
                    }
                    if (e.Control is Control child)
                    {
                        message = $"{parent.Name}.{child.Name}{Environment.NewLine}";
                    }
                    else message = $"{parent.Name}{Environment.NewLine}";
                    richTextBox.AppendText(message);
                }
            };

            buttonShowOther.Click += (sender, e) =>
                OtherForm.Show(this); // Pass 'this' to keep child form on top.
        }
        OtherForm OtherForm = new OtherForm();

        private const int WM_LBUTTONDOWN = 0x0201;
        public bool PreFilterMessage(ref Message m)
        {
            if(m.Msg == WM_LBUTTONDOWN && Control.FromHandle(m.HWnd) is Control control)
            {
                BeginInvoke((MethodInvoker)delegate
                {
                    ClickAnywhere?.Invoke(
                        control.TopLevelControl,
                        new ClickAnywhereEventArgs(control));
                });
            }
            return false;
        }
        event EventHandler<ClickAnywhereEventArgs> ClickAnywhere;
    }
    delegate void ClickAnywhereEventHandler(Object sender, ClickAnywhereEventArgs e);
    class ClickAnywhereEventArgs : EventArgs
    {
        public ClickAnywhereEventArgs(Control control) => Control = control;
        public Control Control { get; }
    }
}
