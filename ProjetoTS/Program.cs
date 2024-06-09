using ProtoIP;
using Server;
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
            //inicialização do login 
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Setup();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Login());
        }

        // Conexão inical do utilizador ao servidor, no qual é enviado um pacote de dados ao servidor e solicitada a chave pública. 
        // A chave é depois enviada para o utilizador por parte do servidor e, por fim, é terminada a conexão entre ambos 
        static void Setup()
        {
            Client client = new Client();
            Packet packet = new Packet((int)ChatPacket.Type.SERVER_PUBLIC_KEY);
            client.Send(Packet.Serialize(packet));
            client.Receive();
            client.Disconnect();
        }
    }
}
