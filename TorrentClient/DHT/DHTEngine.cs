using BencodeNET.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace TorrentClient.DHT
{
    public class DHTEngine
    {
        private List<Node> askedNodes;
        private Torrent _torrent;
        public SortedList<Hash, Node> Nodes { get; set; }
        public List<Peer> Peers { get; set; }

        public DHTEngine(Torrent torrent)
        {
            _torrent = torrent;
            Nodes = new SortedList<Hash, Node>(new TorrentToNodeComparer(torrent));
            Peers = new List<Peer>();
            askedNodes = new List<Node>();
        }

        public void FindPeers()
        {
            DHT dht = new DHT();
            while (Peers.Count == 0)
            {
                Node nodeToAsk = GetNextNode();

                BDictionary response = dht.GetPeers(_torrent, nodeToAsk.IP, nodeToAsk.Port);
                if (response != null)
                {
                    ParseGetPeersResponse(response, _torrent);
                }
            }
        }

        private Node GetNextNode()
        {
            Node nextNode = new Node();

            if (Nodes.Count == 0)
            {
                nextNode.ID = _torrent.Info_Hash;
                nextNode.IP = IPAddress.Parse(Configuration.DHTStartIP);
                nextNode.Port = Configuration.DHTPort;
            }
            else
            {
                foreach (var node in Nodes.Values)
                {
                    if (!askedNodes.Contains(node))
                    {
                        nextNode = node;
                        askedNodes.Add(nextNode);

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
                            Node node = new Node(nodeHash, nodeIP, nodePort);
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
                    }

                    // just for stop runing
                    Peers.Add(new Peer(new byte[1], new byte[1]));
                    break;
                }
            }
        }
       
    }
}
