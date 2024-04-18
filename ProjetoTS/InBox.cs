using ProtoIP;
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

namespace ProjetoTS
{
    public partial class InBox : Form
    {
        private InBoxClient client;
        private List<string> messages;

        public InBox()
        {
            InitializeComponent();
            messages = new List<string>();
            FetchNewMessages();
        }

        private void FetchNewMessages()
        {
            client = new InBoxClient(this);
            Packet packet = new Packet((int)ChatPacket.Type.LIST_MESSAGES);
            packet.SetPayload(client.EncryptMessageWithAES(client.authtoken));
            client.Send(Packet.Serialize(packet));
            client.Receive();
        }

        public void AddMessage(string username, string message)
        {
            listBoxMessages.Items.Add(username);
            messages.Add(message);
        }

        private void btnNewChat_Click(object sender, EventArgs e)
        {
            Chat chat = new Chat();
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
                            ) + Environment.NewLine +
                            "------------" + Environment.NewLine +
                            "Encrypted:" + Environment.NewLine +
                            messages[listBoxMessages.SelectedIndex];
                    } catch (CryptographicException)
                    {
                        txtBoxMessages.Text = "Error: Could not decrypt message. Either the keys have changed or been deleted.";
                    }
                }
            }
        }
    }

    class InBoxClient : Client
    {
        private InBox form;

        public InBoxClient(InBox form) : base()
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
            else if (receivedPacket._GetType() == (int)ChatPacket.Type.LIST_MESSAGES_ERROR)
            {
                MessageBox.Show("Error: " + receivedPacket.GetDataAs<byte[]>());
            }
            else if (receivedPacket._GetType() == (int)ChatPacket.Type.LIST_MESSAGES_SUCCESS)
            {
                string messages = Encoding.UTF8.GetString(DecryptMessageWithAES(receivedPacket.GetDataAs<byte[]>()));
                if (messages == "")
                {
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
            }
        }
    }
}
