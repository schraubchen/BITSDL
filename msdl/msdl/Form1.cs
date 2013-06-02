using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using SharpBits.Base;
using DownloadCtrl;

namespace msdl
{
    public partial class Form1 : Form
    {
        private BitsManager downloadManager;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if(!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Downloads"))
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Downloads");
            
            downloadManager = new BitsManager();
            downloadManager.EnumJobs(JobOwner.CurrentUser);

            loadDownloadControls();
        }

        private void loadDownloadControls()
        {
            foreach (var job in downloadManager.Jobs.Values)
            {
                DownloadControl dl = new DownloadControl(job);
                dl.Width = mainPanel.ClientRectangle.Width - 25;
                dl.Show();
                mainPanel.Controls.Add(dl);
            }
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewDownload dl = new NewDownload();
            if (dl.ShowDialog() == DialogResult.OK)
            {
                BitsJob job = downloadManager.CreateJob("File Download", JobType.Download);
                job.AddFile(dl.downloadURL, makeDownloadPath(dl.downloadURL));
                job.Resume();
                DownloadControl ctrl = new DownloadControl(job);
                ctrl.Width = mainPanel.ClientRectangle.Width - 25;
                ctrl.Show();
                mainPanel.Controls.Add(ctrl);
            }
        }

        private void mainPanel_Resize(object sender, EventArgs e)
        {
            foreach (DownloadControl ctrl in mainPanel.Controls)
            {
                ctrl.Width = mainPanel.ClientRectangle.Width - 25;
            }
        }

        private string makeDownloadPath(string url)
        {
            string fileName = "";
            fileName += Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Downloads\\";
            fileName += url.Substring(url.LastIndexOf("/"));
            return fileName;
        }
    }
}
