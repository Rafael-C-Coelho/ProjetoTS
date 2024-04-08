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

//EXERCÍCIO B
namespace Server
{
    class ChatPacket : Packet
    {
        public enum Type
        {
            ACK = 1,
            HANDSHAKE_REQ,
            HANDSHAKE_RES,
            SERVER_PUBLIC_KEY,
            CLIENT_PUBLIC_KEY,
            AES_KEY,
            BYTES,
            REPEAT,
            EOT,
            SOT,
            FTS,
            FTE,
            FILE_NAME,
            FILE_SIZE,
            PING,
            PONG,
            CHECKSUM,
            RELAY,
            BROADCAST,
            LOGIN,
            LOGIN_ERROR,
            LOGIN_SUCCESS,
            LOGOUT,
            REGISTER,
            REGISTER_ERROR,
            REGISTER_SUCCESS,
            LIST_USERS,
            LIST_MESSAGES,
            SEND_MESSAGE,
            SEND_MESSAGE_ERROR,
            SEND_MESSAGE_SUCCESS,
        }
    }

    class ChatServer : ProtoServer
    {
        private static Random random = new Random();
        public static string GenerateAuthtoken()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, 128)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private RSAParameters serverPrivateKey;
        private RSAParameters clientPublicKey;

        private const string ServerPrivateKeyPath = "server_private.key";
        private const string ClientPublicKeyPath = "client_public.key";

        public ChatServer()
        {
            // Load server's private key or generate a new one if not exists
            if (File.Exists(ServerPrivateKeyPath))
            {
                serverPrivateKey = LoadRSAParameters(ServerPrivateKeyPath);
            }
            else
            {
                using (RSA rsa = RSA.Create())
                {
                    serverPrivateKey = rsa.ExportParameters(true);
                    SaveRSAParameters(serverPrivateKey, ServerPrivateKeyPath);
                }
            }
        }

