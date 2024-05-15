using ProtoIP;
using Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Security.Principal;

namespace ProjetoTS
{
    public partial class InBox : Form
    {
        private InBoxClient client;
        private List<string> messages;
        private Logger logger = new Logger("inbox.log");
        private string username;

        public InBox(string username)
        {
            this.username = username;
            InitializeComponent();
            messages = new List<string>();
            FetchNewMessages();
        }

        private void FetchNewMessages()
        {
            client = new InBoxClient(this, this.username);
            Packet packet = new Packet((int)ChatPacket.Type.LIST_MESSAGES);
            packet.SetPayload(client.EncryptMessageWithAES(client.authtoken));
            client.Send(Packet.Serialize(packet));
            client.Receive();
            client.Disconnect();
        }

        public void AddMessage(string username, string message)
        {
            foreach (string messageStored in messages)
            {
                if (message == messageStored)
                {
                    return;
                }
            }
            listBoxMessages.Items.Add(username);
            messages.Add(message);
        }

        private void btnNewChat_Click(object sender, EventArgs e)
        {
            Chat chat = new Chat(this.username);
            chat.Show();
        }

        private void listBoxMessages_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (RSA rsa = RSA.Create())
            {
                if (listBoxMessages.SelectedIndex != -1)
                {
                    try
                    {
                        rsa.ImportParameters(client.clientPrivateKey);
                        txtBoxMessages.Text =
                            "Decrypted:" + Environment.NewLine +
                            Encoding.UTF8.GetString(
                                rsa.Decrypt(
                                    Convert.FromBase64String(
                                        messages[listBoxMessages.SelectedIndex]
                                    ),
                                    RSAEncryptionPadding.Pkcs1
                                )
                            );
                        logger.Info("Encrypted:" + messages[listBoxMessages.SelectedIndex]);
                    } catch (CryptographicException)
                    {
                        logger.Error("Error: Could not decrypt message. Either the keys have changed or been deleted.");
                        txtBoxMessages.Text = "Error: Could not decrypt message. Either the keys have changed or been deleted.";
                    }
                }
            }
        }

        private void destruirUtilizadorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void timerFetchMessages_Tick(object sender, EventArgs e)
        {
            try
            {
                FetchNewMessages();
            } catch (Exception ex)
            {
                logger.Error("Error: " + ex.Message);
            }
        }

        private void refreshMessagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                FetchNewMessages();
            }
            catch (Exception ex)
            {
                logger.Error("Error: " + ex.Message);
            }
        }

        private void lblname_Click(object sender, EventArgs e)
        {
            string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        }

        private void InBox_Load(object sender, EventArgs e)
        {
            txtBoxUser.Text = Login.showUser;
            
        }


    }

    class InBoxClient : Client
    {
        private InBox form;
        private DBHelper dbhelper = new DBHelper();

        public InBoxClient(InBox form, string username) : base()
        {
            this.form = form;
            this.LoadClientKeys(username);
            base.authtoken = dbhelper.GetAuthToken(username);
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
            else if (receivedPacket._GetType() == (int)ChatPacket.Type.LIST_MESSAGES_ERROR)
            {
                MessageBox.Show("Error: " + receivedPacket.GetDataAs<string>());
                Disconnect();
            }
            else if (receivedPacket._GetType() == (int)ChatPacket.Type.LIST_MESSAGES_SUCCESS)
            {
                string messages = Encoding.UTF8.GetString(DecryptMessageWithAES(receivedPacket.GetDataAs<byte[]>()));
                if (messages == "")
                {
                    Disconnect();
                    return;
                }
                List<string> messagesList = messages.Split(';').ToList();
                foreach (string message in messagesList)
                {
                    string[] messageParts = message.Split(',');
                    string username = messageParts[0];
                    string messageContent = messageParts[1];
                    this.form.AddMessage(username, messageContent);
                }
                Disconnect();
            } else
            {
                Disconnect(); return;
            }
        }
    }
}
