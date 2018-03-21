using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrentClient.DataStore
{
    public class XMLWriter
    {
        public static void WriteXML(Object obj, string id)
        {
            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(obj.GetType());

            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + string.Format("//{0}.xml", id);
            System.IO.FileStream file = System.IO.File.Create(path);

            writer.Serialize(file, obj);
            file.Close();
        }
    }
}
