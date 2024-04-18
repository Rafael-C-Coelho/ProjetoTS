using Server;
using ProtoIP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO.Compression;
using ProtoIP.Crypto;
using RSA = System.Security.Cryptography.RSA;

//EXERCÍCIO B
namespace Server
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

    class ChatServer : ProtoServer
    {
        private static Random random = new Random();
        protected RSAParameters serverPrivateKey;
        protected RSAParameters serverPublicKey;
        private string ServerPrivateKeyPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ProjetoTS",
            "server_private.key"
        ).ToString();
        private AES aesKey;
        private byte[] aesIV;

        public ChatServer()
        {
            // Load server's private key or generate a new one if not exists
            if (File.Exists(ServerPrivateKeyPath))
            {
                Console.WriteLine($"Loading the RSA keys at {ServerPrivateKeyPath}...");
                serverPrivateKey = LoadRSAParameters(ServerPrivateKeyPath, true);

                using (RSA rsa = RSA.Create())
                {
                    rsa.ImportParameters(serverPrivateKey);
                    serverPublicKey = rsa.ExportParameters(false);
                }
            }
            else
            {
                if (!Directory.Exists(Path.GetDirectoryName(ServerPrivateKeyPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(ServerPrivateKeyPath));

                using (RSA rsa = RSA.Create())
                {
                    rsa.KeySize = 1024;
                    Console.WriteLine($"Creating the RSA keys at {ServerPrivateKeyPath}...");
                    serverPrivateKey = rsa.ExportParameters(true);
                    serverPublicKey = rsa.ExportParameters(false);
                    SaveRSAParameters(serverPrivateKey, ServerPrivateKeyPath);
                }
            }
            Console.WriteLine("Finished");
        }

        public static string GenerateAuthtoken(string username)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            string auth = new string(Enumerable.Repeat(chars, 16)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            DBHelper dBHelper = new DBHelper();
            dBHelper.InsertToken(auth, username);
            return auth;
        }

        public byte[] DecryptMessageWithAES(byte[] data)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                if (data.Length == 0)
                {
                    return new byte[0];
                }
                rsa.ImportParameters(serverPrivateKey);
                byte[] aesKeyData = rsa.Decrypt(data.Take(rsa.KeySize / 8).ToArray(), false);
                aesKey = new AES(aesKeyData);
                byte[] bytes = data.Skip(rsa.KeySize / 8).ToArray();
                return aesKey.Decrypt(bytes);
            }
        }

        public byte[] EncryptMessageWithPublicKey(byte[] data, string publicKeyXml)
        {
            if (data.Length == 0)
            {
                return new byte[0];
            }
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKeyXml);
                return rsa.Encrypt(data, false);
            }
        }

        public byte[] EncryptMessageWithAES(string message, string username)
        {
            using (RSA rsa = RSA.Create())
            {
                DBHelper dbhelper = new DBHelper();
                string userPublicKey = dbhelper.GetUserPublicKey(username);
                rsa.FromXmlString(userPublicKey);
                byte[] data = Encoding.UTF8.GetBytes(message);
                if (data.Length == 0)
                {
                    return new byte[0];
                }
                byte[] encryptedData = aesKey.Encrypt(data);
                byte[] encryptedAESKey = EncryptMessageWithPublicKey(aesKey.GetKeyBytes(), rsa.ToXmlString(false));
                return encryptedAESKey.Concat(encryptedData).ToArray();
            }
        }

        private void SaveRSAParameters(RSAParameters parameters, string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    using(RSA rsa = RSA.Create())
                    {
                        rsa.ImportParameters(parameters);
                        sw.Write(rsa.ToXmlString(true));
                    }
                }
            }
        }

        private RSAParameters LoadRSAParameters(string filePath, bool isPrivate)
        {
            string KeyXML = File.ReadAllText(filePath);
            RSAParameters keyRSA;

            using (RSA rsa = RSA.Create())
            {
                // Import RSA private key parameters from XML string
                rsa.FromXmlString(KeyXML);

                keyRSA = rsa.ExportParameters(isPrivate);
            }
            return keyRSA;
        }

        public static string ConvertMD5(string input)
        {
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = System.Security.Cryptography.MD5.HashData(inputBytes);
            return Convert.ToHexString(hashBytes);
        }

        // Encrypt data using RSA
        public byte[] EncryptDataWithRSA(string data, RSAParameters publicKey)
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportParameters(publicKey);
                if (data.Length == 0)
                {
                    return new byte[0];
                }
                return rsa.Encrypt(Encoding.UTF8.GetBytes(data), RSAEncryptionPadding.Pkcs1);
            }
        }

        public byte[] EncryptMessage(byte[] message, string publicKey, byte[] aesKey)
        {
            byte[] encryptedAESKey = EncryptDataWithRSA(Convert.ToBase64String(aesKey), serverPublicKey);
            byte[] encryptedMessage = EncryptDataWithRSA(Convert.ToBase64String(message), serverPublicKey);

            byte[] bytes = new byte[encryptedAESKey.Length + encryptedMessage.Length];
            encryptedAESKey.CopyTo(bytes, 0);
            encryptedMessage.CopyTo(bytes, encryptedAESKey.Length);

            return bytes;
        }   

        // Decrypt data using RSA
        public byte[] DecryptDataWithRSA(byte[] encryptedData)
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportParameters(serverPrivateKey);
                return rsa.Decrypt(encryptedData, RSAEncryptionPadding.Pkcs1);
            }
        }

        private byte[] DecryptMessage(Packet receivedPacket)
        {
            byte[] data = receivedPacket.GetDataAs<byte[]>();
            if (data.Length == 0)
            {
                return new byte[0];
            }
            byte[] decryptedData = DecryptMessageWithAES(data);
            return decryptedData;
        }

        private string SerializePublicKey(RSAParameters serverPrivateKey)
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportParameters(serverPrivateKey);
                return rsa.ToXmlString(false);
            }
        }

        public override void OnRequest(int userID)
        {
            Packet receivedPacket = AssembleReceivedDataIntoPacket(userID);
            DBHelper dbhelper = new DBHelper();

            if (receivedPacket._GetType() == (int)ChatPacket.Type.REGISTER)
            {
                // Decrypt register data with server's private key
                string message = Encoding.UTF8.GetString(DecryptMessage(receivedPacket));
                string[] parts = message.Split(':');
                string username = parts[0];
                string password = parts[1];
                string publicKey = parts[2];
                try
                {
                    Console.WriteLine("Registering user...");
                    dbhelper.InsertUser(username, ConvertMD5(password), publicKey);
                    Packet packet = new Packet((int)ChatPacket.Type.REGISTER_SUCCESS);
                    packet.SetPayload(GenerateAuthtoken(username));
                    Send(Packet.Serialize(packet), userID);
                    Console.WriteLine("User registered successfully.");
                } catch (Exception e)
                {
                    Packet packet = new Packet((int)ChatPacket.Type.REGISTER_ERROR);
                    packet.SetPayload(e.Message);
                    Send(Packet.Serialize(packet), userID);
                    Console.WriteLine("Error registering user: " + e.Message);
                }
            } else if (receivedPacket._GetType() == (int)ChatPacket.Type.LOGIN)
            {
                // Decrypt login data with server's private key
                string message = Encoding.UTF8.GetString(DecryptMessage(receivedPacket));
                string[] parts = message.Split(':');
                string username = parts[0];
                string password = parts[1];
                try
                {
                    bool isLoggedIn = dbhelper.CheckUser(username, ConvertMD5(password));
                    if (!isLoggedIn)
                    {
                        throw new Exception("Invalid username or password");
                    }
                    string authtoken = GenerateAuthtoken(username);
                    Packet packet = new Packet((int)ChatPacket.Type.LOGIN_SUCCESS);
                    packet.SetPayload(authtoken);
                    Send(Packet.Serialize(packet), userID);
                } catch (Exception e)
                {
                    Packet packet = new Packet((int)ChatPacket.Type.LOGIN_ERROR);
                    packet.SetPayload(e.Message);
                    Send(Packet.Serialize(packet), userID);
                }
            } else if (receivedPacket._GetType() == (int)ChatPacket.Type.LOGOUT)
            {
                string message = Encoding.UTF8.GetString(DecryptMessage(receivedPacket));
                string[] parts = message.Split(':');
                string username = parts[0];
                string authtoken = parts[1];
                try
                {
                    dbhelper.DeleteToken(authtoken);
                    Packet packet = new Packet((int)ChatPacket.Type.LOGOUT);
                    packet.SetPayload("Logout successful");
                    Send(Packet.Serialize(packet), userID);
                } catch (Exception e)
                {
                    Packet packet = new Packet((int)ChatPacket.Type.LOGOUT);
                    packet.SetPayload(Encoding.UTF8.GetBytes(e.Message));
                    Send(Packet.Serialize(packet), userID);
                }
            } else if (receivedPacket._GetType() == (int)ChatPacket.Type.LIST_USERS)
            {
                string message = Encoding.UTF8.GetString(DecryptMessage(receivedPacket));
                string[] parts = message.Split(':');
                string authtoken = parts[0];
                try
                {
                    string username = dbhelper.GetUserFromToken(authtoken);
                    List<string> users = dbhelper.ListUsers();
                    Packet packet = new Packet((int)ChatPacket.Type.LIST_USERS_SUCCESS);
                    packet.SetPayload(string.Join(",", users));
                    Send(Packet.Serialize(packet), userID);
                } catch (Exception e)
                {
                    Packet packet = new Packet((int)ChatPacket.Type.LIST_USERS_ERROR);
                    packet.SetPayload(Encoding.UTF8.GetBytes(e.Message));
                    Send(Packet.Serialize(packet), userID);
                }
            } else if (receivedPacket._GetType() == (int)ChatPacket.Type.LIST_MESSAGES)
            {
                string message = Encoding.UTF8.GetString(DecryptMessage(receivedPacket));
                string[] parts = message.Split(':');
                string authtoken = parts[0];
                try
                {
                    string username = dbhelper.GetUserFromToken(authtoken);
                    Packet packet = new Packet((int)ChatPacket.Type.LIST_MESSAGES_SUCCESS);
                    packet.SetPayload(EncryptMessageWithAES(string.Join(";", dbhelper.ListMessages(username)), username));
                    Send(Packet.Serialize(packet), userID);
                } catch(Exception e)
                {
                    Packet packet = new Packet((int)ChatPacket.Type.LIST_MESSAGES_ERROR);
                    packet.SetPayload(Encoding.UTF8.GetBytes(e.Message));
                    Send(Packet.Serialize(packet), userID);
                }
            } else if (receivedPacket._GetType() == (int)ChatPacket.Type.SEND_MESSAGE)
            {
                string message = Encoding.UTF8.GetString(DecryptMessage(receivedPacket));
                string[] parts = message.Split(':');
                string authtoken = parts[0];
                string userTo = parts[1];
                string messageEncrypted = parts[2];
                try
                {
                    string username = dbhelper.GetUserFromToken(authtoken);
                    dbhelper.InsertMessage(messageEncrypted, username, userTo);
                    Packet packet = new Packet((int)ChatPacket.Type.SEND_MESSAGE_SUCCESS);
                    packet.SetPayload("Message sent successfully");
                    Send(Packet.Serialize(packet), userID);
                } catch(Exception e)
                {
                    dbhelper.DeleteToken(authtoken);
                    Packet packet = new Packet((int)ChatPacket.Type.SEND_MESSAGE_ERROR);
                    packet.SetPayload(Encoding.UTF8.GetBytes(e.Message));
                    Send(Packet.Serialize(packet), userID);
                }
            } else if (receivedPacket._GetType() == (int)ChatPacket.Type.SEND_USER_PUBLIC_KEY)
            {
                string message = Encoding.UTF8.GetString(DecryptMessage(receivedPacket));
                string[] parts = message.Split(':');
                string authtoken = parts[0];
                string userTo = parts[1];
                try
                {
                    string username = dbhelper.GetUserFromToken(authtoken);
                    string userToPublicKey = dbhelper.GetUserPublicKey(userTo);
                    Packet packet = new Packet((int)ChatPacket.Type.SEND_USER_PUBLIC_KEY_SUCCESS);
                    packet.SetPayload(EncryptMessageWithAES(userToPublicKey, username));
                    Send(Packet.Serialize(packet), userID);
                } catch(Exception e)
                {
                    Packet packet = new Packet((int)ChatPacket.Type.SEND_USER_PUBLIC_KEY_ERROR);
                    packet.SetPayload(Encoding.UTF8.GetBytes(e.Message));
                    Send(Packet.Serialize(packet), userID);
                }
            } else if (receivedPacket._GetType() == (int)ChatPacket.Type.SERVER_PUBLIC_KEY)
            {
                // Send server's public key to the client
                Console.WriteLine("Sending the Server's Public Key");

                string serverPublicKeyBytes = SerializePublicKey(serverPrivateKey);
                Packet serverPublicKeyPacket = new Packet((int)ChatPacket.Type.SERVER_PUBLIC_KEY);
                serverPublicKeyPacket.SetPayload(serverPublicKeyBytes);
                Send(Packet.Serialize(serverPublicKeyPacket), userID);

                Console.WriteLine("Server's public key sent.");
            } else if (receivedPacket._GetType() == (int)ChatPacket.Type.AES_KEY)
            {
                // Decrypt AES key and IV with server's private key
                string message = Encoding.UTF8.GetString(DecryptMessage(receivedPacket));
                string[] parts = message.Split(':');
                aesKey = new AES(DecryptDataWithRSA(Convert.FromBase64String(parts[0])));
            }
        }
    }

    class Program
    {
        const int PORT = 1111;
        static void Main(string[] args)
        {
            DBHelper dbHelper = new DBHelper();
            ChatServer server = new ChatServer();
            Thread serverThread = new Thread(() => server.Start(PORT));
            serverThread.Start();
        }
    }
}
