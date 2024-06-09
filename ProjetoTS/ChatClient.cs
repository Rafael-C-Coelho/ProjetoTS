using ProtoIP;
using ProtoIP.Crypto;
using Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RSA = System.Security.Cryptography.RSA;

namespace ProjetoTS
{
    class ChatPacket : Packet
    {
        public enum Type
        { ////enumeração dos tipos de mensagens correspondentes ao número que surge quando se efetua o debug do servidor.
          ////Por exemplo, se surgir o número 6 quer dizer que estamos perante um "LOGIN_ERROR"
            SERVER_PUBLIC_KEY = 1,
            AES_KEY,
            PING,
            PONG,
            LOGIN,
            LOGIN_ERROR,
            LOGIN_SUCCESS,
            LOGOUT,
            REGISTER,
            REGISTER_ERROR,
            REGISTER_SUCCESS,
            LIST_USERS,
            LIST_USERS_ERROR,
            LIST_USERS_SUCCESS,
            SEND_USER_PUBLIC_KEY,
            SEND_USER_PUBLIC_KEY_ERROR,
            SEND_USER_PUBLIC_KEY_SUCCESS,
            LIST_MESSAGES,
            LIST_MESSAGES_ERROR,
            LIST_MESSAGES_SUCCESS,
            SEND_MESSAGE,
            SEND_MESSAGE_ERROR,
            SEND_MESSAGE_SUCCESS,
            DELETE_USER,
            DELETE_USER_ERROR,
            DELETE_USER_SUCCESS,
        }
    }

    class Client : ProtoClient
    {
        public DBHelper dbhelper;
        public RSAParameters serverPublicKey;
        public RSAParameters clientPublicKey;
        public RSAParameters clientPrivateKey;
        public string clientPublicKeyString;
        public string clientPrivateKeyString;
        public string authtoken;
        public AES aesKey;
        private Logger logger = new Logger("ChatClient.log");

        public Client()
        {
            string IP = "127.0.0.1";
            int PORT = 1111;
            dbhelper = new DBHelper();
            try
            {
                this.Connect(IP, PORT);
            } catch (Exception e)
            {
                MessageBox.Show(e.Message);
                logger.Exception(e);
            }
            Console.WriteLine("Starting to load the private/public keys needed.");
            logger.Info("Info: Starting to load the private/public keys needed.");
            LoadServerKey();
            aesKey = new AES();
            aesKey.GenerateKey();
            Console.WriteLine("Successfully loaded the private/public keys needed.");
            logger.Info("Info: Starting to load the private/public keys needed.");
        }

        public void LoadClientKeys(string username)
        {
            logger.Info("Info: Loading client keys...");
            using (RSA rSA = RSA.Create())
            {
                clientPrivateKeyString = dbhelper.GetPrivateKey(username);
                rSA.FromXmlString(clientPrivateKeyString);
                clientPrivateKey = rSA.ExportParameters(true);
                clientPublicKeyString = dbhelper.GetPublicKey(username);
                rSA.FromXmlString(clientPublicKeyString);
                clientPublicKey = rSA.ExportParameters(false);
            }
        }

        public void CreateClientKeys(string username)
        {
            logger.Info("Info: Starting create keys...");
            using (RSA rSA = RSA.Create())
            {
                clientPrivateKey = rSA.ExportParameters(true);
                clientPublicKey = rSA.ExportParameters(false);
                clientPrivateKeyString = rSA.ToXmlString(true);
                clientPublicKeyString = rSA.ToXmlString(false);
                dbhelper.InsertUser(username, clientPrivateKeyString, clientPublicKeyString, "");
            }
            logger.Info("Info: Client keys successfully created.");
        }

        //método para armazenar chave pública do servidor 
        public void SaveServerKey(string key)
        {
            logger.Info("Info: Saving server key...");
            using (RSA rSA = RSA.Create())
            {
                File.WriteAllText(GetPathString("server.key"), key);
            }
            logger.Info("Info: Server key successfully saved.");
        }

        public void LoadServerKey()
        {
            if (File.Exists(GetPathString("server.key")))
            {
                using (RSA rSA = RSA.Create())
                {
                    rSA.FromXmlString(File.ReadAllText(GetPathString("server.key")));
                    serverPublicKey = rSA.ExportParameters(false);
                }
            }
        }

        // método que permite criptografar mensagens com o algortimo de chave pública RSA 
        public byte[] EncryptMessageWithPublicKey(string message, string publicKeyXml)
        {
            logger.Info("Info: Encrypting message with public key...");
            byte[] data = Encoding.UTF8.GetBytes(message);
            if (data.Length == 0)
            {
                return new byte[0];
            }
            using (RSA rsa = RSA.Create())
            {
                rsa.FromXmlString(publicKeyXml);
                return rsa.Encrypt(data, RSAEncryptionPadding.Pkcs1);
            }
          
        }

