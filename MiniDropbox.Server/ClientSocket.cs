using System.Net.Sockets;
using MiniDropbox.Shared;

namespace MiniDropbox.Server
{
    public class ClientSocket
    {
        public Socket? Socket { get; set; }
        public byte[] Buffer { get; set; }
        public string? ClientID { get; set; }

        public ClientSocket()
        {
            Buffer = new byte[Protocol.BUFFER_SIZE];
        }
    }
}