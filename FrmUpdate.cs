using Guna.UI2.WinForms.Suite;
using Newtonsoft.Json.Linq;
using Salaros.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MythicalLauncher
{
    public partial class FrmUpdate : Form
    {
        string appPath = Application.StartupPath + "\\MythicalLauncher.exe";
        private string appConfig = Application.StartupPath + @"\settings.ini";

        public FrmUpdate()
        {
            InitializeComponent();
        }

        private void ProgressChanged(object sneder, DownloadProgressChangedEventArgs e) { 
          progressBar1.Value = e.ProgressPercentage;
          label2.Text = "Downloading...   " + BytesToString(e.BytesReceived) + " / " + BytesToString(e.TotalBytesToReceive);
        }
        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            label2.Text = "Done!";
            DialogResult dr = MessageBox.Show("Do you want to install this update?", "Done", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            try
            {
                if (dr == DialogResult.Yes)
                {
                    string tempPath = Path.GetTempPath(); 
                    string tempFilePath = Path.Combine(tempPath, "MythicalLauncher.exe"); 
                    WebClient webClient = new WebClient();
                    webClient.DownloadFile(new Uri("https://github.com/MythicalLTD/MythicalLauncher/releases/latest/download/MythicalLauncher.exe"), tempFilePath);

                    string batchScriptPath = Path.Combine(Application.StartupPath, "update.bat");
                    File.WriteAllText(batchScriptPath, "@echo off\r\n" +
                                                    "timeout /t 2 >nul\r\n" +
                                                    "taskkill /im MythicalLauncher.exe /f\r\n" + 
                                                    "del \"" + appPath + "\"\r\n" + 
                                                    "move /y \"" + tempFilePath + "\" \"" + appPath + "\"\r\n" + 
                                                    "start \"\" \"" + appPath + "\"\r\n" + 
                                                    "del \"%~f0\""); 
                    System.Diagnostics.Process.Start(batchScriptPath);
                    Application.Exit();
                }
                else if (dr == DialogResult.No)
                {
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
            webClient.DownloadFileAsync(new Uri("https://github.com/MythicalLTD/MythicalLauncher/releases/latest/download/MythicalLauncher.exe"),appPath);
        }

        private static string BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place),1);
            return (Math.Sign(bytes) * num).ToString() + suf[place];
        }
        private void DisplayImage()
        {
            var appcfg = new ConfigParser(appConfig);
            var r_key = appcfg.GetValue("RemoteLauncher", "key");
            string jsonFilePath = r_key + "/api/mythicallauncher/settings/getconfig.php";
            JObject data = GetDataFromUrl(jsonFilePath);
            string imageUrl = (string)data["appLogo"];
            string appname = (string)data["appName"];
            using (WebClient webClient = new WebClient())
            {
                byte[] imageBytes = webClient.DownloadData(imageUrl);
                using (MemoryStream memoryStream = new MemoryStream(imageBytes))
                {
                    pictureBox1.Image = Image.FromStream(memoryStream);
                }
            }
            using (WebClient webClient = new WebClient())
            {
                byte[] iconBytes = webClient.DownloadData(imageUrl);
                using (MemoryStream ms = new MemoryStream(iconBytes))
                {
                    Bitmap bitmap = (Bitmap)Image.FromStream(ms);
                    this.Icon = Icon.FromHandle(bitmap.GetHicon());
                }
            }

            string appcolour = (string)data["appMainColour"];
            lbllaunchername.Text = appname + " | Updater";

        }
        public static JObject GetDataFromUrl(string url)
        {
            using (var client = new WebClient())
            {
                string jsonText = client.DownloadString(url);
                JObject jsonObject = JObject.Parse(jsonText);
                return jsonObject;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void FrmUpdate_Load(object sender, EventArgs e)
        {
            DisplayImage();
        }
    }
}