        // método que permite criptografar bytes com o algortimo de chave pública RSA
        public byte[] EncryptBytesWithPublicKey(byte[] message, string publicKeyXml)
        {
            logger.Info("Info: Encrypting bytes with public key...");
            using (RSA rsa = RSA.Create())
            {
                rsa.FromXmlString(publicKeyXml);
                return rsa.Encrypt(message, RSAEncryptionPadding.Pkcs1);
            }
        }

        // este método visa a desencriptografia de mensagens através da chave privada 
        public byte[] DecryptMessageWithPrivateKey(string message, string privateKeyXml)
        {
            logger.Info("Info: Decrypting message...");
            byte[] data = Encoding.UTF8.GetBytes(message);
            if (data.Length == 0)
            {
                return new byte[0];
            }
            using (RSA rsa = RSA.Create())
            {
                rsa.FromXmlString(privateKeyXml);
                return rsa.Decrypt(data, RSAEncryptionPadding.Pkcs1);
            
            }
        }

        // método que permite desencriptografar bytes através da chave privada 
        public byte[] DecryptBytesWithPrivateKey(byte[] data, string privateKeyXml)
        {
            logger.Info("Info: Decrypting bytes...");
            if (data.Length == 0)
            {
                return new byte[0];
            }
            using (RSA rsa = RSA.Create())
            {
                rsa.FromXmlString(privateKeyXml);
                return rsa.Decrypt(data, RSAEncryptionPadding.Pkcs1);
            }
        }

        // método começa por verificar se existe o diretório "ProjetoTS", caso não exista, cria-o. Depois, fornece um caminho seguro que permite armazenar os ficheiros dentro da estrutura de pastas 

        public string GetPathString(string filename)
        {
            if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ProjetoTS")))
            {
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ProjetoTS"));
                logger.Info("Info: Directory created successfully.");
            }
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "ProjetoTS",
                filename
            );
        }

        // método usado para carregar o token de autenticação do username do utilizador na base de dados 
        protected string LoadAuthToken(string username)
        {
            logger.Info("Info: Loading authtoken...");
            return dbhelper.GetAuthToken(username);
        }

        // método que possibilita a deserialização da chave pública
        protected RSAParameters DeserializePublicKey(string publicKeyXml)
        {
            logger.Info("Info: Deserialisation of the public key started...");
            clientPublicKey = new RSAParameters();
            using (RSA rsa = RSA.Create())
            {
                rsa.FromXmlString(publicKeyXml);
                return rsa.ExportParameters(false);
            }
        }

        // método de desencriptação de mensagem através do algortimo AES
        public byte[] DecryptMessageWithAES(byte[] data)
        {
            logger.Info("Info: Decrypting message...");
            if (data.Length == 0)
            {
                return new byte[0];
            }
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportParameters(clientPrivateKey);
                byte[] aesKeyData = rsa.Decrypt(data.Take(rsa.KeySize / 8).ToArray(), RSAEncryptionPadding.Pkcs1);
                byte[] bytes = data.LongLength >= rsa.KeySize / 8 ? data.Skip(rsa.KeySize / 8).ToArray() : data;
                aesKey = new AES(aesKeyData);
                return aesKey.Decrypt(bytes);
            }

        }


        // método de encriptação de mensagem através do algortimo AES
        public byte[] EncryptMessageWithAES(string message)
        {
            logger.Info("Info: Ecrypting message...");
            LoadServerKey();
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportParameters(serverPublicKey);
                byte[] data = Encoding.UTF8.GetBytes(message);
                byte[] encryptedData = aesKey.Encrypt(data);
                byte[] encryptedAESKey = EncryptBytesWithPublicKey(aesKey.GetKeyBytes(), rsa.ToXmlString(false));
                return encryptedAESKey.Concat(encryptedData).ToArray();
            }
        }

        //método que verifica se o pacote recebido pelo utilizador é a sua chave pública . Se for, então é deserialziada e armazenada, e termina a conexão com o servidor  
        public override void OnReceive()
        {
            Packet receivedPacket = AssembleReceivedDataIntoPacket();

            if (receivedPacket._GetType() == (int)ChatPacket.Type.SERVER_PUBLIC_KEY)
            {
                byte[] data = receivedPacket.GetDataAs<byte[]>();
                string message = Encoding.UTF8.GetString(data);
                serverPublicKey = DeserializePublicKey(message);
                SaveServerKey(message);
                Disconnect();
                logger.Info("Info: Received server public key: ");
                logger.Debug($"Server public key: {message}");
            }
        }
    }
}
