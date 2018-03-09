using System.Configuration;

namespace TorrentClient
{
    public class Configuration
    {
        public static string DHTStartIP => ConfigurationManager.AppSettings["DHTStartIP"];

        public static string DHTStartIP_2 => ConfigurationManager.AppSettings["DHTStartIP_2"];

        public static ushort DHTPort = 6881;
    }
}
