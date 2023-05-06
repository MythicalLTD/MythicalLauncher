using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Downloader;
using Newtonsoft.Json.Linq;
using Salaros.Configuration;
using System;
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


        private void FrmMain_Load(object sender, EventArgs e)
        {
            string avatarUrl = $"https://minotar.net/avatar/{FrmLogin.l_username}";

            using (WebClient client = new WebClient())
            {
                byte[] imageBytes = client.DownloadData(avatarUrl);
                mcpicture.Image = System.Drawing.Image.FromStream(new System.IO.MemoryStream(imageBytes));
            }
            if (Program.debugmdoe == "true")
            {
                lblver.Text = "Version: " + File.ReadAllText(versionfile) + " (DEBUG MODE)";
            }
            else
            {
                lblver.Text = "Version: " + File.ReadAllText(versionfile);
            }
            this.WindowState = FormWindowState.Normal;
            label1.Text = "Welcome, " + FrmLogin.l_username;
            lblbuilnumber.Text = "Build number: " + File.ReadAllText(buildfile);
            RememberMe();
            DisplayImage();
            LoadSettings();
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
        private void DisplayImage()
        {
            var appcfg = new ConfigParser(appConfig);
            var r_key = appcfg.GetValue("RemoteLauncher", "key");
            string jsonFilePath = r_key + "/api/mythicallauncher/settings/getconfig.php";
            JObject data = GetDataFromUrl(jsonFilePath);
            string imageUrl = (string)data["appLogo"];
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
            string bgimageURL = (string)data["appBg"];
            using (WebClient backWebClient = new WebClient())
            {
                byte[] bgimageBytes = backWebClient.DownloadData(bgimageURL);
                using (MemoryStream bgmemoryStream = new MemoryStream(bgimageBytes))
                {
                    this.BackgroundImage = Image.FromStream(bgmemoryStream);
                }
            }
            string appcolour = (string)data["appMainColour"];
            btnplay.FillColor = ColorTranslator.FromHtml(appcolour);
            btnplay.FillColor2 = ColorTranslator.FromHtml(appcolour);
        }
        void LoadSettings()
        {
            lbllaunchername.Text = FrmLoading.version + " | Home";
            lblver.Text = "Version: " + FrmLoading.version;

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

                };

                if (FrmLoading.enable_auto_joiner == "true")
                {
                    launchOption.ServerIp = FrmLoading.auto_joiner_ip;
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


        private void label10_Click(object sender, EventArgs e)
        {
            FrmChangeLog x = new FrmChangeLog();
            x.Show();
            this.Hide();
        }
    }
}
