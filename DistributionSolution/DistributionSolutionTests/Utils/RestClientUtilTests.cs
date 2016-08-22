using Microsoft.VisualStudio.TestTools.UnitTesting;
using DistributionSolution.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributionSolution.Model;
using System.IO;

namespace DistributionSolution.Utils.Tests
{
    [TestClass()]
    public class RestClientUtilTests
    {
        [TestMethod()]
        public void GetDistanceTest()
        {
            var path = @"D:\DistanceInfo.json";
            var warehouses = GetWarehousesFromExcel();
            var locations = warehouses.Select(p => p.Location).ToList();

            //工厂地址
            locations.Insert(0, new Location { x = 121.15638m, y = 31.157769m });

            Dictionary<string, decimal> distanceInfo = new Dictionary<string, decimal>();
            for (int i = 0; i < locations.Count; i++)
            {
                for (int j = 0; j < locations.Count; j++)
                {
                    var key = $"DISTANCE-{locations[i].x}-{locations[i].y}-{locations[j].x}-{locations[j].y}";
                    var value = RestClientUtil.GetDistance(locations[i], locations[j]);
                    distanceInfo.Add(key, value);
                }
            }

            File.WriteAllText(path, JsonUtil.Serialize(distanceInfo));

            //var result = RestClientUtil.GetDistance(new Location { x = 121.15638m, y = 31.157769m },
            //    new Location { x = 121.486809m, y = 31.27076m });

            //Assert.AreNotEqual(0, result);
        }

        private List<Warehouse> GetWarehousesFromExcel()
        {
            List<Warehouse> result = new List<Warehouse>();
            string fileName = @"D:\路线规划.xlsx";
            var temp = ExcelUtil.ReadSheet(fileName, 1, "A3", "D29");
            Random random = new Random();
            foreach (var row in temp)
            {
                decimal x = 0;
                decimal y = 0;
                decimal dailyUsage = 0;
                decimal capacity = 0;
                string wareName = row[0];
                Decimal.TryParse(row[1].Split(',')[0], out x);
                Decimal.TryParse(row[1].Split(',')[1], out y);
                Decimal.TryParse(row[2], out dailyUsage);
                Decimal.TryParse(row[3], out capacity);
                result.Add(new Warehouse(wareName, capacity, capacity * 0.3m + dailyUsage +
                    0.2m * capacity * (decimal)random.NextDouble(), dailyUsage, new Location { x = x, y = y }));
            }
            return result;
        }

    }
}