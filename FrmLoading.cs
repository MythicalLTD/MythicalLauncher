using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Text;
using DiscordRPC;
using DiscordRPC.Logging;

namespace MythicalLauncher
{
    public partial class FrmLoading : Form
    {
        public FrmLoading()
        {
            InitializeRPC();
            InitializeComponent();
        }
        public DiscordRpcClient client;
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

        private async void FrmLoading_Load(object sender, EventArgs e)
        {
            await Task.Delay(10000);
            FrmLogin x = new FrmLogin();
            x.Show();
            this.Hide();
        }

        private void logo_Click(object sender, EventArgs e)
        {

        }

        private void bunifuLoader1_Click(object sender, EventArgs e)
        {

        }
    }
}
