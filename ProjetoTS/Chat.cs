using ProtoIP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ProjetoTS
{
    public partial class Chat : Form
    {
        public string username;
        private DBHelper dbhelper = new DBHelper();

        public Chat(string username)
        {
            InitializeComponent();
            this.username = username;
        }

        private void Chat_Load(object sender, EventArgs e)
        {
            txtBoxUser.Text = Login.showUser;
            ChatClient client = new ChatClient(this, this.username);
            Packet packet = new Packet((int)ChatPacket.Type.LIST_USERS);
            packet.SetPayload(client.EncryptMessageWithAES(client.authtoken));
            client.Send(Packet.Serialize(packet));
            client.Receive();
            client.Disconnect();
        }

        internal void AddUsersToDropDown(List<string> users)
        {
            foreach (string user in users)
            {
                cBxTo.Items.Add(user);
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            ChatClient client = new ChatClient(this, this.username);
            Packet packet = new Packet((int)ChatPacket.Type.SEND_USER_PUBLIC_KEY);
            packet.SetPayload(client.EncryptMessageWithAES(client.authtoken + ":" + cBxTo.Text));
            client.Send(Packet.Serialize(packet));
            client.Receive();
            client.Disconnect();
        }

        public string GetUserTo()
        {
            return cBxTo.Text;
        }

        public string GetMessage()
        {
            return textBox.Text;
        }
    }

    internal class ChatClient : Client
    {
        protected Chat form;
        public string ToUserPublicKey;

        public ChatClient(Chat form, string username) : base()
        {
            this.form = form;
            base.LoadClientKeys(username);
            base.authtoken = LoadAuthToken(username);
        }

        public override void OnReceive()
        {
            Packet receivedPacket = AssembleReceivedDataIntoPacket();

            if (receivedPacket._GetType() == (int)ChatPacket.Type.LIST_USERS_SUCCESS)
            {
                string message = receivedPacket.GetDataAs<string>();
                List<string> users = message.Split(',').ToList();
                form.AddUsersToDropDown(users);
                Disconnect();
            }
            else if (receivedPacket._GetType() == (int)ChatPacket.Type.LIST_USERS_ERROR)
            {
                MessageBox.Show("Error: " + DecryptMessageWithAES(receivedPacket.GetDataAs<byte[]>()));
                form.Close();
                Disconnect();
            }
            else if (receivedPacket._GetType() == (int)ChatPacket.Type.SEND_USER_PUBLIC_KEY_SUCCESS)
            {
                ToUserPublicKey = Encoding.UTF8.GetString(DecryptMessageWithAES(receivedPacket.GetDataAs<byte[]>()));
                Packet packet = new Packet((int)ChatPacket.Type.SEND_MESSAGE);
                packet.SetPayload(EncryptMessageWithAES(this.authtoken + ":" + form.GetUserTo() + ":" + Convert.ToBase64String(this.EncryptMessageWithPublicKey(form.GetMessage(), this.ToUserPublicKey))));
                this.Send(Packet.Serialize(packet));
                this.Receive();
                this.Disconnect();
            } else if (receivedPacket._GetType() == (int)ChatPacket.Type.SEND_USER_PUBLIC_KEY_ERROR)
            {
                MessageBox.Show("Error: " + DecryptMessageWithAES(receivedPacket.GetDataAs<byte[]>()));
                Disconnect();
            }
            else if (receivedPacket._GetType() == (int)ChatPacket.Type.SEND_MESSAGE_SUCCESS)
            {
                MessageBox.Show("Message sent successfully.");
                form.Close();
                Disconnect();
            }
            else if (receivedPacket._GetType() == (int)ChatPacket.Type.SEND_MESSAGE_ERROR)
            {
                MessageBox.Show("Error: " + DecryptMessageWithAES(receivedPacket.GetDataAs<byte[]>()));
                Disconnect();
            }
        }
    }
}
