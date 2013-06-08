using System;
using System.Windows.Forms;
using SharpBits.Base;

namespace DownloadKiller
{
    public partial class Form1 : Form
    {
        private BitsManager _manager;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _manager = new BitsManager();
            _manager.EnumJobs(JobOwner.CurrentUser);

            foreach (var job in _manager.Jobs.Values)
                listBox1.Items.Add(job.DisplayName);
        }

        private void btnKill_Click(object sender, EventArgs e)
        {
            foreach (var job in _manager.Jobs.Values)
                job.Cancel();

            listBox1.Items.Clear();
        }
    }
}
