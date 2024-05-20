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
        public static string showUser //propriedade com encapsulamento do username 
        {
            get { return username; } // retorna o valor de username 
            set {username = value; } // define o valor do username e atualiza-o
        }
        
        public static string usernameRegister
        {
            get { return username; }
            set { username = value; }
        }
        private bool ValidatePassword(string password, out string ErrorMessage) // método com diversas validações de password com regex
                                                                                // aquando o login do cliente.
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
            string errorMessage; //string que irá conter as mensagens de erro

            if (!ValidatePassword(txtBoxPassword.Text, out errorMessage))  //verificação da password 
            {
                MessageBox.Show("Error: " + errorMessage); 
                return;
            }
            
            usernameRegister = txtBoxUsername.Text;
            LoginClient client = new LoginClient(this); //criação do cliente para login 
            Packet packet = new Packet((int)ChatPacket.Type.REGISTER);
            client.CreateClientKeys(txtBoxUsername.Text); //criação das chaves criptografadas
            packet.SetPayload(client.EncryptMessageWithAES(txtBoxUsername.Text + ":" + txtBoxPassword.Text + ":" + client.clientPublicKeyString));// criação e envio de um pacote com informações criptografadas (nome de utilizador, senha e chave pública) para o servidor
            client.Send(Packet.Serialize(packet));
            client.Receive(); //resposta do servidor 
            client.Disconnect(); //desconexão do cliente
          
            
        }


        private void buttonLogin_Click(object sender, EventArgs e)
        {
            showUser = txtBoxUsername.Text; //permite que o nome do utilizador apareça no form InBox e Chat 
            LoginClient client = new LoginClient(this); //instanciação neecssária para a comunicação com o servidor 
            Packet packet = new Packet((int)ChatPacket.Type.LOGIN); //criação do pacote de dados para o Login 
            packet.SetPayload(client.EncryptMessageWithAES(txtBoxUsername.Text + ":" + txtBoxPassword.Text)); //mensagem criptografada a enviar para o servidor, sob a forma de pacote
            client.Send(Packet.Serialize(packet)); //serialização do pacote e envio para o servidor 
            client.Receive(); 
            client.Disconnect();
            //após o envio, aguarda-se que o cliente receba a mensagem do servidor e se desconecte 
        }

        public string GetUsername() //obter username do utilizador 
        {
            return txtBoxUsername.Text;
        }


    }

    class LoginClient : Client
    {
        private Login form;
        private DBHelper dbhelper; // classe Dbhelper permite interagir com a base de dados 

        public LoginClient(Login form) : base() // o contrutor vai permitir estabelecer uma conexão entre a base de dados e o form do login
        {
            this.form = form;
            dbhelper = new DBHelper();
        }

        public override void OnReceive() //método responsável por diversas verificações no envio de pacotes de dados para o servidor 
        {
            Packet receivedPacket = AssembleReceivedDataIntoPacket(); 

            if (receivedPacket._GetType() == (int)ChatPacket.Type.SERVER_PUBLIC_KEY) // ocorre uma verificação do pacote que contém a chave pública no decorrer do processo de envio, obtenção e deserialização da chave pública pelo servidor,
                                                                                     // que depois armazena-a e desconecta-se 
            {
                byte[] data = receivedPacket.GetDataAs<byte[]>();
                string message = Encoding.UTF8.GetString(data);
                RSAParameters publicKey = DeserializePublicKey(message);
                serverPublicKey = publicKey;
                Disconnect();
            }
            else if (receivedPacket._GetType() == (int)ChatPacket.Type.REGISTER_ERROR) //caso ocorra um erro no processo de envio do pacote descrito anteriormente,
                                                                                       //apresenta uma mensagem de erro e desconeta-se 
            {
                MessageBox.Show("Error: " + receivedPacket.GetDataAs<string>());
                Disconnect();
            }
            else if (receivedPacket._GetType() == (int)ChatPacket.Type.REGISTER_SUCCESS) //caso o pacote com os dados de registo do utilizador (username) tenha sido enviado com sucesso para o servidor,
                                                                                         //é enviada uma mensagem ao utilizador. Paralelamente,atualiza o token de autenticação do utilizador no banco de dados,
                                                                                         //por via do dbhelper e carrega as chaves do cliente. Por fim, surge a página das mensagens inBox, a página de login é fechada e há a desconexão utilizador - servidor 

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
            else if (receivedPacket._GetType() == (int)ChatPacket.Type.LOGIN_ERROR) // erro no envio do pacote, que culmina numa mensagem indicando isso mesmo e a desconexão user - servidor 
            {
                MessageBox.Show("Error: " + receivedPacket.GetDataAs<string>());
                Disconnect();
            }
            else if (receivedPacket._GetType() == (int)ChatPacket.Type.LOGIN_SUCCESS) // caso o login tenha sido efetuado com sucesso, é apresentada uma mensagem referindo isso mesmo, são carregadas as chaves do cliente
                                                                                      // e atualizados os tokens de autentição do utilizador no banco de dados através do dbhelper.
                                                                                      // Depois, é aberto o form inBox e o form de login é fechado, termina coma  desconexão entre o utilizador e o servidor
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

