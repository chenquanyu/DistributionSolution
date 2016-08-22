using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributionSolution.Model
{
    //整个配送方案,一天
    public class DistributionPlan
    {
        public DistributionPlan()
        {
            DistributionPaths = new List<DistributionPath>();
        }

        public List<DistributionPath> DistributionPaths { get; set; }

        public void AddPath(DistributionPath path)
        {
            DistributionPaths.Add(path);
        }

        public decimal TotalPathLength
        {
            get
            {
                decimal result = 0;

                foreach (var path in DistributionPaths)
                {
                    result += path.TotalPathLength;
                }

                return result;
            }
        }

        ////计算方案总费用
        //public decimal TotalCost
        //{
        //    get
        //    {
        //        decimal result = 0;

        //        foreach (var path in DistributionPaths)
        //        {
        //            var unitCost = path.Van.UnitCost;

        //            //某一辆车的路线
        //            for (int i = 1; i < path.Nodes.Count; i++)
        //            {
        //                //前一个节点到下一个的距离
        //                decimal distance = path.Nodes[i - 1].Warehouse.Location.GetDistince(path.Nodes[i].Warehouse.Location);
        //                result += unitCost * distance;
        //            }
        //        }

        //        return result;
        //    }
        //}

    }

    //单个货车的配送路线,起点和终点都是公司
    public class DistributionPath
    {
        //公司
        public Company Company { get; set; }

        //配送车辆
        public Van Van { get; set; }

        //配送节点
        public List<DistributionNode> Nodes { get; set; }

        public DistributionPath(Company company, Van van, List<DistributionNode> nodes)
        {
            Company = company;
            Van = van;
            Nodes = nodes;
        }

        /// <summary>
        /// 从公司出发并返回公司的总路程
        /// </summary>
        public decimal TotalPathLength
        {
            get
            {
                if (Nodes.Count == 0)
                {
                    return 0;
                }

                decimal result = Company.Warehouse.Location.GetDistince(Nodes[0].Warehouse.Location);

                //某一辆车的路线
                for (int i = 1; i < Nodes.Count; i++)
                {
                    //前一个节点到下一个的距离
                    decimal distance = Nodes[i - 1].Warehouse.Location.GetDistince(Nodes[i].Warehouse.Location);
                    result += distance;
                }

                result += Nodes.Last().Warehouse.Location.GetDistince(Company.Warehouse.Location);
                return result;
            }
        }

        public decimal DistributionAmount
        {
            get
            {
                decimal result = 0;
                foreach (var node in Nodes)
                {
                    result += node.DistributionAmount;
                }
                return result;

            }
        }

    }

    //单个配送节点
    public class DistributionNode
    {
        public DistributionNode(Warehouse warehouse)
        {
            Warehouse = warehouse;
        }

        //配送目标仓库
        public Warehouse Warehouse { get; set; }

        //配送数量
        public decimal DistributionAmount
        {
            get
            {
                return Warehouse.ChargeAmount;
            }
        }
    }

}
