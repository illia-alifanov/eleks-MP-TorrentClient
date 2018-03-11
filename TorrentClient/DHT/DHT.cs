using BencodeNET.Objects;
using BencodeNET.Parsing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace TorrentClient.DHT
{
    public class DHT
    {
        private DHTClient _dhtClient;

        public DHT()
        {
            _dhtClient = new DHTClient();
        }

        public string Ping()
        {
            BString nodeId = "";

            // This constructor arbitrarily assigns the local port number.
            //UdpClient udpClient = new UdpClient();
            try
            {
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(Configuration.DHTStartIP), Configuration.DHTPort);//router.bittorrent.com
                //IPEndPoint ep = new IPEndPoint(IPAddress.Parse("82.221.103.244"), 6881);//router.utorrent.com

                _dhtClient.Connect(ep);

                // Sends a message to the host to which you have connected.
                string bencoded = "d1:ad2:id20:abcdefghij0123456789e1:q4:ping1:t2:aa1:y1:qe";
                Byte[] sendBytes = Encoding.UTF8.GetBytes(bencoded);

                var bParams = new BDictionary();
                bParams.Add("id", "abcdefghij0123456789");

                BDictionary bDictionary = new BDictionary();
                bDictionary.Add("t", "aa");
                bDictionary.Add("y", "q");
                bDictionary.Add("q", "ping");
                bDictionary.Add("a", bParams);

                Byte[] byteRequest = bDictionary.EncodeAsBytes();

                //udpClient.Client.ReceiveTimeout = TimeSpan.FromSeconds(2).Seconds;
                _dhtClient.Send(byteRequest, byteRequest.Length);

                // Blocks until a message returns on this socket from a remote host.
                Byte[] receivedBytes = _dhtClient.Receive(ref ep);
                var parser = new BencodeParser();
                var bResponse = parser.Parse<BDictionary>(receivedBytes);

                Console.WriteLine("Ping. This is the message you received " + bResponse.ToString());
                Console.WriteLine("Ping. This message was sent from " + ep.Address.ToString()
                                    + " on their port number " + ep.Port.ToString());

                IBObject responseNode;
                if (bResponse.TryGetValue("r", out responseNode))
                {
                    nodeId = ((BDictionary)responseNode).Get<BString>("id");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                _dhtClient.Close();
                //Console.Write("Press any key to exit...");
                //Console.ReadKey();
            }

            return nodeId.ToString(Encoding.UTF8);
        }

        public BDictionary GetPeers(Torrent torrent, NetHost nodeHost, Dictionary<NetHost, byte[]> dhtResponses)
        {
            BDictionary response = null;
            try
            {
                var bParams = new BDictionary();
                bParams.Add("id", "abcdefghij0123456789");
                bParams.Add("info_hash", new BString(torrent.Info_Hash.Value));

                BDictionary bDictionary = new BDictionary();
                bDictionary.Add("t", "aa");
                bDictionary.Add("y", "q");
                bDictionary.Add("q", "get_peers");
                bDictionary.Add("a", bParams);

                Byte[] byteRequest = bDictionary.EncodeAsBytes();

                IPEndPoint ep = new IPEndPoint(nodeHost.IP, nodeHost.Port);//router.bittorrent.com
                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

                _dhtClient.Connect(ep);
                _dhtClient.Send(byteRequest, byteRequest.Length);
                
                Byte[] receivedBytes = _dhtClient.Receive(ref sender);

                var parser = new BencodeParser();
                var bResponse = parser.Parse<BDictionary>(receivedBytes);

                IBObject responseObject;
                if (bResponse.TryGetValue("r", out responseObject))
                {
                    response = (BDictionary)responseObject;
                    dhtResponses.Add(new NetHost(sender.Address, (ushort)sender.Port), receivedBytes);
                    return response;
                }

            }
            catch (Exception e)
            {
                //Console.WriteLine(e.ToString());
            }
            return response;
        }


        public void Send_GetPeers(Torrent torrent, NetHost nodeHost, Dictionary<NetHost, byte[]> answeredHosts)
        {
            try
            {
                IPEndPoint ep = new IPEndPoint(nodeHost.IP, nodeHost.Port);//router.bittorrent.com

                _dhtClient.Connect(ep);

                var bParams = new BDictionary();
                bParams.Add("id", "abcdefghij0123456789");
                bParams.Add("info_hash", new BString(torrent.Info_Hash.Value));

                BDictionary bDictionary = new BDictionary();
                bDictionary.Add("t", "aa");
                bDictionary.Add("y", "q");
                bDictionary.Add("q", "get_peers");
                bDictionary.Add("a", bParams);

                Byte[] byteRequest = bDictionary.EncodeAsBytes();

                _dhtClient.Send(byteRequest, byteRequest.Length);

            }
            catch (Exception e)
            {
                //Console.WriteLine(e.ToString());
            }
        }

    }
}
