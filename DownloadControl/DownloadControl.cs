using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SharpBits.Base;

namespace DownloadCtrl
{
    public partial class DownloadControl: UserControl
    {
        public BitsJob DownloadJob;

        public DownloadControl(BitsJob job)
        {
            InitializeComponent();

            DownloadJob = job;
            DownloadJob.OnJobModified += DownloadJob_OnJobModified;
            DownloadJob.OnJobTransferred += DownloadJob_OnJobTransferred;

            tbFileName.Text = DownloadJob.JobId.ToString();
            tbStatus.Text = DownloadJob.State.ToString();

            ManageProgressBar();
            ManageButtonState();
        }

        void DownloadJob_OnJobModified(object sender, JobNotificationEventArgs e)
        {
            Console.WriteLine(string.Format("Job modified! ID:{0}", DownloadJob.JobId));

            ManageProgressBar();
            ManageButtonState();
            tbStatus.Text = DownloadJob.State.ToString();

            if (DownloadJob.State == JobState.Transferred)
                DownloadJob.Complete();

            this.Invalidate();
            this.Update();
        }

        void DownloadJob_OnJobTransferred(object sender, JobNotificationEventArgs e)
        {
            Console.WriteLine(string.Format("Job Transferred! ID:{0}", DownloadJob.JobId));
            ManageButtonState();
            ManageProgressBar();
            DownloadJob.Complete();
        }

        private void ManageButtonState()
        {
            switch (DownloadJob.State)
            {
                case JobState.Suspended:
                    btnPauseResume.Text = "Resume";
                    break;
                case JobState.Error:
                    btnPauseResume.Enabled = false;
                    btnPauseResume.Text = "Error!";
                    break;
                case JobState.Transferred:
                    btnPauseResume.Enabled = false;
                    break;
                default:
                    btnPauseResume.Text = "Pause";
                    break;
            }
        }

        private void ManageProgressBar()
        {
            if (DownloadJob.Progress.BytesTotal != 0)
            {
                float temp = 100 / DownloadJob.Progress.BytesTotal;
                float temp2 = temp / DownloadJob.Progress.BytesTransferred;
                progressBar1.Value = Convert.ToInt32(temp2);
            }
            else
            {
                progressBar1.Value = 0;
            }
        }

        private void btnPauseResume_Click(object sender, EventArgs e)
        {
            if (DownloadJob.State == JobState.Transferring)
                DownloadJob.Suspend();
            else if (DownloadJob.State == JobState.Suspended)
                DownloadJob.Resume();

            ManageButtonState();
        }
    }
}
