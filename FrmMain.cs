using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Downloader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Salaros.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MythicalLauncher
{
    public partial class FrmMain : Form
    {
        private string appConfig = Application.StartupPath + @"\settings.ini";
        private string buildfile = Application.StartupPath + @"\buildnumber";
        private string versionfile = Application.StartupPath + @"\version";
        List<Announcement> announcements = new List<Announcement>();

        public FrmMain(MSession session)
        {
            this.session = session;
            InitializeComponent();
        }
        CMLauncher launcher;
        readonly MSession session;
        MinecraftPath gamePath;
        bool useMJava = true;
        string javaPath = "java.exe";
        private int uiThreadId = Thread.CurrentThread.ManagedThreadId;


        void Alert(string msg, FrmAlert.enmType type)
        {
            FrmAlert frm = new FrmAlert();
            frm.showAlert(msg, type);
        }
        void RememberMe()
        {
            var cfg = new ConfigParser(appConfig);
            var Xms = cfg.GetValue("RAM", "Xms");
            var Xmx = cfg.GetValue("RAM", "Xmx");
            if (Xms == "" || Xmx == "")
            {
                Alert("Please set your Desired Ram Settings!", FrmAlert.enmType.Info);
            }
            else
            {
                txtXms.Text = Xms;
                TxtXmx.Text = Xmx;
            }
        }

        void getAvatar()
        {
            string avatarUrl = $"https://minotar.net/avatar/{FrmLogin.l_username}";

            using (WebClient client = new WebClient())
            {
                byte[] imageBytes = client.DownloadData(avatarUrl);
                mcpicture.Image = System.Drawing.Image.FromStream(new System.IO.MemoryStream(imageBytes));
            }
        }
        private void FrmMain_Load(object sender, EventArgs e)
        {
            LoadAnnouncements();
            loadAnnouncements();
            if (Program.debugmode == "true")
            {
                lblver.Text = "Version: " + File.ReadAllText(versionfile) + " (DEBUG MODE)";
            }
            else
            {
                lblver.Text = "Version: " + File.ReadAllText(versionfile);
            }
            getAvatar();
            this.WindowState = FormWindowState.Normal;
            label1.Text = "Welcome, " + FrmLogin.l_username;
            lblbuilnumber.Text = "Build number: " + File.ReadAllText(buildfile);
            RememberMe();
            DisplayImage();
            LoadSettings();
        }
        void LoadAnnouncements()
        {
            listBox1.ClearSelected();
            listBox1.DataSource = announcements;
            listBox1.DisplayMember = "Title";


        }
        void loadAnnouncements()
        {
            try
            {
                var cfg = new ConfigParser(appConfig);
                var r_key = cfg.GetValue("RemoteLauncher", "key");
                string url = r_key + "/api/mythicallauncher/announcements.php";
                using (WebClient client = new WebClient())
                {
                    string json = client.DownloadString(url);
                    List<Announcement> announcements = JsonConvert.DeserializeObject<List<Announcement>>(json);
                    listBox1.DataSource = announcements;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Console.WriteLine("[{0:HH:mm:ss}] [Announcements] "+ex.Message);
            }
            
        }
        public class Announcement
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime Date { get; set; }
            public override string ToString()
            {
                return Title;
            }
        }       

        private void DisplayImage()
        {
            using (WebClient webClient = new WebClient())
            {
                byte[] imageBytes = webClient.DownloadData(FrmLoading.applogo);
                using (MemoryStream memoryStream = new MemoryStream(imageBytes))
                {
                    pictureBox1.Image = Image.FromStream(memoryStream);
                }
            }
            using (WebClient webClient = new WebClient())
            {
                byte[] iconBytes = webClient.DownloadData(FrmLoading.applogo);
                using (MemoryStream ms = new MemoryStream(iconBytes))
                {
                    Bitmap bitmap = (Bitmap)Image.FromStream(ms);
                    this.Icon = Icon.FromHandle(bitmap.GetHicon());
                }
            }
            using (WebClient backWebClient = new WebClient())
            {
                byte[] bgimageBytes = backWebClient.DownloadData(FrmLoading.appbg);
                using (MemoryStream bgmemoryStream = new MemoryStream(bgimageBytes))
                {
                    this.BackgroundImage = Image.FromStream(bgmemoryStream);
                }
            }
            btnplay.FillColor = ColorTranslator.FromHtml(FrmLoading.appcolour);
            btnplay.FillColor2 = ColorTranslator.FromHtml(FrmLoading.appcolour);
        }
        void LoadSettings()
        {
            var cfg = new ConfigParser(appConfig);
            lbllaunchername.Text = FrmLoading.appname + " | Home";
            lblver.Text = "Version: " + FrmLoading.version;
            var SkipAssets = cfg.GetValue("MAIN", "SkipAssets");
            var SkipHashCheck = cfg.GetValue("MAIN", "SkipHashCheck");
            var ParallelDownload = cfg.GetValue("MAIN", "ParallelDownload");
            var LockTOP = cfg.GetValue("MAIN", "LockTOP");
            var enable_rpc = cfg.GetValue("MAIN", "enable_rpc");
            var fullscreen = cfg.GetValue("MAIN", "full-screen");
            if (SkipAssets == "true")
            {
                cbSkipAssets.Checked = true;
            }
            else if (SkipAssets == "false")
            {
                cbSkipAssets.Checked = false;
            }
            if (SkipHashCheck == "true")
            {
                cbSkipHashCheck.Checked = true;
            }
            else if (SkipHashCheck == "false")
            {
                cbSkipHashCheck.Checked = false;
            }
            if (ParallelDownload == "true")
            {
                rbParallelDownload.Checked = true;
            }
            else if (ParallelDownload == "false")
            {
                rbParallelDownload.Checked = false;
            }
            if (LockTOP == "true")
            {
                guna2CheckBox2.Checked = true;
                this.TopMost = true;
            }
            else if (LockTOP == "false")
            {
                guna2CheckBox2.Checked = false;
                this.TopMost = false;
            }
            if (enable_rpc == "true")
            {
                guna2CheckBox2.Checked = true;
            }
            else if (enable_rpc == "false") 
            {
                guna2CheckBox1.Checked = false;
            }
            if (fullscreen == "true")
            {
                cbFullscreen.Checked = true;
            }
            else if (fullscreen == "false")
            {
                cbFullscreen.Checked = false;
            }

        }
        private async void FrmMain_Shown(object sender, EventArgs e)
        {
            this.Refresh();
            var defaultPath = new MinecraftPath(MinecraftPath.GetOSDefaultPath());
            await initializeLauncher(defaultPath);
        }
        private async Task initializeLauncher(MinecraftPath path)
        {
            this.gamePath = path;
            launcher = new CMLauncher(path);
            launcher.FileChanged += Launcher_FileChanged;
            launcher.ProgressChanged += Launcher_ProgressChanged;
            await refreshVersions(null);
        }
        private void Launcher_FileChanged(DownloadFileChangedEventArgs e)
        {
            if (Thread.CurrentThread.ManagedThreadId != uiThreadId)
            {
                Debug.WriteLine(e);
            }
            Pb_Progress.Maximum = e.TotalFileCount;
            Pb_Progress.Value = e.ProgressedFileCount;
            Lv_Status.Text = $"{e.FileKind} : {e.FileName} ({e.ProgressedFileCount}/{e.TotalFileCount})";
        }
        private void Launcher_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (Thread.CurrentThread.ManagedThreadId != uiThreadId)
            {
                Debug.WriteLine(e);
            }
            Pb_Progress.Maximum = 100;
            Pb_Progress.Value = e.ProgressPercentage;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (session == null)
            {
                Alert("Oops, we could not verify your session.", FrmAlert.enmType.Warning);
                return;
            }

            if (cbVersion.Text == "")
            {
                Alert("You need to select a version first.", FrmAlert.enmType.Warning);
                return;
            }

            var cfg = new ConfigParser(appConfig);
            var Xms = cfg.GetValue("RAM", "Xms");
            var Xmx = cfg.GetValue("RAM", "Xmx");
            if (Xms == "" || Xmx == "")
            {
                Alert("Please set your Desired Ram Settings!", FrmAlert.enmType.Info);
                return;
            }

            try
            {

                var launchOption = new MLaunchOption()
                {
                    MaximumRamMb = int.Parse(TxtXmx.Text),
                    Session = this.session,
                    GameLauncherName = FrmLoading.appname,
                    FullScreen = cbFullscreen.Checked

                };

                if (FrmLoading.enable_auto_joiner == "true")
                {
                    launchOption.ServerIp = FrmLoading.auto_joiner_ip;
                    launchOption.ServerPort = FrmLoading.auto_joiner_port;
                }

                if (!useMJava)
                    launchOption.JavaPath = javaPath;

                if (!string.IsNullOrEmpty(txtXms.Text))
                    launchOption.MinimumRamMb = int.Parse(txtXms.Text);

                if (rbParallelDownload.Checked)
                {
                    System.Net.ServicePointManager.DefaultConnectionLimit = 256;
                    launcher.FileDownloader = new AsyncParallelDownloader();
                }
                else
                    launcher.FileDownloader = new SequenceDownloader();

                launcher.GameFileCheckers.AssetFileChecker.CheckHash = cbSkipHashCheck.Checked;
                launcher.GameFileCheckers.ClientFileChecker.CheckHash = cbSkipHashCheck.Checked;
                launcher.GameFileCheckers.LibraryFileChecker.CheckHash = cbSkipHashCheck.Checked;

                if (cbSkipAssets.Checked)
                    launcher.GameFileCheckers.AssetFileChecker = null;

                var process = await launcher.CreateProcessAsync(cbVersion.Text, launchOption);
                StartProcess(process);
            }
            catch (FormatException fex)
            {
                MessageBox.Show("Failed to create Launch Options\n\n" + fex);
            }
            catch (MDownloadFileException mex)
            {
                MessageBox.Show(
                    $"FileName : {mex.ExceptionFile.Name}\n" +
                    $"FilePath : {mex.ExceptionFile.Path}\n" +
                    $"FileUrl : {mex.ExceptionFile.Url}\n" +
                    $"FileType : {mex.ExceptionFile.Type}\n\n" +
                    mex.ToString());
            }
            catch (Win32Exception wex)
            {
                Alert(wex + "Oops, we found a problem in your Java.", FrmAlert.enmType.Error);
            }
            catch (Exception ex)
            {
                Alert(ex + "", FrmAlert.enmType.Error);
            }
            finally
            {
                this.WindowState = FormWindowState.Minimized;
            }

        }

        private void StartProcess(Process process)
        {
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.EnableRaisingEvents = true;

            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
        }


        private void btnSetLastVersion_Click(object sender, EventArgs e)
        {
            cbVersion.Text = launcher.Versions.LatestReleaseVersion?.Name;
        }

        private async Task refreshVersions(string showVersion)
        {
            cbVersion.Items.Clear();

            var versions = await launcher.GetAllVersionsAsync();

            bool showVersionExist = false;
            foreach (var item in versions)
            {
                if (showVersion != null && item.Name == showVersion)
                    showVersionExist = true;
                cbVersion.Items.Add(item.Name);
            }

            if (showVersion == null || !showVersionExist)
                btnSetLastVersion_Click(null, null);
            else
                cbVersion.Text = showVersion;
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void txtXms_TextChanged(object sender, EventArgs e)
        {
            var cfg = new ConfigParser(appConfig);
            cfg.SetValue("RAM", "Xms", txtXms.Text);
            cfg.Save();
        }

        private void TxtXmx_TextChanged(object sender, EventArgs e)
        {
            var cfg = new ConfigParser(appConfig);
            cfg.SetValue("RAM", "Xmx", TxtXmx.Text);
            cfg.Save();
        }

        private void bunifuButton25_Click(object sender, EventArgs e)
        {
            Misc.SetPage(SettingsPage);
        }

        private void bunifuButton26_Click(object sender, EventArgs e)
        {
            Misc.SetPage(HomePage);
        }

        private async void pictureBox5_Click(object sender, EventArgs e)
        {
            await refreshVersions(null);
        }

        private async void label4_Click(object sender, EventArgs e)
        {
            await refreshVersions(null);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                Announcement selectedAnnouncement = (Announcement)listBox1.SelectedItem;
                announcementDescriptionTextBox.Text = selectedAnnouncement.Description;
            }
        }

        private void cbSkipAssets_Click(object sender, EventArgs e)
        {
            var cfg = new ConfigParser(appConfig);
            if (cbSkipAssets.Checked == true)
            {
                cfg.SetValue("MAIN", "SkipAssets", "true");
            }
            else if (cbSkipAssets.Checked == false)
            {
                cfg.SetValue("MAIN", "SkipAssets", "false");
            }
            cfg.Save();
        }

        private void cbSkipHashCheck_Click(object sender, EventArgs e)
        {
            var cfg = new ConfigParser(appConfig);

            if (cbSkipHashCheck.Checked == true)
            {
                cfg.SetValue("MAIN", "SkipHashCheck", "true");
            }
            else if (cbSkipHashCheck.Checked == false)
            {
                cfg.SetValue("MAIN", "SkipHashCheck", "false");
            }
            cfg.Save();
        }

        private void rbParallelDownload_Click(object sender, EventArgs e)
        {
            var cfg = new ConfigParser(appConfig);
            if (rbParallelDownload.Checked == true)
            {
                cfg.SetValue("MAIN", "ParallelDownload", "true");
            }
            else if(rbParallelDownload.Checked == false)
            {
                cfg.SetValue("MAIN", "ParallelDownload", "false");
            }
            cfg.Save();
        }

        private void guna2CheckBox2_Click(object sender, EventArgs e)
        {
            var cfg = new ConfigParser(appConfig);
            if(guna2CheckBox2.Checked == true)
            {
                cfg.SetValue("MAIN", "LockTOP", "true");
                this.TopMost = true;
            }
            else if (guna2CheckBox2.Checked == false)
            {
                cfg.SetValue("MAIN", "LockTOP", "false");
                this.TopMost = false;
            }
            cfg.Save();
        }

        private void guna2CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            var cfg = new ConfigParser(appConfig);
            if (guna2CheckBox1.Checked == true)
            {
                cfg.SetValue("MAIN", "enable_rpc", "true");
                Alert("Please restart the app to apply this change", FrmAlert.enmType.Warning);
            }
            else if (guna2CheckBox1.Checked == false)
            {
                cfg.SetValue("MAIN", "enable_rpc", "false");
                Alert("Please restart the app to apply this change", FrmAlert.enmType.Warning);
            }
            cfg.Save();
        }

        private void cbFullscreen_CheckedChanged(object sender, EventArgs e)
        {
            var cfg = new ConfigParser(appConfig);
            if (cbFullscreen.Checked == true)
            {
                cfg.SetValue("MAIN", "full-screen","true");
            }
            else if (cbFullscreen.Checked == false)
            {
                cfg.SetValue("MAIN", "full-screen", "false");
            }
            cfg.Save();
        }
    }
}
