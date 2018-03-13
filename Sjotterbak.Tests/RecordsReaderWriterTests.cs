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

                readerWriter.Records.Games.Add(new Game()
                {
                    Team1Player1 = new Player("TimM"),
                    Team1Player2 = new Player("TimV"),
                    Team2Player1 = new Player("Chris"),
                    Team2Player2 =  new Player("Jeremy")
                });

                readerWriter.Persist();
            }
            { // Read
                var readerWriter = new RecordsReaderWriter(tempPath);
                Assert.AreEqual(1, readerWriter.Records.Games.Count);
                Assert.AreEqual("TimM", readerWriter.Records.GetPlayers().First().Name);
                Assert.AreEqual(4, readerWriter.Records.GetPlayers().Count());
            }
        }
    }
}
