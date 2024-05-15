using ProtoIP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;


namespace ProjetoTS
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        public static string username;
        public static string showUser
        {
            get { return username; }
            set {username = value; }
        }     
        private bool ValidatePassword(string password, out string ErrorMessage) // méotodo para validações de password com regex aquando o registo do cliente.
        {

            var input = password;

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMiniMaxChars = new Regex(@".{8,15}");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

            if (!hasLowerChar.IsMatch(input))
            {
                ErrorMessage = "Password should contain at least one lower case letter";
                return false;
            }
            else if (!hasUpperChar.IsMatch(input))
            {
                ErrorMessage = "Password should contain at least one upper case letter";
                return false;
            }
            else if (!hasMiniMaxChars.IsMatch(input))
            {
                ErrorMessage = "Password must be between 8 and 15 characters long";
                return false;
            }
            else if (!hasNumber.IsMatch(input))
            {
                ErrorMessage = "Password should contain at least one numeric value";
                return false;
            }

            else if (!hasSymbols.IsMatch(input))
            {
                ErrorMessage = "Password should contain at least one special case characters";
                return false;
            }
            else
            {
                ErrorMessage = "";
                return true;
            }

        }

        private void buttonRegister_Click(object sender, EventArgs e)
        {
            string errorMessage;

            if (!ValidatePassword(txtBoxPassword.Text, out errorMessage))
            {
                MessageBox.Show("Error: " + errorMessage);
                return;
            }

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
            showUser = txtBoxUsername.Text;
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
            }
            else if (receivedPacket._GetType() == (int)ChatPacket.Type.REGISTER_SUCCESS)
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
            }
            else if (receivedPacket._GetType() == (int)ChatPacket.Type.LOGIN_SUCCESS)
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

