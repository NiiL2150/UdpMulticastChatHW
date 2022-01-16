using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UdpMulticastChatHW.Model;

namespace UdpMulticastChatHW
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonLogIn_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormChat form = new FormChat(IUser.Admin(textBoxLogIn.Text));
            form.ShowDialog();
            this.Close();
        }
    }
}
