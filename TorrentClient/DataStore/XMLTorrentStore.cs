using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TorrentClient.DataStore
{
    public class XMLTorrentStore : ITorrentStore
    {
        private HashSet<Peer> _peers;
        private string _torrentId;
        public XMLTorrentStore(string torrentId, HashSet<Peer> peers)
        {
            _peers = peers;
            _torrentId = torrentId;
        }

        public void SavePeers()
        {
            var document = new XmlDocument();

            XmlNode torrentNode = document.CreateElement(StoreAttributes.TORRENT_NODE);
            XmlAttribute idAttribute = document.CreateAttribute(StoreAttributes.ID_ATTRIBUTE);
            idAttribute.Value = _torrentId;
            torrentNode.Attributes.Append(idAttribute);
            document.AppendChild(torrentNode);

            foreach (var peer in _peers)
            {
                XmlNode peerNode = document.CreateElement(StoreAttributes.PEER_NODE);

                XmlAttribute peerIP = document.CreateAttribute(StoreAttributes.IP_ATTRIBUTE);
                peerIP.Value = peer.Host.IP.ToString();

                XmlAttribute peerPort = document.CreateAttribute(StoreAttributes.PORT_ATTRIBUTE);
                peerPort.Value = peer.Host.Port.ToString();

                peerNode.Attributes.Append(peerIP);
                peerNode.Attributes.Append(peerPort);
                torrentNode.AppendChild(peerNode);
            }

            document.Save(GetPath(_torrentId));
        }


        public HashSet<Peer> ReadPeers()
        {
            XmlDocument document = new XmlDocument();
            document.Load(GetPath(_torrentId));

            HashSet<Peer> peers = new HashSet<Peer>();

            XmlNode torrentNode = document.SelectSingleNode(StoreAttributes.TORRENT_NODE+"[id=]"+_torrentId);
            var nodelist = torrentNode.SelectNodes(StoreAttributes.PEER_NODE);
            foreach (XmlNode node in nodelist) // for each <testcase> node
            {
                try
                {
                    var ip = node.Attributes.GetNamedItem(StoreAttributes.IP_ATTRIBUTE).Value;
                    var port = node.Attributes.GetNamedItem(StoreAttributes.PORT_ATTRIBUTE).Value;
                    var host = new NetHost(IPAddress.Parse(ip), ushort.Parse(port));
                    peers.Add(new Peer(host));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error in reading XML: " + e.Message);
                }
            }

            return peers;
        }

        private static string GetPath(string torrent_id)
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + string.Format("//{0}.xml", torrent_id);
        }

    }
}