        private RSAParameters DeserializePublicKey(byte[] publicKeyBytes)
        {
            using (MemoryStream stream = new MemoryStream(publicKeyBytes))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    int modulusLength = reader.ReadInt32();
                    byte[] modulus = reader.ReadBytes(modulusLength);
                    int exponentLength = reader.ReadInt32();
                    byte[] exponent = reader.ReadBytes(exponentLength);

                    RSAParameters publicKey = new RSAParameters
                    {
                        Modulus = modulus,
                        Exponent = exponent
                    };

                    return publicKey;
                }
            }
        }

        private byte[] GenerateAESKey()
        {
            // Generate a new AES key
            using (Aes aes = Aes.Create())
            {
                aes.GenerateKey();
                return aes.Key;
            }
        }

        private byte[] EncryptAESKey(byte[] aesKey, RSAParameters publicKey)
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportParameters(publicKey);
                return rsa.Encrypt(aesKey, RSAEncryptionPadding.OaepSHA256);
            }
        }

        private void SaveRSAParameters(RSAParameters parameters, string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                using (BinaryWriter writer = new BinaryWriter(fs))
                {
                    writer.Write(parameters.Modulus.Length);
                    writer.Write(parameters.Modulus);
                    writer.Write(parameters.Exponent.Length);
                    writer.Write(parameters.Exponent);
                    writer.Write(parameters.D.Length);
                    writer.Write(parameters.D);
                    writer.Write(parameters.P.Length);
                    writer.Write(parameters.P);
                    writer.Write(parameters.Q.Length);
                    writer.Write(parameters.Q);
                    writer.Write(parameters.DP.Length);
                    writer.Write(parameters.DP);
                    writer.Write(parameters.DQ.Length);
                    writer.Write(parameters.DQ);
                    writer.Write(parameters.InverseQ.Length);
                    writer.Write(parameters.InverseQ);
                }
            }
        }

        private RSAParameters LoadRSAParameters(string filePath)
        {
            RSAParameters parameters = new RSAParameters();
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    int modulusLength = reader.ReadInt32();
                    parameters.Modulus = reader.ReadBytes(modulusLength);
                    int exponentLength = reader.ReadInt32();
                    parameters.Exponent = reader.ReadBytes(exponentLength);
                    parameters.D = reader.ReadBytes(reader.ReadInt32());
                    parameters.P = reader.ReadBytes(reader.ReadInt32());
                    parameters.Q = reader.ReadBytes(reader.ReadInt32());
                    parameters.DP = reader.ReadBytes(reader.ReadInt32());
                    parameters.DQ = reader.ReadBytes(reader.ReadInt32());
                    parameters.InverseQ = reader.ReadBytes(reader.ReadInt32());
                }
            }
            return parameters;
        }

        private byte[] SerializePublicKey(RSAParameters publicKey)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(publicKey.Modulus.Length);
                    writer.Write(publicKey.Modulus);
                    writer.Write(publicKey.Exponent.Length);
                    writer.Write(publicKey.Exponent);
                }
                return stream.ToArray();
            }
        }

        private byte[] DecryptData(byte[] encryptedData, RSAParameters privateKey)
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportParameters(privateKey);
                return rsa.Decrypt(encryptedData, RSAEncryptionPadding.OaepSHA256);
            }
        }

        public static string ConvertMD5(string input)
        {
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = System.Security.Cryptography.MD5.HashData(inputBytes);
            return Convert.ToHexString(hashBytes);
        }

        public override void OnRequest(int userID)
        {
            Packet receivedPacket = AssembleReceivedDataIntoPacket(userID);
            
            if (receivedPacket._GetType() == (int)ChatPacket.Type.REGISTER)
            {
                byte[] encryptedData = receivedPacket.GetDataAs<byte[]>();
                byte[] decryptedData = DecryptData(encryptedData, serverPrivateKey);
                string message = Encoding.UTF8.GetString(decryptedData);

                string[] parts = message.Split(':');
                string username = parts[0];
                string password = parts[1];
                string publicKey = parts[2];
                DBHelper dbhelper = new DBHelper();
                try
                {
                    dbhelper.InsertUser(username, ConvertMD5(password), publicKey);
                    Packet packet = new Packet((int)ChatPacket.Type.REGISTER_SUCCESS);
                    packet.SetPayload(Encoding.UTF8.GetBytes(GenerateAuthtoken()));
                    Send(Packet.Serialize(packet), userID);
                } catch (Exception e)
                {
                    Packet packet = new Packet((int)ChatPacket.Type.REGISTER_ERROR);
                    packet.SetPayload(Encoding.UTF8.GetBytes(e.Message));
                    Send(Packet.Serialize(packet), userID);
                }
            } else if (receivedPacket._GetType() == (int)ChatPacket.Type.LOGIN)
            {
                // Decrypt login data with server's private key
                byte[] encryptedData = receivedPacket.GetDataAs<byte[]>();
                byte[] decryptedData = DecryptData(encryptedData, serverPrivateKey);
                string message = Encoding.UTF8.GetString(decryptedData);

                string[] parts = message.Split(':');
                string username = parts[0];
                string password = parts[1];
                DBHelper dbhelper = new DBHelper();
                try
                {
                    bool isLoggedIn = dbhelper.CheckUser(username, ConvertMD5(password));
                    if (!isLoggedIn)
                    {
                        throw new Exception("Invalid username or password");
                    }
                    string authtoken = GenerateAuthtoken();
                    dbhelper.InsertToken(username, authtoken);
                    Packet packet = new Packet((int)ChatPacket.Type.LOGIN_SUCCESS);
                    packet.SetPayload(Encoding.UTF8.GetBytes(authtoken));
                    Send(Packet.Serialize(packet), userID);
                } catch (Exception e)
                {
                    Packet packet = new Packet((int)ChatPacket.Type.LOGIN_ERROR);
                    packet.SetPayload(Encoding.UTF8.GetBytes(e.Message));
                    Send(Packet.Serialize(packet), userID);
                }
            } else if (receivedPacket._GetType() == (int)ChatPacket.Type.LOGOUT)
            {
                byte[] data = receivedPacket.GetDataAs<byte[]>();
                string message = Encoding.UTF8.GetString(data);
                string[] parts = message.Split(':');
                string username = parts[0];
                string authtoken = parts[1];
                DBHelper dbhelper = new DBHelper();
                try
                {
                    dbhelper.DeleteToken(authtoken);
                    Packet packet = new Packet((int)ChatPacket.Type.LOGOUT);
                    packet.SetPayload(Encoding.UTF8.GetBytes("Logout successful"));
                    Send(Packet.Serialize(packet), userID);
                } catch (Exception e)
                {
                    Packet packet = new Packet((int)ChatPacket.Type.LOGOUT);
                    packet.SetPayload(Encoding.UTF8.GetBytes(e.Message));
                    Send(Packet.Serialize(packet), userID);
                }
            } else if (receivedPacket._GetType() == (int)ChatPacket.Type.LIST_USERS)
            {
                byte[] data = receivedPacket.GetDataAs<byte[]>();
                string message = Encoding.UTF8.GetString(data);
                string[] parts = message.Split(':');
                string username = parts[0];
                string authtoken = parts[1];
                DBHelper dbhelper = new DBHelper();
                try
                {
                    string user = dbhelper.GetUserFromToken(authtoken);
                    List<string> users = dbhelper.ListUsers();
                    Packet packet = new Packet((int)ChatPacket.Type.LIST_USERS);
                    packet.SetPayload(Encoding.UTF8.GetBytes(string.Join(",", users)));
                    Send(Packet.Serialize(packet), userID);
                } catch (Exception e)
                {
                    Packet packet = new Packet((int)ChatPacket.Type.LIST_USERS);
                    packet.SetPayload(Encoding.UTF8.GetBytes(e.Message));
                    Send(Packet.Serialize(packet), userID);
                }
            } else if (receivedPacket._GetType() == (int)ChatPacket.Type.LIST_MESSAGES)
            {
                byte[] data = receivedPacket.GetDataAs<byte[]>();
                string message = Encoding.UTF8.GetString(data);
                string[] parts = message.Split(':');
                string username = parts[0];
                string authtoken = parts[1];
                DBHelper dbhelper = new DBHelper();
                try
                {
                    string userFromToken = dbhelper.GetUserFromToken(authtoken);
                    if (userFromToken == username)
                    {
                        Packet packet = new Packet((int)ChatPacket.Type.LIST_MESSAGES);
                        packet.SetPayload(string.Join(";\n", dbhelper.ListMessages(username)));
                        Send(Packet.Serialize(packet), userID);
                    }
                } catch(Exception e)
                {
                    dbhelper.DeleteToken(authtoken);
                    Packet packet = new Packet((int)ChatPacket.Type.LOGOUT);
                    packet.SetPayload(Encoding.UTF8.GetBytes(e.Message));
                    Send(Packet.Serialize(packet), userID);
                }
            } else if (receivedPacket._GetType() == (int)ChatPacket.Type.SERVER_PUBLIC_KEY)
            {
                // Send server's public key to the client
                byte[] serverPublicKeyBytes = SerializePublicKey(serverPrivateKey);
                Packet serverPublicKeyPacket = new Packet((int)ChatPacket.Type.SERVER_PUBLIC_KEY);
                serverPublicKeyPacket.SetPayload(serverPublicKeyBytes);
                Send(Packet.Serialize(serverPublicKeyPacket), userID);
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
