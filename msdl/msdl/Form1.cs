using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SharpBits.Base;

/*
 * TODO:
 *      High priority:
 *          build a progress bar column http://stackoverflow.com/questions/4646920/populating-a-datagridview-with-text-and-progressbars
 *          make the download directory configurable.
 *          implement a timer to manage the labels on the buttons and stuff.
 *          implement a possibility to start a job with multiple files.
 *          implement a possibility to add files to a running job
 *          implement a possibility to change job priorities.
 *          implement a possibility to remove jobs.
 *
 *      Low priority:
 *          build a tray status monitor.
 */

namespace msdl
{
    public partial class Form1 : Form
    {
        private BitsManager _downloadManager;
        private ColumnIDs _colIDs;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Downloads"))
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Downloads");

            _downloadManager = new BitsManager();
            _downloadManager.OnJobModified += downloadManager_OnJobModified;
            _downloadManager.OnJobTransferred += downloadManager_OnJobTransferred;
            _downloadManager.EnumJobs(JobOwner.CurrentUser);

            foreach (var job in _downloadManager.Jobs.Values)
            {
                if (job.State == JobState.Transferred)
                    job.Complete();
                else
                    AddJobFlags(job);
            }

            _colIDs = new ColumnIDs(dataGridView1);

            dataGridView1.Rows.AddRange(GetDownloadJobsAsRows().ToArray());
        }

        /// <summary>
        /// Sets the necessary flags on a BitsJob to fire the events we want.
        /// </summary>
        /// <param name="job">The BitsJob whom to apply the flags.</param>
        private static void AddJobFlags(BitsJob job)
        {
            job.NotificationFlags = NotificationFlags.JobModified | NotificationFlags.JobTransferred |
                                    NotificationFlags.JobErrorOccured;
        }

        private void downloadManager_OnJobTransferred(object sender, NotificationEventArgs e)
        {
            e.Job.Complete();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (e.Job.JobId.ToString() == row.Cells[_colIDs.ID].Value.ToString())
                    row.Cells[_colIDs.Status].Value = e.Job.State.ToString();
            }
            Console.WriteLine(string.Format("Job {0} was transferred.", e.Job.JobId));
        }


        public void downloadManager_OnJobModified(object sender, NotificationEventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (e.Job.JobId.ToString() == row.Cells[_colIDs.ID].Value.ToString())
                {
                    row.Cells[_colIDs.Progress].Value = String.Format("{0}/{1}", e.Job.Progress.BytesTransferred, e.Job.Progress.BytesTotal);
                    row.Cells[_colIDs.Status].Value = e.Job.State.ToString();
                }
            }

            Console.WriteLine("Job {0} was modified.", e.Job.JobId);
        }

        private IEnumerable<DataGridViewRow> GetDownloadJobsAsRows()
        {
            return _downloadManager.Jobs.Values.Select(MakeRow);
        }

        private DataGridViewRow MakeRow(BitsJob job)
        {
            var row = new DataGridViewRow();
            row.CreateCells(dataGridView1);
            row.Cells[_colIDs.FileName].Value = job.JobId; // TODO: change this to get the filename rather then the download ID
            row.Cells[_colIDs.Button].Value = getButtonText(job);
            if (row.Cells[_colIDs.Button].Value.ToString() == "Done")
                row.Cells[_colIDs.Button].ReadOnly = true;
            row.Cells[_colIDs.Progress].Value = String.Format("{0}/{1}", job.Progress.BytesTransferred, job.Progress.BytesTotal);
            row.Cells[_colIDs.Status].Value = job.State.ToString();
            row.Cells[_colIDs.ID].Value = job.JobId.ToString();
            return row;
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
            var dl = new NewDownload();
            if (dl.ShowDialog() == DialogResult.OK)
            {
                var job = _downloadManager.CreateJob("File Download", JobType.Download);
                job.AddFile(dl.DownloadUrl, MakeDownloadPath(dl.DownloadUrl));
                AddJobFlags(job);
                job.Resume();
                dataGridView1.Rows.Add(MakeRow(job));
            }
        }

        private static string MakeDownloadPath(string url)
        {
            var fileName = "";
            fileName += Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Downloads\\";
            fileName += url.Substring(url.LastIndexOf("/"));
            return fileName;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            switch (e.ColumnIndex)
            {
                case 1: // aka button, do this properly!!!
                    foreach (var job in _downloadManager.Jobs.Values)
                    {
                        if (dataGridView1.Rows[e.RowIndex].Cells[_colIDs.ID].Value.ToString() == job.JobId.ToString())
                            ManageJobState(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), job);
                    }
                    break;
            }
        }

        private static void ManageJobState(string button, BitsJob job)
        {
            switch (button)
            {
                case "Resume":
                    job.Resume();
                    break;
                case "Cancel":
                    job.Cancel();
                    break;
            }
        }
    }
}
