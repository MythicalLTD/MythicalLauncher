using CmlLib.Core.Auth;
using DiscordRPC;
using DiscordRPC.Logging;
using Microsoft.VisualBasic.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Salaros.Configuration;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MythicalLauncher
{
    public partial class FrmLogin : Form
    {
        MLogin login = new MLogin();
        public static string username;
        private string appConfig = Application.StartupPath + @"\settings.ini";

        public FrmLogin()
        {
            InitializeComponent();
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

        private void UpdateSession(MSession session)
        {
            var mainForm = new FrmMain(session);
            mainForm.FormClosed += (s, e) => this.Close();
            mainForm.Show();
            this.Close();
            
        }
        void LoadSettings()
        {
            var cfg = new ConfigParser(appConfig);
            var appName = cfg.GetValue("RemoteLauncher", "appName");
            //lbllaunchername.Text = appName + " | Login";
            lbllchname.Text = appName;
        }
        void RememberMe()
        {
            var cfg = new ConfigParser(appConfig);
            var re_email = cfg.GetValue("ACCOUNT", "email");
            var re_pass = cfg.GetValue("ACCOUNT", "pass");
            if (re_email == "" || re_pass == "")
            {
                Alert("Cant get your last account info!",FrmAlert.enmType.Warning);
            }
            else
            {
                email.Text = re_email;
                password.Text = re_pass;
                var r_key = cfg.GetValue("RemoteLauncher", "key");
                string webURL = r_key + "/api/mythicallauncher/login/login.php?email=" + email.Text + "&pass=" + password.Text;
                WebClient wc = new WebClient();
                wc.Headers.Add("user-agent", "Only a Header!");
                byte[] rawByteArray = wc.DownloadData(webURL);
                string webContent = Encoding.UTF8.GetString(rawByteArray);

                if (webContent.ToUpper().Contains("LOGIN_SUCCES"))
                    getUsername();
                   
                else if (webContent.ToUpper().Contains("LOGIN_FAILD"))
                    Alert("Invalid email or password", FrmAlert.enmType.Error);
                else if (webContent.ToUpper().Contains("ERROR_BANNED"))
                    Alert("Your are banned from using our launcher!", FrmAlert.enmType.Warning);
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
            loginbutton.FillColor = ColorTranslator.FromHtml(appcolour);
            loginbutton.FillColor2 = ColorTranslator.FromHtml(appcolour);

        }

        void Alert(string msg, FrmAlert.enmType type)
        {
            FrmAlert frm = new FrmAlert();
            frm.showAlert(msg, type);
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            var cfg = new ConfigParser(appConfig);
            var r_key = cfg.GetValue("RemoteLauncher", "key");
            RememberMe();
            LoadSettings();
            DisplayImage();
            
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void register_Click(object sender, EventArgs e)
        {
            FrmRegister x = new FrmRegister();
            x.Show();
            this.Hide();
        }

        private void registerbutton_Click(object sender, EventArgs e)
        {
            if (email.Text == "" || password.Text == "")
            {
                Alert("Please fill in the textbox", FrmAlert.enmType.Error);
            }
            else
            {
                var cfg = new ConfigParser(appConfig);
                var r_key = cfg.GetValue("RemoteLauncher", "key");
                string webURL = r_key + "/api/mythicallauncher/login/login.php?email=" + email.Text + "&pass=" + password.Text;
                WebClient wc = new WebClient();
                wc.Headers.Add("user-agent", "Only a Header!");
                byte[] rawByteArray = wc.DownloadData(webURL);
                string webContent = Encoding.UTF8.GetString(rawByteArray);

                if (webContent.ToUpper().Contains("LOGIN_SUCCES"))
                    getUsername();
                else if (webContent.ToUpper().Contains("LOGIN_FAILD"))
                    Alert("Invalid email or password", FrmAlert.enmType.Error);
                else if (webContent.ToUpper().Contains("ERROR_BANNED"))
                    Alert("Your are banned from using our launcher!", FrmAlert.enmType.Warning);
            }
            
        }

        private void label2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        void getUsername()
        {
            var cfg = new ConfigParser(appConfig);
            cfg.SetValue("ACCOUNT", "email", email.Text);
            cfg.SetValue("ACCOUNT", "pass", password.Text);
            cfg.Save();
            Alert("Login Success", FrmAlert.enmType.Succes);
            var r_key = cfg.GetValue("RemoteLauncher", "key");
            string webURL = r_key+ "/api/mythicallauncher/user/getusername.php?email=" + email.Text + "&pass=" + password.Text;
            WebClient wc = new WebClient();
            wc.Headers.Add("user-agent", "Only a Header!");
            byte[] rawByteArray = wc.DownloadData(webURL);
            string webContent = Encoding.UTF8.GetString(rawByteArray);
            username = webContent;
            UpdateSession(MSession.GetOfflineSession(username));
            

        }
        private void label3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            FrmRegister x = new FrmRegister();
            x.Show();
            this.Hide();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            var cfg = new ConfigParser(appConfig);
            var appWebsite = cfg.GetValue("RemoteLauncher", "appWebsite");
            System.Diagnostics.Process.Start(appWebsite);

        }

        private void lblstore_Click(object sender, EventArgs e)
        {
            var cfg = new ConfigParser(appConfig);
            var appStore = cfg.GetValue("RemoteLauncher", "appStore");
            System.Diagnostics.Process.Start(appStore);
        }

        private void lblvote_Click(object sender, EventArgs e)
        {
            var cfg = new ConfigParser(appConfig);
            var appVote = cfg.GetValue("RemoteLauncher", "appVote");
            System.Diagnostics.Process.Start(appVote);
        }

        private void lbldiscord_Click(object sender, EventArgs e)
        {
            var cfg = new ConfigParser(appConfig);
            var appDiscord = cfg.GetValue("RemoteLauncher", "appDiscord");
            System.Diagnostics.Process.Start(appDiscord);
        }
    }
}
