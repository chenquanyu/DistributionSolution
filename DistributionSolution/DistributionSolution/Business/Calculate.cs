using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributionSolution.Model;
using DistributionSolution.Utils;
using System.Runtime.Caching;

namespace DistributionSolution.Business
{
    public static class Calculate
    {
        /// <summary>
        /// 过滤第3天存量会少于30%的仓库 
        /// </summary>
        /// <param name="warehouses"></param>
        /// <returns></returns>
        public static List<Warehouse> FilterWaresNeedCharge(List<Warehouse> warehouses)
        {
            List<Warehouse> result = new List<Warehouse>();
            foreach (var ware in warehouses)
            {
                if (ware.IsNeedCharge())
                {
                    result.Add(ware);
                }
            }
            return result;
        }

        /// <summary>
        /// 计算仓库列表所需总重量
        /// </summary>
        /// <param name="warehouses"></param>
        /// <returns></returns>
        public static decimal GetTotalNeedWeight(List<Warehouse> warehouses)
        {
            decimal result = 0;
            foreach (var ware in warehouses)
            {
                result += ware.ChargeAmount;
            }
            return result;
        }

        /// <summary>
        /// 计算仓库列表每天的总消耗量
        /// </summary>
        /// <param name="warehouses"></param>
        /// <returns></returns>
        public static decimal GetTotalDailyUsage(List<Warehouse> warehouses)
        {
            decimal result = 0;
            foreach (var ware in warehouses)
            {
                result += ware.DailyUsage;
            }
            return result;
        }


        //计算一条路径的总长度，公司-仓库1-仓库2-……-仓库n-公司
        public static decimal GetPathLenght(Company company, List<Warehouse> warehouses)
        {
            if (warehouses.Count == 0)
            {
                return 0;
            }

            decimal result = company.Warehouse.Location.GetDistince(warehouses[0].Location);
            for (int i = 1; i < warehouses.Count; i++)
            {
                result += warehouses[i - 1].Location.GetDistince(warehouses[i].Location);
            }
            result += warehouses.Last().Location.GetDistince(company.Warehouse.Location);
            return result;
        }

        /// <summary>
        /// 使用全排列获取 经过n个仓库的最短路径,使用动态规划优化
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public static List<Warehouse> GetShortestPath(Company company, List<Warehouse> set)
        {
            if (set.Count == 0)
            {
                return set;
            }

            var cache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy()
            {
                AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(30))
            };

            var keyBuilder = new StringBuilder("ShorestPath");
            set.OrderBy(p => p.Name).Select(p => p.Name).ToList().ForEach(p => keyBuilder.Append($"-{p}"));
            var key = keyBuilder.ToString();
            var pathLengthKey = key.Replace("ShorestPath", "ShorestPathLength");

            var value = cache[key];
            if (value != null)
            {
                return value as List<Warehouse>;
            }

            int count = set.Count;
            List<Warehouse> temp = new List<Warehouse>(set);
            List<Warehouse> best = new List<Warehouse>();
            decimal minPath = Decimal.MaxValue;
            PermutationShortest(company, set, count, temp, ref best, ref minPath);

            cache.Set(key, best, policy);
            cache.Set(pathLengthKey, minPath, policy);
            return best;
        }

        /// <summary>
        /// 针对一条路线获取最短里程
        /// </summary>
        /// <param name="company"></param>
        /// <param name="set"></param>
        /// <returns></returns>
        public static decimal GetShortestPathLength(Company company, List<Warehouse> set)
        {
            var cache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy()
            {
                AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(30))
            };

            var keyBuilder = new StringBuilder("ShorestPathLength");
            set.OrderBy(p => p.Name).Select(p => p.Name).ToList().ForEach(p => keyBuilder.Append($"-{p}"));
            var key = keyBuilder.ToString();

            var value = cache[key];
            if (value != null)
            {
                return (decimal)value;
            }

