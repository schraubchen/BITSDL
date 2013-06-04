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

/*
 * TODO:
 *      High priority:
 *          exchange the download control with a datagridview.
 *          build a progress bar column http://stackoverflow.com/questions/4646920/populating-a-datagridview-with-text-and-progressbars
 *          make the download directory configurable.
 *
 *      Low priority:
 *          build a tray status monitor.
 */


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

            toolStripStatusLabel1.Text = string.Format("{0} job/s completed in the background.", loadDownloadControls());
        }

        /// <summary>
        /// Returns the count of completed jobs.
        /// </summary>
        /// <returns></returns>
        private int loadDownloadControls()
        {
            Console.WriteLine(string.Format("Jobs found: {0}", downloadManager.Jobs.Values.Count));
            
            int completed = 0;
            foreach (var job in downloadManager.Jobs.Values)
            {
                if (job.State == JobState.Transferred)
                {
                    job.Complete();
                    completed++;
                }
                else
                {
                    DownloadControl dl = new DownloadControl(job);
                    dl.Width = mainPanel.ClientRectangle.Width - 25;
                    dl.Show();
                    mainPanel.Controls.Add(dl);
                }
            }
            return completed;
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
