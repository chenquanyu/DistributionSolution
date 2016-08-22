using Microsoft.VisualStudio.TestTools.UnitTesting;
using DistributionSolution.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DistributionSolution.Utils.Tests
{
    [TestClass()]
    public class UtilTests
    {
        [TestMethod()]
        public void nextPermutationTest()
        {
            var a = new int[] { 1, 2, 3, 4 };

            Console.WriteLine(Util.GetListString(a));
            while (Util.NextPermutation(ref a, 0, 4))
            {
                Console.WriteLine(Util.GetListString(a));
            }
        }

        [TestMethod()]
        public void GetResourcePathTest()
        {
            var fileName = Path.Combine(Util.GetResourcePath(), @"DistanceInfo.json");

            var result = File.ReadAllText(fileName);

            Assert.Fail();
        }
    }
}