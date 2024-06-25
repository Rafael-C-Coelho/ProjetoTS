using ProtoIP;
using ProtoIP.Crypto;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

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
            txtBoxUser.Text = this.username;
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
            Packet receivedPacket = AssembleReceivedDataIntoPacket(); //é construído um pacote a partir dos dados recebidos 

            if (receivedPacket._GetType() == (int)ChatPacket.Type.LIST_USERS_SUCCESS)
            {
                string message = receivedPacket.GetDataAs<string>(); // extrai a mensagem do pacote enquanto string 
                List<string> users = message.Split(',').ToList(); // string message é dividida em substrings sempre que uma vírgula (,) é encontrada 
                form.AddUsersToDropDown(users); // adiciona esses utilizadores a um dropwdown no formulário
                Disconnect(); //disconecta do servidor
            }
            else if (receivedPacket._GetType() == (int)ChatPacket.Type.LIST_USERS_ERROR) // caso dê erro
            {
                MessageBox.Show("Error: " + DecryptMessageWithAES(receivedPacket.GetDataAs<byte[]>())); //apresenta a mensagem de erro ao descriptografar 
                form.Close(); //fecha o formulário 
                Disconnect(); //disconecta do servidor 
            }
            else if (receivedPacket._GetType() == (int)ChatPacket.Type.SEND_USER_PUBLIC_KEY_SUCCESS) // chave pública do utilizador final é recebida com sucesso
            {
                ToUserPublicKey = Encoding.UTF8.GetString(DecryptMessageWithAES(receivedPacket.GetDataAs<byte[]>())); // desencripta  a chave pública do destinatário da msg
                Packet packet = new Packet((int)ChatPacket.Type.SEND_MESSAGE); // cria pacote para enviar a mensagem 
                packet.SetPayload(EncryptMessageWithAES(this.authtoken + ":" + form.GetUserTo() + ":" + Convert.ToBase64String(this.EncryptMessageWithPublicKey(form.GetMessage(), this.ToUserPublicKey)))); // define o payload do pacote e inclui o token de autenticação, o destinatário e a mensagem encriptografa
                this.Send(Packet.Serialize(packet)); //envia o pacote
                this.Receive(); // recebe a resposta em como foi recebido 
                this.Disconnect(); //desconecta-se do servidor 
            } else if (receivedPacket._GetType() == (int)ChatPacket.Type.SEND_USER_PUBLIC_KEY_ERROR) // caso dê erro na obtenção da chave pública 
            {
                MessageBox.Show("Error: " + DecryptMessageWithAES(receivedPacket.GetDataAs<byte[]>())); //exibe uma mensagem de erro quando tenta desencriptar a mensagem 
                Disconnect(); //desconecta-se do servidor 
            }
            else if (receivedPacket._GetType() == (int)ChatPacket.Type.SEND_MESSAGE_SUCCESS) // envio da mensagem com sucesso 
            {
                MessageBox.Show("Message sent successfully."); // exibe a mensagem 
                form.Close(); //fecha o formulário 
                Disconnect(); //desconecta-se do servidor 
            }
            else if (receivedPacket._GetType() == (int)ChatPacket.Type.SEND_MESSAGE_ERROR) // caso dê erro no envio da mensagem
            {
                MessageBox.Show("Error: " + DecryptMessageWithAES(receivedPacket.GetDataAs<byte[]>()));
                Disconnect();
            } else
            {
                Disconnect(); return;
            }
        }
    }
}
