using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Sjotterbak
{
    /// <summary>
    /// Leest en schrijft XML database bestanden
    /// maakt bij elke save een nieuw bestand aan met een prefix zodat ook
    /// de historiek beschikbaar is.
    /// </summary>
    public class RecordsReaderWriter
    {
        private readonly string _path;
        public const string Prefix = "SjotterbakDatabase-V2-";
        public RecordsReaderWriter(string path)
        {
            _path = path;

            var fileName = DetermineNewestFile(path);
            using (var fileStream = File.Open(fileName, System.IO.FileMode.Open))
            {
                Records = new XmlSerializer(typeof(Records)).Deserialize(fileStream) as Records;
            }
        }

        private string DetermineNewestFile(string path)
        {
            var directory = new DirectoryInfo(path);
            var files = directory.EnumerateFiles("*.xml", SearchOption.TopDirectoryOnly);
            var fileName = files.Where(z => z.Name.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase)).OrderByDescending(z => z.Name).FirstOrDefault()?.FullName;
            if (string.IsNullOrWhiteSpace(fileName) || File.Exists(fileName) == false || new FileInfo(fileName).Length == 0)
            {
                Records = new Records();
                Persist();
                return DetermineNewestFile(path);
            }
            return fileName;
        }

        private static string DetermineNewFileName(string path)
        {
            var proposedFileName = Path.Combine(path, $"{Prefix}{DateTime.UtcNow:yyyyMMddHHmmss}{DateTime.UtcNow.Ticks}.xml");
            if (File.Exists(proposedFileName))
            {
                return DetermineNewFileName(path);
            }
            else
            {
                return proposedFileName;
            }
        }

        public Records Records { get; private set; }

        public void Persist()
        {
            Records.MetaData.CreatedTimestamp = DateTime.UtcNow;
            using (var writeStream = File.OpenWrite(DetermineNewFileName(_path)))
            {
                new XmlSerializer(typeof(Records)).Serialize(writeStream, Records);
            }
        }
    }
}