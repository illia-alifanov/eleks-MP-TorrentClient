using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TorrentClient.DHT
{
    public class DHTServer : UdpClient
    {
        private CancellationToken _cancellationToken;
        private IDictionary<NetHost, byte[]> _dhtBus;

        public DHTServer(IDictionary<NetHost, byte[]> dhtBus,  CancellationToken cancellationToken) : base(1234)
        {
            _cancellationToken = cancellationToken;
            _dhtBus = dhtBus;
        }

        public void Listen()
        {
            while (!_cancellationToken.IsCancellationRequested) { 
                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                
                var data = this.Receive(ref sender);
                var key = new NetHost(sender.Address, (ushort)sender.Port);
                _dhtBus.Add(key, data);
            }
        }
    }
}
