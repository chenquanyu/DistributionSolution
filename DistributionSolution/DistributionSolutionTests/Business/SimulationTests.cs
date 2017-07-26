using Microsoft.VisualStudio.TestTools.UnitTesting;
using DistributionSolution.Business;
using DistributionSolution.Model;
using DistributionSolution.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace DistributionSolution.Business.Tests
{
    [TestClass()]
    public class SimulationTests
    {
        [TestMethod()]
        public void SimulationByDaysTest()
        {

            var vans = new List<Van> {
                new Van ("A", 6000, 1),
                new Van ("B", 6000, 1),
                new Van ("C", 10000, 1),
                new Van ("D", 10000, 1),
                new Van ("E", 18000, 1),
            };

            List<Van> tempVans = new List<Van>(vans);
            var VansPermutation = new List<List<Van>>();
            Util.Permutation(vans, 5, tempVans, VansPermutation);

            //工厂地址
            var companyLocation = new Location { x = 121.15638m, y = 31.157769m };

            Company company = new Company
            {
                Name = "青浦工厂",
                Vans = vans,
                Warehouse = new Warehouse("青浦工厂", Decimal.MaxValue, decimal.MaxValue, 0, companyLocation)
            };

            List<Warehouse> warehouses = GetWarehousesFromExcel();

            var partitions = Calculate.GetPartition(warehouses, 5);

            //过滤不合理的划分
            partitions = Calculate.FilterPartitions(partitions);

            decimal shortestDistince = Decimal.MaxValue;
            Simulation bestSim = null;
            
            List<string> visitedVanListKey = new List<string>();
            //对货车顺序全排列
            foreach (var item in VansPermutation)
            {
                //由于货车容量相同时计算结果相同，所以应该避免重复计算
                var vansKey = GetKeyForVans(item);
                if (visitedVanListKey.Contains(vansKey))
                {
                    continue;
                }
                else
                {
                    visitedVanListKey.Add(vansKey);
                }

                //处理划分
                foreach (var part in partitions)
                {
                    //过滤不合理的划分方案
                    if (!Calculate.IsPartitionValid(part, item))
                    {
                        continue;
                    };
                    
                    company.Vans = item;

                    var sim = new Simulation(company, warehouses, part, 10);

                    if (sim.SimulationByDays() && sim.TotalDistance < shortestDistince)
                    {
                        shortestDistince = sim.TotalDistance;
                        bestSim = sim;
                    }
                    
                }
            }

            WriteResult(bestSim.GetLog());
        }

        [TestMethod()]
        public void OneSimulationByDaysTest()
        {
            var vans = new List<Van> {
                new Van ("A", 6000, 1),
                new Van ("B", 6000, 1),
                new Van ("C", 10000, 1),
                new Van ("D", 10000, 1),
                new Van ("E", 18000, 1),
            };

            List<Van> tempVans = new List<Van>(vans);
            var VansPermutation = new List<List<Van>>();
            Util.Permutation(vans, 5, tempVans, VansPermutation);

            //工厂地址
            var companyLocation = new Location { x = 121.15638m, y = 31.157769m };

            Company company = new Company
            {
                Name = "青浦工厂",
                Vans = vans,
                Warehouse = new Warehouse("青浦工厂", Decimal.MaxValue, decimal.MaxValue, 0, companyLocation)
            };

            List<Warehouse> warehouses = GetWarehousesFromExcel();

            var partitions = Calculate.GetPartition(warehouses, 5);

            //过滤不合理的划分
            partitions = Calculate.FilterPartitions(partitions);

            decimal shortestDistince = Decimal.MaxValue;
            Simulation bestSim = null;

            List<string> visitedVanListKey = new List<string>();
            //对货车顺序全排列
            foreach (var item in VansPermutation)
            {
                //由于货车容量相同时计算结果相同，所以应该避免重复计算
                var vansKey = GetKeyForVans(item);
                if (visitedVanListKey.Contains(vansKey))
                {
                    continue;
                }
                else
                {
                    visitedVanListKey.Add(vansKey);
                }

                //处理划分
                foreach (var part in partitions)
                {
                    //过滤不合理的划分方案
                    if (!Calculate.IsPartitionValid(part, item))
                    {
                        continue;
                    };

                    company.Vans = item;

                    var sim = new Simulation(company, warehouses, part, 30);

                    if (sim.SimulationByDays() && sim.TotalDistance < shortestDistince)
                    {
                        shortestDistince = sim.TotalDistance;
                        bestSim = sim;
                    }
                }
            }

            WriteResult(bestSim.GetLog());

        }

        private List<Warehouse> GetWarehousesFromExcel()
        {
            List<Warehouse> result = new List<Warehouse>();
            string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resource\路线规划.xlsx");
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
                    (0.6m * capacity - dailyUsage) * (decimal)random.NextDouble(), dailyUsage, new Location { x = x, y = y }));
            }
            return result;
        }

        private void WriteResult(string result)
        {
            string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"result.txt");
            File.WriteAllText(fileName, result);
        }

        //由于货车容量相同时计算结果相同，所以应该避免重复计算
        private string GetKeyForVans(List<Van> vans)
        {
            StringBuilder key = new StringBuilder("VanList");
            vans.ForEach(p => key.Append($"-{p.Capacity}"));
            return key.ToString();
        }
    }
}