using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpBits.Base;

namespace DownloadKiller
{
    public partial class Form1 : Form
    {
        BitsManager manager;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            manager = new BitsManager();
            manager.EnumJobs(JobOwner.CurrentUser);

            foreach (var job in manager.Jobs.Values)
            {
                listBox1.Items.Add(job.DisplayName);
            }
        }

        private void btnKill_Click(object sender, EventArgs e)
        {
            foreach (var job in manager.Jobs.Values)
            {
                job.Cancel();  
            }

            listBox1.Items.Clear();
        }
    }
}