            return GetPathLenght(company, GetShortestPath(company, set));
        }

        /// <summary>
        /// 使用全排列获取最短路径
        /// </summary>
        private static void PermutationShortest(Company company, List<Warehouse> nodes, int count, List<Warehouse> temp, ref List<Warehouse> best, ref decimal minPath)
        {
            foreach (var node in nodes)
            {
                int a = nodes.Count();
                //将第count - nodes.Count()个元素放入临时列表
                temp[count - a] = node;
                var spare = new List<Warehouse>(nodes);
                spare.Remove(node);

                if (spare.Count() == 0)
                {
                    //将完成的排列存入结果集
                    var length = GetPathLenght(company, temp);
                    if (length < minPath)
                    {
                        best = new List<Warehouse>(temp);
                        minPath = length;
                    }
                    return;
                }
                PermutationShortest(company, spare, count, temp, ref best, ref minPath);
            }
        }

        /// <summary>
        /// 针对固定的划分和固定的顺序计算分配方案，不合理的方案直接返回null
        /// 由于2次配送复杂度太高，暂时注释
        /// </summary>
        /// <param name="partition"></param>
        /// <param name="vans"></param>
        /// <returns></returns>
        //public static DistributionPlan CalculatePlanWithOnePartition(Company company, List<List<Warehouse>> partition, List<Van> vans)
        //{
        //    if (partition.Count != vans.Count || partition.Count == 0)
        //    {
        //        throw new ArgumentException("划分的子集数量必须与货车数量相等");
        //    }

        //    DistributionPlan result = new DistributionPlan();

        //    for (int i = 0; i < partition.Count; i++)
        //    {
        //        var warehouses = partition[i];
        //        var van = vans[i];

        //        //计算需要补货的仓库
        //        var chargeWares = Calculate.FilterWaresNeedCharge(warehouses);

        //        if (chargeWares.Count == 0)
        //        {
        //            continue;
        //        }

        //        //Todo:增加减少空载率的逻辑，后期优化

        //        //需求量超过2次运输的容量，方案无效
        //        var weightNeeded = Calculate.GetTotalNeedWeight(chargeWares);
        //        if (weightNeeded > van.Capacity * 2)
        //        {
        //            return null;
        //        }

        //        //获取配送点的最短路径

        //        //一次配送即可完成时
        //        if (weightNeeded <= van.Capacity)
        //        {
        //            chargeWares = Calculate.GetShortestPath(company, chargeWares);

        //            var nodes = new List<DistributionNode>();
        //            for (int j = 0; j < chargeWares.Count; j++)
        //            {
        //                var warehouse = chargeWares[j];
        //                nodes.Add(new DistributionNode(warehouse));
        //            }

        //            var path = new DistributionPath(company, van, nodes);
        //            result.AddPath(path);
        //        }
        //        else
        //        {
        //            //需要两次配送，划分处理
        //            var partList = Util.SetPartition(chargeWares, 2);

        //            decimal shorestPath = Decimal.MaxValue;
        //            DistributionPath firstPath = null;
        //            DistributionPath secondPath = null;

        //            foreach (var part in partList)
        //            {
        //                var set1 = part[0];
        //                var set2 = part[1];

        //                decimal weight1 = Calculate.GetTotalNeedWeight(set1);
        //                decimal weight2 = Calculate.GetTotalNeedWeight(set2);

        //                //过滤不合理的划分
        //                if (weight1 > van.Capacity || weight2 > van.Capacity)
        //                {
        //                    continue;
        //                }

        //                var path1 = new DistributionPath(company, van,
        //                    Calculate.GetDistributionNodeList(Calculate.GetShortestPath(company, set1)));
        //                var path2 = new DistributionPath(company, van,
        //                    Calculate.GetDistributionNodeList(Calculate.GetShortestPath(company, set2)));

        //                if (!path1.IsValid() || !path2.IsValid())
        //                {
        //                    continue;
        //                }

        //                decimal length1 = path1.TotalPathLength;
        //                decimal length2 = path2.TotalPathLength;

        //                if (length1 + length2 < shorestPath)
        //                {
        //                    shorestPath = length1 + length2;
        //                    firstPath = path1;
        //                    secondPath = path2;
        //                }
        //            }

        //            //找不到合理的划分，说明整个方案失败
        //            if (firstPath == null || secondPath == null)
        //            {
        //                return null;
        //            }

        //            result.AddPath(firstPath);
        //            result.AddPath(secondPath);
        //        }

        //    }

        //    return result;
        //}

        /// <summary>
        /// 针对固定的划分和固定的顺序计算分配方案，不合理的方案直接返回null
        /// </summary>
        /// <param name="partition"></param>
        /// <param name="vans"></param>
        /// <returns></returns>
        public static DistributionPlan CalculatePlanWithOnePartition(Company company, List<List<Warehouse>> partition, List<Van> vans)
        {
            if (partition.Count != vans.Count || partition.Count == 0)
            {
                throw new ArgumentException("划分的子集数量必须与货车数量相等");
            }

            DistributionPlan result = new DistributionPlan();

            for (int i = 0; i < partition.Count; i++)
            {
                var set = partition[i];
                var van = vans[i];

                //计算必须补货的仓库
                var chargeWares = Calculate.FilterWaresNeedCharge(set);

                //需求量超过1次运输的容量，方案无效
                var weightNeeded = Calculate.GetTotalNeedWeight(chargeWares);
                if (weightNeeded > van.Capacity)
                {
                    return null;
                }

                //今天没有必须补货的仓库，根据单位里程配送量决定是否配送
                if (chargeWares.Count == 0)
                {
                    var better1 = AddBestNode(company, set, chargeWares, van);

                    while (better1.Count != chargeWares.Count)
                    {
                        chargeWares = better1;
                        better1 = AddBestNode(company, set, chargeWares, van);
                    }

                    if (GetUnitDelivery(company, chargeWares) < 0.04m)
                    {
                        continue;
                    }
                }
                else
                {
                    //获取配送点的最短路径
                    var better = AddBestNode(company, set, chargeWares, van);

                    while (better.Count != chargeWares.Count)
                    {
                        chargeWares = better;
                        better = AddBestNode(company, set, chargeWares, van);
                    }
                }

                chargeWares = Calculate.GetShortestPath(company, chargeWares);

                var nodes = new List<DistributionNode>();
                for (int j = 0; j < chargeWares.Count; j++)
                {
                    var warehouse = chargeWares[j];
                    nodes.Add(new DistributionNode(warehouse));
                }

                var path = new DistributionPath(company, van, nodes);
                result.AddPath(path);

            }

            return result;
        }

        /// <summary>
        /// 从线路set剩余的点中选择一个点加入baseNodes中，找出最优的组合
        /// </summary>
        private static List<Warehouse> AddBestNode(Company company, List<Warehouse> set, List<Warehouse> baseNodes, Van van)
        {
            var result = new List<Warehouse>(baseNodes);

            var spare = set.Except(baseNodes).ToList();
            decimal bestUnitDelivery = GetUnitDelivery(company, baseNodes);
            List<Warehouse> bestSet = baseNodes;

            foreach (var node in spare)
            {
                baseNodes.Add(node);
                if (GetTotalNeedWeight(baseNodes) < van.Capacity)
                {
                    if (GetUnitDelivery(company, baseNodes) > bestUnitDelivery)
                    {
                        bestUnitDelivery = GetUnitDelivery(company, baseNodes);
                        bestSet = new List<Warehouse>(baseNodes);
                    }
                }
                baseNodes.Remove(node);
            }

            return bestSet;
        }

        /// <summary>
        /// 获取单位里程配送量
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        private static decimal GetUnitDelivery(Company company, List<Warehouse> set)
        {
            if (set.Count == 0)
            {
                return 0;
            }

            return GetTotalNeedWeight(set) / GetShortestPathLength(company, set);
        }


        /// <summary>
        /// 从集合wares中找出2个距离start最近的点，返回这3个点的集合
        /// </summary>
        /// <param name="start"></param>
        /// <param name="wares"></param>
        /// <returns></returns>
        public static List<Warehouse> CombineNearestWareHouses(Warehouse start, List<Warehouse> wares)
        {
            var distances = new Dictionary<Warehouse, decimal>();
            foreach (var item in wares)
            {
                var distance = start.Location.GetDistince(item.Location);
                distances.Add(item, distance);
            }
            distances.OrderBy(p => p.Value);

            var result = distances.Take(2).Select(p => p.Key).ToList();
            result.Add(start);
            return result;
        }

        //根据传入集合返回划分
        public static List<List<List<Warehouse>>> GetPartition(List<Warehouse> warehouses, int amount)
        {
            List<List<Warehouse>> combine = new List<List<Warehouse>>();

            var temp = warehouses.ToList();
            while (temp.Count > 0)
            {
                var ware = temp[0];
                temp.Remove(ware);
                var item = Calculate.CombineNearestWareHouses(ware, temp);
                combine.Add(item);
                temp = temp.Except(item).ToList();
            }

            List<int> combineIndex = new List<int>();
            for (int i = 0; i < combine.Count; i++)
            {
                combineIndex.Add(i);
            }

            //对合并的索引分组
            var partitions = Util.SetPartition(combineIndex, 5);
            List<List<List<Warehouse>>> result = new List<List<List<Warehouse>>>();
            foreach (var partition in partitions)
            {
                var wpartition = new List<List<Warehouse>>();
                foreach (var indexs in partition)
                {
                    var wares = new List<Warehouse>();
                    foreach (var index in indexs)
                    {
                        wares.AddRange(combine[index]);
                    }
                    wpartition.Add(wares);
                }
                result.Add(wpartition);
            }

            return result;
        }

        /// <summary>
        /// 过滤含有太大集合的划分
        /// </summary>
        /// <param name="partitions"></param>
        /// <returns></returns>
        public static List<List<List<Warehouse>>> FilterPartitions(List<List<List<Warehouse>>> partitions)
        {
            List<List<List<Warehouse>>> result = new List<List<List<Warehouse>>>();
            foreach (var part in partitions)
            {
                var isVaild = true;
                foreach (var set in part)
                {
                    if (set.Count > 9)
                    {
                        isVaild = false;
                    }
                }
                if (isVaild)
                {
                    result.Add(part);
                }
            }
            return result;

        }

        /// <summary>
        /// 过滤含有太大集合的划分
        /// </summary>
        /// <param name="partitions"></param>
        /// <returns></returns>
        public static bool IsPartitionValid(List<List<Warehouse>> part, List<Van> vans)
        {
            var isVaild = true;
            for (var i = 0; i < part.Count; i++)
            {
                var set = part[i];
                var van = vans[i];
                if (GetTotalDailyUsage(set) > van.Capacity)
                {
                    isVaild = false;
                }
            }

            return isVaild;
        }

        public static List<DistributionNode> GetDistributionNodeList(List<Warehouse> wares)
        {
            var result = new List<DistributionNode>();
            foreach (var ware in wares)
            {
                result.Add(new DistributionNode(ware));
            }
            return result;
        }


    }
}
