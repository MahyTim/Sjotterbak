using System.IO;
using System.Xml.Serialization;

namespace Sjotterbak
{
    public class RecordsReaderWriter
    {
        private readonly string _path;
        public RecordsReaderWriter(string path)
        {
            _path = path;
            if (File.Exists(path) == false || new FileInfo(_path).Length == 0)
            {
                Records = new Records();
                Persist();
            }
            using (var fileStream = File.Open(path, System.IO.FileMode.Open))
            {
                Records = new XmlSerializer(typeof(Records)).Deserialize(fileStream) as Records;
            }
        }

        public Records Records { get; private set; }

        public void Persist()
        {
            using (var writeStream = File.OpenWrite(_path))
            {
                new XmlSerializer(typeof(Records)).Serialize(writeStream, Records);
            }
        }
    }
}