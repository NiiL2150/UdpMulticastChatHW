using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UdpMulticastChatHW.Model
{
    public static class IPStorage
    {
        public static IPEndPoint AdminEndPoint { get; }
        public static IPEndPoint AdminSettings { get; }
        public static IPEndPoint MultiCastEndPoint { get; }
        public static IPEndPoint MultiCastSettings { get; }
        public static IPEndPoint ReceiveEndPoint { get; }

        static IPStorage()
        {
            AdminEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 51234);
            MultiCastEndPoint = new IPEndPoint(IPAddress.Parse("224.5.5.5"), 51234);
            AdminSettings = new IPEndPoint(IPAddress.Parse("127.0.0.2"), 51234);
            MultiCastSettings = new IPEndPoint(IPAddress.Parse("224.6.6.6"), 51234);
            ReceiveEndPoint = new IPEndPoint(IPAddress.Any, 51234);
        }

        public static void ChangeMode(NetworkModes mode)
        {
            switch (mode)
            {
                case NetworkModes.LocalHost:
                    AdminEndPoint.Address = IPAddress.Parse("127.0.0.1");
                    AdminSettings.Address = IPAddress.Parse("127.0.0.2");
                    break;
                case NetworkModes.LocalNetwork:
                    //TODO: make this work for local networks
                    AdminEndPoint.Address = IPAddress.Parse("192.168.5.1");
                    AdminSettings.Address = IPAddress.Parse("192.168.5.2");
                    break;
            }
        }

        public static void Reuse(this UdpClient client)
        {
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        }

        public static IPEndPoint Clone(this IPEndPoint endPoint)
        {
            return new IPEndPoint(endPoint.Address, endPoint.Port);
        }

        public enum NetworkModes
        {
            LocalHost,
            LocalNetwork
        }
    }
}
