using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sjotterbak.Tests
{
    [TestClass]
    public class RecordsReaderWriterTests
    {
        [TestMethod]
        public void Test_Read_Write_Read()
        {
            var tempPath = Path.GetTempPath();

            var tempFilesFromEarlierTests = new DirectoryInfo(tempPath).EnumerateFiles("*.xml", SearchOption.TopDirectoryOnly)
                .Where(z => z.Name.StartsWith(RecordsReaderWriter.Prefix)).ToArray();
            foreach (var tempFilesFromEarlierTest in tempFilesFromEarlierTests)
            {
                System.IO.File.Delete(tempFilesFromEarlierTest.FullName);
            }

            { // Write
                var readerWriter = new RecordsReaderWriter(tempPath);

                readerWriter.Records.Players.Add(new Player()
                {
                    CreatedTimestamp = new DateTime(2001, 1, 1),
                    Id = new PlayerId()
                    {
                        Value = 1
                    },
                    Name = "Tim M"
                });
                readerWriter.Records.Players.Add(new Player()
                {
                    CreatedTimestamp = new DateTime(2003, 1, 1),
                    Id = new PlayerId()
                    {
                        Value = 2
                    },
                    Name = "Tim V"
                });
                readerWriter.Records.Teams.Add(new Team()
                {
                    Id = new TeamId()
                    {
                        Value = 1
                    },
                    CreatedTimestamp = new DateTime(2009, 1, 1),
                    PlayerOne = readerWriter.Records.Players.First().Id,
                    PlayerTwo = readerWriter.Records.Players.Skip(1).First().Id
                });

                readerWriter.Persist();
            }
            { // Read
                var readerWriter = new RecordsReaderWriter(tempPath);
                Assert.AreEqual(2, readerWriter.Records.Players.Count);
                Assert.AreEqual("Tim M", readerWriter.Records.Players.First().Name);
                Assert.AreEqual(1, readerWriter.Records.Players.First().Id.Value);
                Assert.AreEqual(1, readerWriter.Records.Teams.Count);
            }
        }
    }
}
