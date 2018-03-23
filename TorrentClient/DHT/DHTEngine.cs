using BencodeNET.Objects;
using BencodeNET.Parsing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace TorrentClient.DHT
{
    public class DHTEngine
    {
        private Dictionary<Node, int> askedNodes;
        private Dictionary<NetHost, byte[]> dhtResponses;

        private Torrent _torrent;
        public SortedList<Hash, Node> Nodes { get; set; }
        public HashSet<Peer> Peers { get; set; }

        public DHTEngine(Torrent torrent)
        {
            _torrent = torrent;
            Nodes = new SortedList<Hash, Node>(new TorrentToNodeComparer(torrent));
            Peers = new HashSet<Peer>();
            askedNodes = new Dictionary<Node, int>();
            dhtResponses = new Dictionary<NetHost, byte[]>();
        }

        public void FindPeersWithServer()
        {
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            CancellationToken token = cancelTokenSource.Token;


            var server = new DHTServer(dhtResponses, token);
            Task dhtUDPServer = new Task(() => server.Listen());

            dhtUDPServer.Start();

            DHT dht = new DHT();
            while (Peers.Count == 0)
            {
                Node nodeToAsk = GetNextNode();

                if (nodeToAsk.ID != null)
                {
                    dht.Send_GetPeers(_torrent, nodeToAsk.Host, dhtResponses);
                }

                Thread.Sleep(2000);

                while (dhtResponses.Count > 0)
                {
                    foreach (var host in dhtResponses.Keys)
                    {
                        byte[] response;
                        if (dhtResponses.TryGetValue(host, out response))
                        {
                            var parser = new BencodeParser();
                            var bResponse = parser.Parse<BDictionary>(response);

                            IBObject responseObject;
                            if (bResponse.TryGetValue("r", out responseObject))
                            {
                                BDictionary answer = (BDictionary)responseObject;
                                ParseGetPeersResponse(answer, _torrent);
                            }
                        }
                        dhtResponses.Remove(host);
                    }
                }
            }

            cancelTokenSource.Cancel();
        }

        public void FindPeers()
        {

            DHT dht = new DHT();
            while (Peers.Count < 10)
            {
                Node nodeToAsk = GetNextNode();

                if (askedNodes.ContainsKey(nodeToAsk))
                {
                    askedNodes[nodeToAsk] += 1;
                }
                else
                {
                    askedNodes.Add(nodeToAsk, 1);
                }

                if (nodeToAsk.ID != null)
                {
                    BDictionary response = dht.GetPeers(_torrent, nodeToAsk.Host, dhtResponses);
                    if (response != null)
                    {
                        ParseGetPeersResponse(response, _torrent);
                    }
                }
            }
            
        }

        private Node GetNextNode()
        {
            Node nextNode = new Node();

            if (Nodes.Count == 0)
            {
                nextNode.ID = _torrent.Info_Hash;
                nextNode.Host = new NetHost()
                {
                    IP = IPAddress.Parse(Configuration.DHTStartIP),
                    Port = Configuration.DHTPort
                };
            }
            else
            {
                foreach (var node in Nodes.Values)
                {
                    if ( !askedNodes.ContainsKey(node)  || !dhtResponses.ContainsKey(node.Host) && askedNodes[node] < 5)
                    {
                        nextNode = node;
                        BitArray torrentID = new BitArray(_torrent.Info_Hash.Value);

                        BitArray nodeID = new BitArray(node.ID.Value);
                        BitArray distance = nodeID.Xor(torrentID);
                        Console.WriteLine("distance: " + ConvertHelper.BitArrayToBitString(distance));

                        break;
                    }
                }
            }

            return nextNode;
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
                            Node node = new Node()//nodeHash, nodeIP, nodePort
                            {
                                ID = nodeHash,
                                Host = new NetHost(nodeIP, nodePort)
                            };
                            if (!Nodes.ContainsKey(nodeHash))
                            {
                                Nodes.Add(nodeHash, node);
                            }
                        }
                    }
                }
                if (e.Key == "id")
                {

                }
                if (e.Key == "values")
                {
                    var list = ((BList)e.Value).Value;
                    foreach (var element in list)
                    {
                        byte[] peerResp = (byte[])(((BString)element).Value);
                        using (var stream = new MemoryStream(peerResp))
                        {
                            var reader = new BinaryReader(stream);

                            var peerIP = new IPAddress(reader.ReadBytes(4));
                            byte[] portBytes = reader.ReadBytes(2);

                            ushort peerPort;

                            if (BitConverter.IsLittleEndian)
                                peerPort = BitConverter.ToUInt16(new byte[2] { (byte)portBytes[1], (byte)portBytes[0] }, 0);
                            else
                                peerPort = BitConverter.ToUInt16(new byte[2] { (byte)portBytes[0], (byte)portBytes[1] }, 0);

                            Peers.Add(new Peer(new NetHost(peerIP, peerPort)));
                        }
                    }

                    // just for stop runing
                    
                }
            }
        }

    }
}
