using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Sjotterbak
{
    public class Records
    {
        public List<Game> Games = new List<Game>();
        public List<Player> Players = new List<Player>();
        public List<Team> Teams = new List<Team>();
    }

    public class RecordsReaderWriter
    {
        private string _path;
        public RecordsReaderWriter(string path)
        {
            _path = path;
            using (var fileStream = System.IO.File.Open(path, System.IO.FileMode.OpenOrCreate))
            {
                Records = new XmlSerializer(typeof(Records)).Deserialize(fileStream) as Records ?? new Records();
            }
        }

        public Records Records { get; private set; }

        public void Persist()
        {
            using (var writeStream = System.IO.File.OpenWrite(_path))
            {
                new XmlSerializer(typeof(Records)).Serialize(writeStream, Records);
            }
        }
    }

}
