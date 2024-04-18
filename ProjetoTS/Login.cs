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
    }

    class LoginClient : Client
    {
        private Login form;

        public LoginClient(Login form) : base()
        {
            this.form = form;
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
            } else if (receivedPacket._GetType() == (int)ChatPacket.Type.REGISTER_SUCCESS)
            {
                MessageBox.Show("Success");
                this.SaveAuthTokenToFile(authtoken: receivedPacket.GetDataAs<string>());
                InBox inBox = new InBox();
                inBox.Show();
                this.form.Hide();
            }
            else if (receivedPacket._GetType() == (int)ChatPacket.Type.LOGIN_ERROR)
            {
                MessageBox.Show("Error: " + receivedPacket.GetDataAs<string>());
            } else if (receivedPacket._GetType() == (int)ChatPacket.Type.LOGIN_SUCCESS)
            {
                MessageBox.Show("Success");
                this.SaveAuthTokenToFile(authtoken: receivedPacket.GetDataAs<string>());
                InBox inBox = new InBox();
                inBox.Show();
                this.form.Hide();
            }
        }
    }
}
