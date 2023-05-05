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
    public partial class FrmRegister : Form
    {
        public FrmRegister()
        {
            InitializeComponent();
        }
        private string appConfig = Application.StartupPath + @"\settings.ini";

        void Alert(string msg, FrmAlert.enmType type)
        {
            FrmAlert frm = new FrmAlert();
            frm.showAlert(msg, type);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Application.Exit();
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


        private void FrmRegister_Load(object sender, EventArgs e)
        {
            LoadSettings();
            DisplayImage();
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
            registerbutton.FillColor = ColorTranslator.FromHtml(appcolour);
            registerbutton.FillColor2 = ColorTranslator.FromHtml(appcolour);
        }
        void LoadSettings()
        {
            var cfg = new ConfigParser(appConfig);
            var appName = cfg.GetValue("RemoteLauncher", "appName");
            lbllchname.Text = appName;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void registerbutton_Click_1(object sender, EventArgs e)
        {
            if (username.Text == "" || password.Text == "" || email.Text == "")
            {
                Alert("Please fill in your information", FrmAlert.enmType.Error);
            }
            else
            {
                var appcfg = new ConfigParser(appConfig);
                var r_key = appcfg.GetValue("RemoteLauncher", "key");
                string webURL = r_key + "/api/mythicallauncher/auth/register.php?email=" + email.Text + "&usrn=" + username.Text + "&pass=" + password.Text;
                WebClient wc = new WebClient();
                wc.Headers.Add("user-agent", "Only a Header!");
                byte[] rawByteArray = wc.DownloadData(webURL);
                string webContent = Encoding.UTF8.GetString(rawByteArray);

                if (webContent.ToUpper().Contains("USERNAME_EXISTS"))
                    Alert("Username already exists", FrmAlert.enmType.Warning);
                else if (webContent.ToUpper().Contains("EMAIL_EXISTS"))
                    Alert("Email already exists", FrmAlert.enmType.Warning);
                else if (webContent.ToUpper().Contains("SUCCES_REGISTER"))
                    Alert("Registered", FrmAlert.enmType.Succes);
                FrmLogin x = new FrmLogin();
                x.Show();
                this.Hide();
            }
            
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

        }

        private void label4_Click_1(object sender, EventArgs e)
        {
            FrmLogin x = new FrmLogin();
            x.Show();
            this.Hide();
        }

        private void lblweb_Click(object sender, EventArgs e)
        {
            var cfg = new ConfigParser(appConfig);
            var appWebsite = cfg.GetValue("RemoteLauncher", "appWebsite");
            System.Diagnostics.Process.Start(appWebsite);
        }

        private void lbldiscord_Click_1(object sender, EventArgs e)
        {
            var cfg = new ConfigParser(appConfig);
            var appDiscord = cfg.GetValue("RemoteLauncher", "appDiscord");
            System.Diagnostics.Process.Start(appDiscord);
        }

        private void lblstore_Click_1(object sender, EventArgs e)
        {
            var cfg = new ConfigParser(appConfig);
            var appStore = cfg.GetValue("RemoteLauncher", "appStore");
            System.Diagnostics.Process.Start(appStore);
        }

        private void lblvote_Click_1(object sender, EventArgs e)
        {
            var cfg = new ConfigParser(appConfig);
            var appVote = cfg.GetValue("RemoteLauncher", "appVote");
            System.Diagnostics.Process.Start(appVote);
        }
    }
}
