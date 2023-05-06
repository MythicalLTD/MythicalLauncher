using DiscordRPC;
using DiscordRPC.Logging;
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
        /* Core Stuff */
        public static string appname;
        public static string applogo;
        public static string applang;
        public static string version;
        public static string appbg;
        public static string appcolour;
        public static string appweb;
        public static string appstore;
        public static string appvote;
        /* DISCORD RPC STRINGS */
        public static string appdiscord;
        private static string enable_discordrpc;
        private static string discord_id;
        private static string discordrpc_button1_text;
        private static string discordrpc_button1_url;
        private static string discordrpc_button2_text;
        private static string discordrpc_button2_url;
        private static string discordrpc_description;
        /* AUTO JOINER */
        public static string enable_auto_joiner;
        public static string auto_joiner_ip;

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
            Console.WriteLine("[{0:HH:mm:ss}] [DISCORD RPC] Please wait while we load RPC", DateTime.Now);
            client = new DiscordRpcClient(discord_id);

            client.Logger = new ConsoleLogger() { Level = LogLevel.Warning };

            client.OnReady += (sender, e) =>
            {
                Console.WriteLine("[{0:HH:mm:ss}] [DISCORD RPC] Received Ready from user {0}", e.User.Username, DateTime.Now);
            };

            client.OnPresenceUpdate += (sender, e) =>
            {
                Console.WriteLine("[{0:HH:mm:ss}] [DISCORD RPC] Received Update! {0}", e.Presence, DateTime.Now);
            };

            client.Initialize();
            DiscordRPC.Button btns = new DiscordRPC.Button();
            DiscordRPC.Button btns2 = new DiscordRPC.Button();
            btns.Label = discordrpc_button1_text;
            btns.Url = discordrpc_button1_url;
            btns2.Label = discordrpc_button2_text;
            btns2.Url = discordrpc_button2_url;

            client.SetPresence(new RichPresence()
            {
                Details = "Playing on " + appname,
                State = discordrpc_description,
                Assets = new Assets()
                {
                    LargeImageKey = "logo",
                    LargeImageText = appname,
                },
                Buttons = new DiscordRPC.Button[]
                {
                    btns,
                    btns2
                },
            });

            Console.WriteLine("[{0:HH:mm:ss}] Discord RPC was successfully loaded", DateTime.Now);

        }

        private void lblexit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void GetSettings()
        {
            Console.WriteLine("[{0:HH:mm:ss}] [SETTINGS] We are trying to load the server settings please wait", DateTime.Now);
            var appcfg = new ConfigParser(appConfig);
            var r_key = appcfg.GetValue("RemoteLauncher", "key");
            if (r_key == "")
            {
                Console.WriteLine("[{0:HH:mm:ss}] [SETTINGS] There was a problem while connecting", DateTime.Now);
                Console.WriteLine("[{0:HH:mm:ss}] [SETTINGS] No server was specified, please make sure to specify it into the config file", DateTime.Now);
                Alert("Please specify a server for the launcher to connect", FrmAlert.enmType.Error);
            }
            else
            {
                string jsonFilePath = r_key + "/api/mythicallauncher/settings/getconfig.php";
                using (var client = new WebClient())
                {
                    Console.WriteLine("[{0:HH:mm:ss}] [SETTINGS] We are downloading the server side settings", DateTime.Now);
                    string json = client.DownloadString(jsonFilePath);
                    dynamic data = JsonConvert.DeserializeObject(json);
                    Console.WriteLine("[{0:HH:mm:ss}] [SETTINGS] Done downloading now please wait while we save the server settings on your local pc", DateTime.Now);
                    version = data.Version;
                    appname = data.appName;
                    applogo = data.appLogo;
                    applang = data.appLang;
                    appbg = data.appBg;
                    appcolour = data.appMainColour;
                    appdiscord = data.appDiscord;
                    appvote = data.appVote;
                    appweb = data.appWebsite;
                    appstore = data.appStore;
                    enable_discordrpc = data.enable_discordrpc;
                    discord_id = data.discord_id;
                    discordrpc_button1_text = data.discordrpc_button1_text;
                    discordrpc_button1_url = data.discordrpc_button1_url;
                    discordrpc_button2_text = data.discordrpc_button2_text;
                    discordrpc_button2_url = data.discordrpc_button2_url;
                    discordrpc_description = data.discordrpc_description;
                    enable_auto_joiner = data.enable_auto_joiner;
                    auto_joiner_ip = data.auto_joiner_ip;
                    lblver.Text = "V" + version;
                    loader.Color = ColorTranslator.FromHtml(appcolour);
                    Console.WriteLine("[{0:HH:mm:ss}] [SETTINGS] Done", DateTime.Now);
                }
            }
        }
        private Boolean checkUpdate()
        {
            Console.WriteLine("[{0:HH:mm:ss}] [UPDATER] Please wait while we check for updates", DateTime.Now);
            Boolean versionStatu;
            try
            {
                WebClient client = new WebClient();
                var appcfg = new ConfigParser(appConfig);
                var r_key = appcfg.GetValue("RemoteLauncher", "key");
                Stream str = client.OpenRead(r_key + "/api/mythicallauncher/settings/update.php?version=" + Program.version);
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
                Console.WriteLine("[{0:HH:mm:ss}] [UPDATER] Update found!", DateTime.Now);
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
            Console.WriteLine("[{0:HH:mm:ss}] [PRELOADER] Please wait while we load all plugins", DateTime.Now);
            Console.WriteLine("[{0:HH:mm:ss}] [PRELOADER] Loading updater..", DateTime.Now);
            askUpdate();
            Console.WriteLine("[{0:HH:mm:ss}] [PRELOADER] Done..", DateTime.Now);
            Console.WriteLine("[{0:HH:mm:ss}] [PRELOADER] Loading settings..", DateTime.Now);
            GetSettings();
            Console.WriteLine("[{0:HH:mm:ss}] [PRELOADER] Done..", DateTime.Now);
            Console.WriteLine("[{0:HH:mm:ss}] [PRELOADER] Loading Discord RPC..", DateTime.Now);
            if (enable_discordrpc == "true")
            {
                InitializeRPC();
            }
            else
            {
                Console.WriteLine("[{0:HH:mm:ss}] [PRELOADER] We disabled Discord RPC..", DateTime.Now);
            }
            Console.WriteLine("[{0:HH:mm:ss}] [PRELOADER] Please wait while we display the server imagine", DateTime.Now);
            DisplayImage();
            Console.WriteLine("[{0:HH:mm:ss}] [PRELOADER] Done..", DateTime.Now);
            lblver.Text = "V" + File.ReadAllText(versionfile);
            await Task.Delay(5000);
            Console.WriteLine("[{0:HH:mm:ss}] [PRELOADER] Loading login form", DateTime.Now);
            FrmLogin x = new FrmLogin();
            x.Show();
            Console.WriteLine("[{0:HH:mm:ss}] [PRELOADER] Done..", DateTime.Now);
            this.Hide();

        }
    }
}
