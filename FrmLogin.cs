using CmlLib.Core.Auth;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Salaros.Configuration;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace MythicalLauncher
{
    public partial class FrmLogin : Form
    {
        MLogin login = new MLogin();
  
        public static string l_username;
        public static string l_email;
        public static string l_banned;
        public static string l_admin;
        public static string l_role;
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
            lbllchname.Text = FrmLoading.appname;
        }
        void RememberMe()
        {
            var cfg = new ConfigParser(appConfig);
            var re_email = cfg.GetValue("ACCOUNT", "email");
            var re_pass = cfg.GetValue("ACCOUNT", "pass");
            if (re_email == "" || re_pass == "")
            {
                Alert("Can't get your last account info!", FrmAlert.enmType.Warning);
                Console.WriteLine("[{0:HH:mm:ss}] [AUTH] Can't get your last account info", DateTime.Now);
            }
            else
            {
                email.Text = re_email;
                password.Text = re_pass;
                var r_key = cfg.GetValue("RemoteLauncher", "key");
                string webURL = r_key + "/api/mythicallauncher/auth/login.php?email=" + email.Text + "&pass=" + password.Text;
                WebClient wc = new WebClient();
                wc.Headers.Add("user-agent", "Only a Header!");
                byte[] rawByteArray = wc.DownloadData(webURL);
                string webContent = Encoding.UTF8.GetString(rawByteArray);

                if (webContent.ToUpper().Contains("LOGIN_SUCCES"))
                    getUserData();

                else if (webContent.ToUpper().Contains("LOGIN_FAILD"))
                    InvalidLogin();
                else if (webContent.ToUpper().Contains("ERROR_BANNED"))
                    BannedLogin();
            }
        }
        void BannedLogin()
        {
            Alert("Your are banned from using our launcher!", FrmAlert.enmType.Warning);
            Console.WriteLine("[{0:HH:mm:ss}] [AUTH] [{0:HH:mm:ss}]", DateTime.Now);
        }
        void InvalidLogin()
        {
            Alert("Invalid email or password", FrmAlert.enmType.Error);
            Console.WriteLine("[{0:HH:mm:ss}] [AUTH] The email or the password is invalid", DateTime.Now);
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
        private void loginbutton_Click(object sender, EventArgs e)
        {
            if (email.Text == "" || password.Text == "")
            {
                Alert("Please fill in the textbox", FrmAlert.enmType.Error);
            }
            else
            {
                var cfg = new ConfigParser(appConfig);
                var r_key = cfg.GetValue("RemoteLauncher", "key");
                string webURL = r_key + "/api/mythicallauncher/auth/login.php?email=" + email.Text + "&pass=" + password.Text;
                WebClient wc = new WebClient();
                wc.Headers.Add("user-agent", "Only a Header!");
                byte[] rawByteArray = wc.DownloadData(webURL);
                string webContent = Encoding.UTF8.GetString(rawByteArray);

                if (webContent.ToUpper().Contains("LOGIN_SUCCES"))
                    getUserData();
                else if (webContent.ToUpper().Contains("LOGIN_FAILD"))
                    InvalidLogin();
                else if (webContent.ToUpper().Contains("ERROR_BANNED"))
                    BannedLogin();
            }

        }
        void getUserData()
        {
            Console.WriteLine("[{0:HH:mm:ss}] [USERDATA] Please wait while we download the user data!");
            try
            {
                var appcfg = new ConfigParser(appConfig);
                var r_key = appcfg.GetValue("RemoteLauncher", "key");
                string jsonFilePath = r_key + "/api/mythicallauncher/user/info.php?email=" + email.Text + "&pass=" + password.Text + "&get=username";
                using (var client = new WebClient())
                {
                    string json = client.DownloadString(jsonFilePath);
                    dynamic udata = JsonConvert.DeserializeObject(json);
                    l_username = udata.Username;
                    l_email = udata.Email;
                    l_banned = udata.Banned;
                    l_admin = udata.Admin;
                    l_role = udata.Role;
                    appcfg.SetValue("ACCOUNT", "email", email.Text);
                    appcfg.SetValue("ACCOUNT", "pass", password.Text);
                    appcfg.Save();
                    Alert("Login Success", FrmAlert.enmType.Succes);
                    UpdateSession(MSession.GetOfflineSession(l_username));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[{0:HH:mm:ss}] [USERDATA] Something wen't wrong" + ex);
            }
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

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(FrmLoading.appweb);

        }

        private void lblstore_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(FrmLoading.appstore);
        }

        private void lblvote_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(FrmLoading.appvote);
        }

        private void lbldiscord_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(FrmLoading.appdiscord);
        }
    }
}
