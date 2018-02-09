
using BencodeNET.Objects;
using BencodeNET.Parsing;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TorrentClient.DHT
{
    public class DHT
    {
        public string Ping()
        {
            BString nodeId = "";

            // This constructor arbitrarily assigns the local port number.
            UdpClient udpClient = new UdpClient();
            try
            {
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(Configuration.DHTStartIP), Configuration.DHTPort);//router.bittorrent.com
                //IPEndPoint ep = new IPEndPoint(IPAddress.Parse("82.221.103.244"), 6881);//router.utorrent.com

                udpClient.Connect(ep);

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
                udpClient.Send(byteRequest, byteRequest.Length);

                // Blocks until a message returns on this socket from a remote host.
                Byte[] receivedBytes = udpClient.Receive(ref ep);
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
                udpClient.Close();
                //Console.Write("Press any key to exit...");
                //Console.ReadKey();
            }

            return nodeId.ToString(Encoding.UTF8);
        }
        public BDictionary GetPeers(string info_hash, string nodeIP, int nodePort)
        {
            BDictionary response = null;
            UdpClient udpClient = new UdpClient();
            try
            {
                //string nodeId = this.Ping();
                //if (!string.IsNullOrEmpty(nodeId))
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(nodeIP), nodePort);//router.bittorrent.com

                udpClient.Connect(ep);

                // Sends a message to the host to which you have connected.
                string bencoded = "d1:ad2:id20:abcdefghij0123456789e1:q4:ping1:t2:aa1:y1:qe";
                Byte[] sendBytes = Encoding.UTF8.GetBytes(bencoded);

                var bParams = new BDictionary();
                bParams.Add("id", "abcdefghij0123456789");
                bParams.Add("info_hash", info_hash);

                BDictionary bDictionary = new BDictionary();
                bDictionary.Add("t", "aa");
                bDictionary.Add("y", "q");
                bDictionary.Add("q", "get_peers");
                bDictionary.Add("a", bParams);

                Byte[] byteRequest = bDictionary.EncodeAsBytes();
                //udpClient.Client.ReceiveTimeout = TimeSpan.FromSeconds(2).Seconds;

                udpClient.Send(byteRequest, byteRequest.Length);
                Byte[] receivedBytes = udpClient.Receive(ref ep);

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
            finally
            {
                udpClient.Close();
            }
            return response;
        }
    }
}
