using System;
using System.Windows.Forms;

namespace msdl
{
    public partial class NewDownload : Form
    {
        public string DownloadUrl { get; private set; }

        public NewDownload()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DownloadUrl = tbURL.Text;
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void tbURL_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    DownloadUrl = tbURL.Text;
                    DialogResult = DialogResult.OK;
                    break;
            }
        }
    }
}
