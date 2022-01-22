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
            labelName.Text = ActiveUser.Name;
            Timer timer = new Timer();
            timer.Tick += Timer_Tick;
            timer.Interval = 1000;
            timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (ActiveUser.IsKicked)
            {
                Timer? timer = (Timer?)sender;
                timer?.Stop();
                this.Hide();
                MessageBox.Show("You've been kicked");
                this.Close();
            }
        }

        private async void FormChat_Load(object sender, EventArgs e)
        {
            await ActiveUser.ReceiveSettingsAsync();
        }

        private async void buttonSend_Click(object sender, EventArgs e)
        {
            await ActiveUser.SendMessageAsync(textBoxMessage.Text);
            textBoxMessage.Clear();
        }

        private async void FormChat_Shown(object sender, EventArgs e)
        {
            await foreach (var item in ActiveUser.ReceiveMessageAsync())
            {
                textBoxChat.AppendText(item);
            }
        }
    }
}
