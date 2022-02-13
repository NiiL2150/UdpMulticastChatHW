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
using static UdpMulticastChatHW.Model.IPStorage;

namespace UdpMulticastChatHW
{
    public partial class Form1 : Form
    {
        private NetworkModes mode = NetworkModes.LocalHost;

        public Form1()
        {
            InitializeComponent();
        }

        private void buttonLogIn_Click(object sender, EventArgs e)
        {
            FormChat form;
            string tmpName = textBoxLogIn.Text;
            string tmpPass = textBoxPassword.Text;
            if (!IsValidUserName(tmpName) || !IsValidPassword(tmpPass))
            {
                return;
            }
            if(tmpName.ToLower() == "admin")
            {
                ChatDBContext db = new ChatDBContext();
                if (db.Database.EnsureCreated())
                {
                    db.Users.Add(new DBModel.DBUser() { Name = "admin", Password = tmpPass });
                    db.SaveChanges();
                }
                else if (!db.Users.Any(x => x.Name == "admin"))
                {
                    db.Users.Add(new DBModel.DBUser() { Name = "admin", Password = tmpPass });
                    db.SaveChanges();
                }
                else
                {
                    if(db.Users.First(x=>x.Name=="admin").Password != tmpPass)
                    {
                        return;
                    }
                }
                form = new FormChat(new Admin());
            }
            else
            {
                form = new FormChat(new User(tmpName, tmpPass));
            }
            this.Hide();
            form.ShowDialog();
            this.Show();
        }

        private void buttonSignIn_Click(object sender, EventArgs e)
        {
            FormChat form;
            string tmpName = textBoxLogIn.Text;
            string tmpPass = textBoxPassword.Text;
            if (!IsValidUserName(tmpName) || !IsValidPassword(tmpPass))
            {
                return;
            }
            if(tmpName.ToLower() == "admin")
            {
                return;
            }
            form = new FormChat(new User(tmpName, tmpPass, User.LogType.Register));
            this.Hide();
            form.ShowDialog();
            this.Show();
    }

        public static bool IsValidString(string str)
        {
            string chars = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM1234567890_";
            char[] charArr = chars.ToCharArray();
            foreach (char ch in str)
            {
                if (!charArr.Contains(ch))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsValidUserName(string username)
        {
            return username.Length >= 4 && username.Length <= 32 && IsValidString(username);
        }

        public static bool IsValidPassword(string password)
        {
            return password.Length >= 6 && password.Length <= 128 && IsValidString(password);
        }

        private void buttonIPMode_Click(object sender, EventArgs e)
        {
            string txt = "";
            if(mode == NetworkModes.LocalNetwork)
            {
                mode = NetworkModes.LocalHost;
                txt = "Local host";
            }
            else if(mode == NetworkModes.LocalHost)
            {
                mode = NetworkModes.LocalNetwork;
                txt = "Local network";
            }
            buttonIPMode.Text = txt;
            IPStorage.ChangeMode(mode);
        }
    }
}
