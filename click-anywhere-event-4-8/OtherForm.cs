using System.Windows.Forms;

namespace click_anywhere_event_4_8
{
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
}
