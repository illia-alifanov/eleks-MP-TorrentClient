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
        private IDictionary<DHTBusKey, byte[]> _dhtBus;

        public DHTServer(IDictionary<DHTBusKey, byte[]> dhtBus,  CancellationToken cancellationToken) : base()
        {
            _cancellationToken = cancellationToken;
        }

        public void Listen()
        {
            while (!_cancellationToken.IsCancellationRequested) { 
                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                
                var data = this.Receive(ref sender);
                var key = new DHTBusKey(sender.Address, sender.Port);
                _dhtBus.Add(key, data);
            }
        }
    }
}
