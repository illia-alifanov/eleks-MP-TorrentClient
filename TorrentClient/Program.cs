using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TorrentClient
{
    class Program
    {
        static void Main(string[] args)
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
                Byte[] sendBytes = Encoding.ASCII.GetBytes(bencoded);

                udpClient.Send(sendBytes, sendBytes.Length);

                ////IPEndPoint object will allow us to read datagrams sent from any source.
                //IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

                // Blocks until a message returns on this socket from a remote host.
                Byte[] receiveBytes = udpClient.Receive(ref ep);
                string returnData = Encoding.ASCII.GetString(receiveBytes);

                Console.WriteLine("This is the message you received " + returnData.ToString());
                Console.WriteLine("This message was sent from " + ep.Address.ToString() 
                                    + " on their port number "  + ep.Port.ToString());
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
    }
}
