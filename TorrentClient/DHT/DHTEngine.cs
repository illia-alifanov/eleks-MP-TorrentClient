
using BencodeNET.Objects;
using BencodeNET.Parsing;
using System;
using System.Collections;
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
                    for (int i = 0; i < nodesRep.Length; i = i + 26)
                    {
                        byte[] nodeInfo = new byte[26];
                        byte[] nodeId = new byte[20];
                        byte[] nodeIP = new byte[4];
                        byte[] nodePort = new byte[2];

                        Array.Copy(nodesRep, i, nodeInfo, 0, 26);
                        Array.Copy(nodeInfo, nodeId, 20);
                        Array.Copy(nodesRep, 20, nodeIP, 0, 4);
                        Array.Copy(nodesRep, 24, nodePort, 0, 2);

                        //new IPAddress(nodeId);
                        //BitConverter.ToInt32(nodePort, 0);
                        //Array.Reverse(nodePort, 0, nodePort.Length);
                        Node node = new Node(nodeId, nodeIP, nodePort);
                        torrent.Nodes.Add(node);
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
