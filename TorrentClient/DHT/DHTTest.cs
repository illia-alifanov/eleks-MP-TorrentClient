using System;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using BencodeNET.Objects;
using System.Collections.Generic;
using BencodeNET.Parsing;

namespace TorrentClient.DHT
{
    public class DHTTest
    {
        public string Ping()
        {
            BString nodeId = "";

            // This constructor arbitrarily assigns the local port number.
            UdpClient udpClient = new UdpClient();
            try
            {
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse("67.215.246.10"), 6881);//router.bittorrent.com
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
                
                udpClient.Send(byteRequest, byteRequest.Length);

                // Blocks until a message returns on this socket from a remote host.
                Byte[] receivedBytes = udpClient.Receive(ref ep);
                var parser = new BencodeParser();
                var bResponse = parser.Parse<BDictionary>(receivedBytes);
                
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

        public void FindNode(string targetId)
        {
            // This constructor arbitrarily assigns the local port number.
            UdpClient udpClient = new UdpClient();
            try
            {
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse("67.215.246.10"), 6881);//router.bittorrent.com
                //IPEndPoint ep = new IPEndPoint(IPAddress.Parse("82.221.103.244"), 6881);//router.utorrent.com

                udpClient.Connect(ep);

                BDictionary bdictionary = new BDictionary { { "id", "abcdefghij0123456789e1" }, {"target", targetId } };
                Byte[] byteRequest = bdictionary.EncodeAsBytes();

                udpClient.Send(byteRequest, byteRequest.Length);

                // Blocks until a message returns on this socket from a remote host.
                Byte[] receivedBytes = udpClient.Receive(ref ep);
                var parser = new BencodeParser();

                var bstring = parser.Parse<BString>(receivedBytes);
                string returnData = bstring.ToString();
                
                Console.WriteLine("This is the message you received " + returnData.ToString());
                Console.WriteLine("This message was sent from " + ep.Address.ToString()
                                    + " on their port number " + ep.Port.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                udpClient.Close();
                Console.Write("Press any key to exit...");
                Console.ReadKey();
            }
        }

        public void GetPeers()
        {
            // This constructor arbitrarily assigns the local port number.
            UdpClient udpClient = new UdpClient();
            try
            {
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse("67.215.246.10"), 6881);//router.bittorrent.com
                //IPEndPoint ep = new IPEndPoint(IPAddress.Parse("82.221.103.244"), 6881);//router.utorrent.com

                udpClient.Connect(ep);

                // Sends a message to the host to which you have connected.
                string bencoded = "d1:ad2:id20:abcdefghij0123456789e1:q4:ping1:t2:aa1:y1:qe";
                Byte[] sendBytes = Encoding.UTF8.GetBytes(bencoded);

                var bParams = new BDictionary();
                bParams.Add("id", "abcdefghij0123456789");
                bParams.Add("info_hash", "274F73F6F428B51FD475C2C499917100ACA8F1D5");

                BDictionary bDictionary = new BDictionary();
                bDictionary.Add("t", "aa");
                bDictionary.Add("y", "q");
                bDictionary.Add("q", "get_peers");
                bDictionary.Add("a", bParams);

                Byte[] byteRequest = bDictionary.EncodeAsBytes();

                udpClient.Send(byteRequest, byteRequest.Length);

                // Blocks until a message returns on this socket from a remote host.
                Byte[] receivedBytes = udpClient.Receive(ref ep);
                var parser = new BencodeParser();
                var bResponse = parser.Parse<BDictionary>(receivedBytes);

                IBObject response;
                if (bResponse.TryGetValue("r", out response))
                {
                    //nodeId = ((BDictionary)responseNode).Get<BString>("id");
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
        }
    }
}
