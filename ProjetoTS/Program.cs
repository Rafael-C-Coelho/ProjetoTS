using ProtoIP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjetoTS
{

    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Setup();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Login());
        }

        static void Setup()
        {
            Client client = new Client();
            Packet packet = new Packet((int)ChatPacket.Type.SERVER_PUBLIC_KEY);
            client.Send(Packet.Serialize(packet));
            client.Receive();
        }
    }
}
