using ProtoIP;
using ProtoIP.Crypto;
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
        {
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
        }
    }

    class Client : ProtoClient
    {
        public RSAParameters serverPublicKey;
        public RSAParameters clientPublicKey;
        public RSAParameters clientPrivateKey;
        public string clientPublicKeyString;
        public string clientPrivateKeyString;
        public string authtoken;
        public AES aesKey;

        public Client()
        {
            string IP = "127.0.0.1";
            int PORT = 1111;
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    this.Connect(IP, PORT);
                    break;
                } catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            Console.WriteLine("Starting to load the private/public keys needed.");
            authtoken = LoadAuthTokenFromFile();
            LoadClientKeys();
            LoadServerKey();
            aesKey = new AES();
            aesKey.GenerateKey();
            Console.WriteLine("Successfully loaded the private/public keys needed.");
        }

        public void LoadClientKeys()
        {
            if (Directory.Exists(GetPathString("")) == false)
            {
                Directory.CreateDirectory(GetPathString(""));
            }

            if (File.Exists(GetPathString("private.key")) && File.Exists(GetPathString("public.key")))
            {
                using (RSA rSA = RSA.Create())
                {
                    rSA.FromXmlString(File.ReadAllText(GetPathString("private.key")));
                    clientPrivateKey = rSA.ExportParameters(true);
                    clientPrivateKeyString = rSA.ToXmlString(true);
                    rSA.FromXmlString(File.ReadAllText(GetPathString("public.key")));
                    clientPublicKey = rSA.ExportParameters(false);
                    clientPublicKeyString = rSA.ToXmlString(false);
                }
            } else
            {
                using (RSA rSA = RSA.Create())
                {
                    rSA.KeySize = 1024;
                    clientPrivateKey = rSA.ExportParameters(true);
                    clientPublicKey = rSA.ExportParameters(false);
                    clientPublicKeyString = rSA.ToXmlString(false);
                    File.WriteAllText(GetPathString("private.key"), rSA.ToXmlString(true));
                    File.WriteAllText(GetPathString("public.key"), rSA.ToXmlString(false));
                }
            }
        }

        public void SaveServerKey(string key)
        {
            using (RSA rSA = RSA.Create())
            {
                File.WriteAllText(GetPathString("server.key"), key);
            }
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

        public byte[] EncryptMessageWithPublicKey(string message, string publicKeyXml)
        {
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

        public byte[] EncryptBytesWithPublicKey(byte[] message, string publicKeyXml)
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.FromXmlString(publicKeyXml);
                return rsa.Encrypt(message, RSAEncryptionPadding.Pkcs1);
            }
        }

        public byte[] DecryptMessageWithPrivateKey(string message, string privateKeyXml)
        {
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

        public byte[] DecryptBytesWithPrivateKey(byte[] data, string privateKeyXml)
        {
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

        public static string GetPathString(string filename)
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "ProjetoTS",
                filename
            );
        }

        protected void SaveAuthTokenToFile(string authtoken)
        {
            using (StreamWriter sw = new StreamWriter(GetPathString("authtoken.key")))
            {
                sw.Write(authtoken);
            }
        }

        protected string LoadAuthTokenFromFile()
        {
            string authtoken = "";
            if (File.Exists(GetPathString("authtoken.key")))
            {
                using (StreamReader sr = new StreamReader(GetPathString("authtoken.key")))
                {
                    authtoken = sr.ReadToEnd();
                }
            }
            return authtoken;
        }

        protected RSAParameters DeserializePublicKey(string publicKeyXml)
        {
            clientPublicKey = new RSAParameters();
            using (RSA rsa = RSA.Create())
            {
                rsa.FromXmlString(publicKeyXml);
                return rsa.ExportParameters(false);
            }
        }

        public byte[] DecryptMessageWithAES(byte[] data)
        {
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

        public byte[] EncryptMessageWithAES(string message)
        {
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
            }
        }
    }
}
