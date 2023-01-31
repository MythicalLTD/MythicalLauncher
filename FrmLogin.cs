using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MythicalLauncher
{
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            string json = File.ReadAllText("file.json");
            dynamic data = JsonConvert.DeserializeObject(json);
            textBox1.Text = data.Name + "\r\n" + data.Age + "\r\n" + data.City;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
