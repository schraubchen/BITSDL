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

            tbFileName.Text = DownloadJob.JobId.ToString();
            tbStatus.Text = DownloadJob.State.ToString();

            ManageProgressBar();
            ManageButtonState();
        }

        void DownloadJob_OnJobModified(object sender, JobNotificationEventArgs e)
        {
            ManageProgressBar();
            ManageButtonState();
            tbStatus.Text = DownloadJob.State.ToString();
        }

        private void ManageButtonState()
        {
            if (DownloadJob.State == JobState.Transferring)
                btnPauseResume.Text = "Pause";
            else if( DownloadJob.State == JobState.Suspended)
                btnPauseResume.Text = "Resume";
        }

        private void ManageProgressBar()
        {
            if (DownloadJob.Progress.BytesTotal != 0)
                progressBar1.Value = (int)((100 / DownloadJob.Progress.BytesTotal) * DownloadJob.Progress.BytesTransferred);
            else
                progressBar1.Value = 0;
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
