
using BencodeNET.Objects;
using BencodeNET.Parsing;
using System;
using System.Collections;
using System.IO;
using System.Net;

namespace TorrentClient.DHT
{
    public class DHTEngine
    {
        public void FindPeers(Torrent torrent)
        {
            DHT dht = new DHT();
            while (true)
            {
                var response = dht.GetPeers(torrent.Info_Hash, Configuration.DHTStartIP, Configuration.DHTPort);
                if (response != null)
                {
                    ParseGetPeersResponse(response, torrent);
                }
            }
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
                        byte[] nodeIP = new byte[4];
                        byte[] nodePort = new byte[2];

                        var reader = new BinaryReader(stream);
                        for (int i = 0; i < nodesRep.Length/26; i++)
                        {
                            nodeId = reader.ReadBytes(20);
                            nodeIP = reader.ReadBytes(4);
                            nodePort = reader.ReadBytes(2);

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
                    break;
                }
                Console.WriteLine("     GetPeers response " + e.Key + ", Type: " + e.Value.GetType() + ", value: " + e.Value);
            }
        }
    }
}
