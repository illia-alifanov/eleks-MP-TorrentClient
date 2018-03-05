
using BencodeNET.Objects;
using BencodeNET.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TorrentClient.DHT
{
    public class DHT
    {
        private List<Node> askedNodes;
        private DHTClient _dhtClient;

        public DHT()
        {
            askedNodes = new List<Node>();
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

        public void FindPeers(Torrent torrent)
        {
            Hash hash = torrent.Info_Hash;
            IPAddress IP = IPAddress.Parse(Configuration.DHTStartIP_2);
            int port = Configuration.DHTPort;
            
            if (torrent.Nodes.Count != 0)
            {
                foreach (var node in torrent.Nodes.Values)
                {
                    if (!askedNodes.Contains(node))
                    {
                        IP = node.IP;
                        port = node.Port;

                        askedNodes.Add(node);
                        break;
                    }
                }
            }

            BDictionary response = GetPeers(hash, IP, port);
            if (response != null)
            {
                ParseGetPeersResponse(response, torrent);
            }
        }

        public BDictionary GetPeers(Hash info_hash, IPAddress nodeIP, int nodePort)
        {
            BDictionary response = null;
            //UdpClient udpClient = new UdpClient();
            try
            {
                IPEndPoint ep = new IPEndPoint(nodeIP, nodePort);//router.bittorrent.com

                _dhtClient.Connect(ep);

                var bParams = new BDictionary();
                bParams.Add("id", "abcdefghij0123456789");
                bParams.Add("info_hash", info_hash.ToString());

                BDictionary bDictionary = new BDictionary();
                bDictionary.Add("t", "aa");
                bDictionary.Add("y", "q");
                bDictionary.Add("q", "get_peers");
                bDictionary.Add("a", bParams);

                Byte[] byteRequest = bDictionary.EncodeAsBytes();
                

                _dhtClient.Send(byteRequest, byteRequest.Length);

                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                //byte[] data = _connection.Receive(ref sender);
                Byte[] receivedBytes = _dhtClient.Receive(ref sender);

                var parser = new BencodeParser();
                var bResponse = parser.Parse<BDictionary>(receivedBytes);

                Console.WriteLine("GetPeers. This is the message you received " + bResponse.ToString());
                Console.WriteLine("GetPeers. This message was sent from " + ep.Address.ToString()
                                    + " on their port number " + ep.Port.ToString());

                IBObject responseObject;
                if (bResponse.TryGetValue("r", out responseObject))
                {
                    return (BDictionary)responseObject;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            //finally
            //{
            //    _dhtClient.Close();
            //}
            return response;
        }

        private void ParseGetPeersResponse(BDictionary response, Torrent torrent)
        {
            foreach (var e in (BDictionary)response)
            {
                if (e.Key == "nodes")
                {
                    byte[] nodesRep = (byte[])((BString)e.Value).Value;
                    using (var stream = new MemoryStream(nodesRep))
                    {
                        byte[] nodeId = new byte[20];
                        IPAddress nodeIP;
                        ushort nodePort;

                        var reader = new BinaryReader(stream);
                        for (int i = 0; i < nodesRep.Length / 26; i++)
                        {
                            nodeId = reader.ReadBytes(20);
                            nodeIP = new IPAddress(reader.ReadBytes(4));
                            byte[] portBytes = reader.ReadBytes(2);

                            if (BitConverter.IsLittleEndian)
                                nodePort = BitConverter.ToUInt16(new byte[2] { (byte)portBytes[1], (byte)portBytes[0] }, 0);
                            else
                                nodePort = BitConverter.ToUInt16(new byte[2] { (byte)portBytes[0], (byte)portBytes[1] }, 0);

                            Hash nodeHash = new Hash(nodeId);
                            Node node = new Node(nodeHash, nodeIP, nodePort);
                            if (!torrent.Nodes.ContainsKey(nodeHash))
                            {
                                torrent.Nodes.Add(nodeHash, node);
                            }
                        }
                    }
                }
                if (e.Key == "id")
                {

                }
                if (e.Key == "values")
                {
                    //break;
                    var list = ((BList)e.Value).Value;
                    foreach (var element in list)
                    {

                    }

                }
                Console.WriteLine("     GetPeers response " + e.Key + ", Type: " + e.Value.GetType() + ", value: " + e.Value);
            }
        }
    }
}
