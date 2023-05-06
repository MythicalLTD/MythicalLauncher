using System;
using System.Windows.Forms;
using CmlLib.Utils;


namespace MythicalLauncher
{
    public partial class FrmChangeLog : Form
    {
        public FrmChangeLog()
        {
            InitializeComponent();
        }
        private Changelogs changelogs;
        private async void ChangeLog_Load(object sender, EventArgs e)
        {
            btnLoad.Enabled = false;
            changelogs = await Changelogs.GetChangelogs();
            listBox1.Items.AddRange(changelogs.GetAvailableVersions());
            btnLoad.Enabled = true;
        }

        private async void btnLoad_Click(object sender, EventArgs e)
        {
            var version = listBox1.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(version))
                return;

            btnLoad.Enabled = false;

            var body = await changelogs.GetChangelogHtml(version);
            webBrowser1.DocumentText = body;

            btnLoad.Enabled = true;
        }
    }
}
