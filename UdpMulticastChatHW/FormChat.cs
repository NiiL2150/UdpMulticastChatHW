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
    public partial class FormChat : Form
    {
        public IUser ActiveUser { get; set; }
        public FormChat(IUser user)
        {
            InitializeComponent();
            ActiveUser = user;
        }

        private async void FormChat_Load(object sender, EventArgs e)
        {
            await ActiveUser.ReceiveAsync();
        }

        private async void buttonSend_Click(object sender, EventArgs e)
        {
            await ActiveUser.SendAsync();
        }
    }
}
