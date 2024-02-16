## Click Anywhere

This answer should work with or without Infragistics. As I understand it, you want to be notified of a mouse click occurring _anywhere in any form_ of your app and when that happens you want to be able to inspect it to determine whether the click occurred specifically in "the tab" (or not). Implementing `IMessageFilter` for the main form of your app as show should give the expected behavior as shown:

[![mouse click event responses][1]][1]


```
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
```

When you receive the event, look at `sender` to see if it's a match for "the tab".
___

The implementation of `IMessageFilter` consists of a single method, and here the click "event" (the WM_LBUTTONDOWN _message_) will be detected so that the universal custom event can be raised.

```
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
    private const int WM_LBUTTONDOWN = 0x0201;
    OtherForm OtherForm = new OtherForm();
}
```
___


**Mock example of other forms**

```
public partial class OtherForm : Form
{
    public OtherForm() => InitializeComponent();
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if(e.CloseReason == CloseReason.UserClosing)
        {
            e.Cancel = true;
            Hide();
        }
        base.OnFormClosing(e);
    }
}
```
  [1]: https://i.stack.imgur.com/fa5DK.png