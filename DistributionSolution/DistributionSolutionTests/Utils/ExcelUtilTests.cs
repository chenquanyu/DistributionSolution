using Microsoft.VisualStudio.TestTools.UnitTesting;
using DistributionSolution.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributionSolution.Utils.Tests
{
    [TestClass()]
    public class ExcelUtilTests
    {
        [TestMethod()]
        public void ReadSheetTest()
        {
            string fileName = @"D:\路线规划.xlsx";
            var result = ExcelUtil.ReadSheet(fileName, 1, "A3", "D29");
        }
    }
}