using ProtoIP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace ProjetoTS
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void buttonRegister_Click(object sender, EventArgs e)
        {
            LoginClient client = new LoginClient(this);
            Packet packet = new Packet((int)ChatPacket.Type.REGISTER);
            client.CreateClientKeys(txtBoxUsername.Text);
            packet.SetPayload(client.EncryptMessageWithAES(txtBoxUsername.Text + ":" + txtBoxPassword.Text + ":" + client.clientPublicKeyString));
            client.Send(Packet.Serialize(packet));
            client.Receive();
            client.Disconnect();
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            LoginClient client = new LoginClient(this);
            Packet packet = new Packet((int)ChatPacket.Type.LOGIN);
            packet.SetPayload(client.EncryptMessageWithAES(txtBoxUsername.Text + ":" + txtBoxPassword.Text));
            client.Send(Packet.Serialize(packet));
            client.Receive();
            client.Disconnect();
        }

        public string GetUsername()
        {
            return txtBoxUsername.Text;
        }
    }

    class LoginClient : Client
    {
        private Login form;
        private DBHelper dbhelper;

        public LoginClient(Login form) : base()
        {
            this.form = form;
            dbhelper = new DBHelper();
        }

        public override void OnReceive()
        {
            Packet receivedPacket = AssembleReceivedDataIntoPacket();

            if (receivedPacket._GetType() == (int)ChatPacket.Type.SERVER_PUBLIC_KEY)
            {
                byte[] data = receivedPacket.GetDataAs<byte[]>();
                string message = Encoding.UTF8.GetString(data);
                RSAParameters publicKey = DeserializePublicKey(message);
                serverPublicKey = publicKey;
                Disconnect();
            }
            else if (receivedPacket._GetType() == (int)ChatPacket.Type.REGISTER_ERROR)
            {
                MessageBox.Show("Error: " + receivedPacket.GetDataAs<string>());
                Disconnect();
            } else if (receivedPacket._GetType() == (int)ChatPacket.Type.REGISTER_SUCCESS)
            {
                MessageBox.Show("Success");
                string username = this.form.GetUsername();
                dbhelper.UpdateAuthToken(username, receivedPacket.GetDataAs<string>());
                this.LoadClientKeys(username);
                InBox inBox = new InBox(username);
                inBox.Show();
                this.form.Hide();
                Disconnect();
            }
            else if (receivedPacket._GetType() == (int)ChatPacket.Type.LOGIN_ERROR)
            {
                MessageBox.Show("Error: " + receivedPacket.GetDataAs<string>());
                Disconnect();
            } else if (receivedPacket._GetType() == (int)ChatPacket.Type.LOGIN_SUCCESS)
            {
                MessageBox.Show("Success");
                string username = this.form.GetUsername();
                this.LoadClientKeys(username);
                dbhelper.UpdateAuthToken(username, receivedPacket.GetDataAs<string>());
                InBox inBox = new InBox(username);
                inBox.Show();
                this.form.Hide();
                Disconnect();
            }
        }
    }
}
