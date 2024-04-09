using ProtoIP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjetoTS
{
    class ChatPacket : Packet
    {
        public enum Type
        {
            SERVER_PUBLIC_KEY = 1,
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
            LIST_MESSAGES,
            LIST_MESSAGES_ERROR,
            SEND_MESSAGE,
            SEND_MESSAGE_ERROR,
            SEND_MESSAGE_SUCCESS,
        }
    }

    class ChatClient : ProtoClient
    {
        public override void OnReceive()
        {
            Packet receivedPacket = AssembleReceivedDataIntoPacket();

            if (receivedPacket._GetType() == (int)ChatPacket.Type.SERVER_PUBLIC_KEY)
            {
                Console.WriteLine("Managing server's public key...");

            }
        }
    }

    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Setup();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Login());
            Application.Run(new InBox());
            Application.Run(new Chat());
        }

        static void Setup()
        {
            string IP = "127.0.0.1";
            int PORT = 1111;
            ChatClient client = new ChatClient();
            client.Connect(IP, PORT);
            Packet packet = new Packet((int)ChatPacket.Type.SERVER_PUBLIC_KEY);
            client.Send(Packet.Serialize(packet));
        }
    }
}
