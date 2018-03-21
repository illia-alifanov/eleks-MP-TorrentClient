
using System;

namespace TorrentClient.DataStore
{
    public class XMLReader
    {
        public object ReadXML(string fileName, string type)
        {
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(Type.GetType(type));
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + string.Format("//{0}.xml", fileName);
            System.IO.StreamReader file = new System.IO.StreamReader(path);
            object obj = reader.Deserialize(file);
            file.Close();
            return obj;
        }
    }
}
