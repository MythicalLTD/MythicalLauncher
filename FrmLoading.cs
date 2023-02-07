using DiscordRPC;
using DiscordRPC.Logging;
using Guna.UI2.WinForms.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Salaros.Configuration;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MythicalLauncher
{
    public partial class FrmLoading : Form
    {
        public FrmLoading()
        {
            InitializeComponent();
        }
        public DiscordRpcClient client;
        private string appConfig = Application.StartupPath + @"\settings.ini";
        private string versionfile = Application.StartupPath + @"\version";

        public static string appname;
        public static string applogo;
        public static string applang;
        public static string version;

        public static JObject GetDataFromUrl(string url)
        {
            using (var client = new WebClient())
            {
                string jsonText = client.DownloadString(url);
                JObject jsonObject = JObject.Parse(jsonText);
                return jsonObject;
            }
        }

        void InitializeRPC()
        {
            client = new DiscordRpcClient("1038164770244788254");

            client.Logger = new ConsoleLogger() { Level = LogLevel.Warning };

            client.OnReady += (sender, e) =>
            {
                Console.WriteLine("Received Ready from user {0}", e.User.Username);
            };

            client.OnPresenceUpdate += (sender, e) =>
            {
                Console.WriteLine("Received Update! {0}", e.Presence);
            };

            client.Initialize();

            DiscordRPC.Button btns = new DiscordRPC.Button();
            DiscordRPC.Button btns2 = new DiscordRPC.Button();
            btns.Label = "Github";
            btns.Url = "https://github.com/MythicalLTD/MythicalLauncher";
            btns2.Label = "Free Hosting";
            btns2.Url = "https://my.mythicalnodes.xyz";

            client.SetPresence(new RichPresence()
            {
                Details = "MythicalSystems",
                State = "Free custom minecraft launcher!",
                Assets = new Assets()
                {
                    LargeImageKey = "logo",
                    LargeImageText = "MythicalSystems",
                },
                Buttons = new DiscordRPC.Button[]
                {
                    btns,
                    btns2
                },
            });
        }

        private void lblexit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void GetSettings()
        {
            var appcfg = new ConfigParser(appConfig);
            var r_key = appcfg.GetValue("RemoteLauncher", "key");
            if (r_key == "")
            {
                Alert("Please specify a server for the launcher to connect", FrmAlert.enmType.Error);
            }
            else
            {
                string jsonFilePath = r_key+ "/api/mythicallauncher/settings/getconfig.php";
                using (var client = new WebClient())
                {
                    string json = client.DownloadString(jsonFilePath);
                    dynamic data = JsonConvert.DeserializeObject(json);
                    string version = data.Version;
                    string appname = data.appName;
                    string applogo = data.appLogo;
                    string applang = data.appLang;
                    string appbg = data.appBg;
                    string appcolour = data.appMainColour;
                    string appdiscord = data.appDiscord;
                    string appvote = data.appVote;
                    string appweb = data.appWebsite;
                    string appstore = data.appStore;
                    lblver.Text = "V" +version;
                    appcfg.SetValue("RemoteLauncher", "Version", version);
                    appcfg.SetValue("RemoteLauncher", "appName", appname);
                    appcfg.SetValue("RemoteLauncher", "appLogo", applogo);
                    appcfg.SetValue("RemoteLauncher", "appBg", appbg);
                    appcfg.SetValue("RemoteLauncher", "appMainColour", appcolour);
                    appcfg.SetValue("RemoteLauncher", "appDiscord", appdiscord);
                    appcfg.SetValue("RemoteLauncher", "appWebsite", appweb);
                    appcfg.SetValue("RemoteLauncher", "appVote", appvote);
                    appcfg.SetValue("RemoteLauncher", "appStore", appstore);
                    appcfg.SetValue("RemoteLauncher", "appLang", applang);
                    appcfg.Save();
                    loader.Color = ColorTranslator.FromHtml(appcolour);
                }
            }
        }
        private Boolean checkUpdate()
        {
            Boolean versionStatu;
            try
            {
                WebClient client = new WebClient();
                var appcfg = new ConfigParser(appConfig);
                var r_key = appcfg.GetValue("RemoteLauncher", "key");
                Stream str = client.OpenRead(r_key+ "/api/mythicallauncher/settings/update.php?version=" + Program.version);
                StreamReader reader = new StreamReader(str);
                String content = reader.ReadToEnd();
                versionStatu = (content == "GETUPDATE") ? versionStatu = true : versionStatu = false;
            }
            catch { versionStatu = false; }
            return versionStatu;

        }
        private void askUpdate()
        {

            if (checkUpdate())
            {
                DialogResult dr = MessageBox.Show("New update is available. \n\r Would you like to install it now?", "Update found", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (dr == DialogResult.Yes)
                {
                    System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(getUpdate));
                    thread.Start();
                    this.Close();
                }
                if (dr == DialogResult.No)
                {
                    Application.Exit();
                }
            }
        }

        private void getUpdate()
        {
            Application.Run(new FrmUpdate());
        }   
        void Alert(string msg, FrmAlert.enmType type)
        {
            FrmAlert frm = new FrmAlert();
            frm.showAlert(msg, type);
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
                    logo.Image = Image.FromStream(memoryStream);
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
        }
        private async void FrmLoading_Load(object sender, EventArgs e)
        {
            askUpdate();
            GetSettings();
            InitializeRPC();
            DisplayImage();
            lblver.Text = "V"+File.ReadAllText(versionfile);
            await Task.Delay(5000);
            FrmLogin x = new FrmLogin();
            x.Show();
            this.Hide();

        }
    }
}
