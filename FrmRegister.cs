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

            lbllchname.Text = FrmLoading.appname;
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
            System.Diagnostics.Process.Start(FrmLoading.appweb);
        }

        private void lbldiscord_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(FrmLoading.appdiscord);
        }

        private void lblstore_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(FrmLoading.appstore);
        }

        private void lblvote_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(FrmLoading.appvote);
        }
    }
}
