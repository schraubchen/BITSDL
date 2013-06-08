using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace msdl
{
    public partial class NewDownload : Form
    {
        public string downloadURL {get; private set;}

        public NewDownload()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            downloadURL = tbURL.Text;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void tbURL_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    downloadURL = tbURL.Text;
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    break;
            }
        }
    }
}
