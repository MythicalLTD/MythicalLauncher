using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MythicalLauncher
{
    public partial class FrmUpdate : Form
    {
        string appPath = Application.StartupPath + "\\example3.rar";
        public FrmUpdate()
        {
            InitializeComponent();
        }

        private void ProgressChanged(object sneder, DownloadProgressChangedEventArgs e) { 
          progressBar1.Value = e.ProgressPercentage;
          label2.Text = "Downloading...   " + BytesToString(e.BytesReceived) + " / " + BytesToString(e.TotalBytesToReceive);
        }
        private void Completed(object sender, AsyncCompletedEventArgs e) {
            label2.Text = "Done!";
            DialogResult dr = MessageBox.Show("Do you want to install","Done",MessageBoxButtons.YesNo,MessageBoxIcon.Information);
            try { 
                if(dr == DialogResult.Yes){
                    System.Diagnostics.Process.Start(appPath);
                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            WebClient webClient = new WebClient();
            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
            webClient.DownloadFileAsync(new Uri("http://localhost/can/example3.rar"),appPath);
        }

        private static string BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place),1);
            return (Math.Sign(bytes) * num).ToString() + suf[place];
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
