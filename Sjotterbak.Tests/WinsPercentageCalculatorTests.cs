using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sjotterbak.Ranking.EasyStats;

namespace Sjotterbak.Tests
{
    [TestClass]
    public class WinsPercentageCalculatorTests
    {
        [TestMethod]
        public void Percentage_Tests()
        {
            var calc = new WinsPercentageCalculator();
            var data = new Records();
            var timm = data.AddPlayer("TimM");
            var timv = data.AddPlayer("TimV");
            var chris = data.AddPlayer("Chris");
            var nick = data.AddPlayer("Nick");
            var dieter = data.AddPlayer("Dieter");

            var ranking = calc.DetermineRanking(data);


        }
    }
}
