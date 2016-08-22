using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributionSolution.Utils;
using DistributionSolution.Model;

namespace DistributionSolution.Business
{
    public class Simulation
    {
        private int Days { get; set; }

        public Company Company { get; set; }

        private List<Warehouse> Warehouses { get; set; }

        public List<List<Warehouse>> Partition { get; set; }

        public decimal TotalDistance { get; set; }
        public List<DistributionPlan> PlanHistory { get; set; }
        public List<List<Warehouse>> StatusHistory { get; set; }

        public Simulation(Company company, List<Warehouse> warehouses, List<List<Warehouse>> partition, int days)
        {
            Days = days;
            TotalDistance = 0;
            Company = company;
            Warehouses = warehouses;
            Partition = partition;
            //将初始状态记录进历史
            StatusHistory = new List<List<Warehouse>> { DeepCopy(Warehouses) };
            PlanHistory = new List<DistributionPlan>();
        }

        //执行一天配送方案，更新Company，Customers的库存状态并记录历史
        public void ActionAsPlaned(DistributionPlan plan)
        {
            //对所有方案中的仓库补货
            foreach (var path in plan.DistributionPaths)
            {
                foreach (var node in path.Nodes)
                {
                    node.Warehouse.Stock = node.Warehouse.Capacity * 0.9m;
                }
            }

            foreach (var ware in Warehouses)
            {
                ware.Stock -= ware.DailyUsage;
            }

            TotalDistance += plan.TotalPathLength;
            StatusHistory.Add(DeepCopy(Warehouses));
            PlanHistory.Add(plan);
        }

        private List<Warehouse> DeepCopy(List<Warehouse> wares)
        {
            var result = new List<Warehouse>();
            foreach (var ware in wares)
            {
                result.Add(ware.Clone());
            }
            return result;
        }

        //模拟days天数的情况并记录历史
        public bool SimulationByDays()
        {
            if (Days < 1)
            {
                return false;
            }

            for (int i = 0; i < Days; i++)
            {
                var plan = Calculate.CalculatePlanWithOnePartition(Company, Partition, Company.Vans);
                if (plan == null)
                {
                    return false;
                }
                ActionAsPlaned(plan);
            }

            //将仓库状态还原
            Warehouses = DeepCopy(StatusHistory[0]);

            return true;

        }

        public string GetLog()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"将仓库划分为5条线路的最短路程为{TotalDistance}m");
            sb.AppendLine("最优划分方案为：");
            for (int i = 0; i < 5; i++)
            {
                var van = Company.Vans[i];
                var set = Partition[i];
                sb.Append($"线路{i+1}: 货车 {van.Name},{van.Capacity}Kg  ");
                foreach (var ware in set)
                {
                    sb.Append(ware.Name + ",");
                }
                sb.AppendLine();
            }

            for (int i = 0; i < Days; i++)
            {
                var plan = PlanHistory[i];
                sb.AppendLine($"第{i+1}天：");
                sb.AppendLine($"方案(总里程 {plan.TotalPathLength}m)：");

                foreach (var path in plan.DistributionPaths)
                {
                    sb.AppendLine($"货车: {path.Van.Name},{path.Van.Capacity} 配送量: {path.DistributionAmount}, 配送里程: {path.TotalPathLength}");
                    sb.Append(Company.Name);
                    path.Nodes.ForEach(p => sb.Append($" -> {p.Warehouse.Name}"));
                    sb.AppendLine($" -> {Company.Name}");
                }
                sb.AppendLine();

                sb.AppendLine("各仓库状态：");
                var wares = StatusHistory[i];
                wares.ForEach(p => sb.AppendLine($"名称:{p.Name}，容量:{p.Capacity}Kg，库存:{p.Stock}Kg，库存百分比：{p.StockPercent.ToString("f2")}"));

                sb.AppendLine();
            }

            return sb.ToString();
        }

    }
}
