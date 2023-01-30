using System;
using System.Windows.Forms;

namespace MythicalLauncher
{
    public partial class FrmLoading : Form
    {
        public FrmLoading()
        {
            InitializeComponent();
        }

        private void lblexit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
