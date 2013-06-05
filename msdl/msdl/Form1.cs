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

/*
 * TODO:
 *      High priority:
 *          implement the events properly!
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
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Downloads"))
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Downloads");

            downloadManager = new BitsManager();
            downloadManager.EnumJobs(JobOwner.CurrentUser);

            foreach (var job in downloadManager.Jobs.Values)
            {
                job.OnJobModified += job_OnJobModified;
                job.OnJobTransferred += job_OnJobTransferred;
            }

            dataGridView1.Rows.AddRange(getDownloadJobsAsRows().ToArray());
        }

        void job_OnJobTransferred(object sender, JobNotificationEventArgs e)
        {
            Console.WriteLine("Some job was transfered.");
        }

        void job_OnJobModified(object sender, JobNotificationEventArgs e)
        {
            Console.WriteLine("Some job was modified.");
        }

        private IEnumerable<DataGridViewRow> getDownloadJobsAsRows()
        {
            foreach (var job in downloadManager.Jobs.Values)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridView1);
                row.Cells[0].Value = job.JobId;
                row.Cells[1].Value = getButtonText(job);
                if (row.Cells[1].Value.ToString() == "Done")
                    row.Cells[1].ReadOnly = true;
                row.Cells[2].Value = String.Format("{0}/{1}", job.Progress.BytesTransferred, job.Progress.BytesTotal);
                row.Cells[3].Value = job.State.ToString();
                yield return row;
            }
        }

        private string getButtonText(BitsJob job)
        {
            switch (job.State)
            {
                case JobState.Suspended:
                    return "Resume";
                case JobState.Transferred:
                case JobState.Acknowledged:
                    return "Done";
                default:
                    return "Cancel";
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
            }
        }

        private string makeDownloadPath(string url)
        {
            string fileName = "";
            fileName += Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Downloads\\";
            fileName += url.Substring(url.LastIndexOf("/"));
            return fileName;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1) // aka button, do this properly!!!
            {

            }
        }
    }
}
