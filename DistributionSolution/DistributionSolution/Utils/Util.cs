using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributionSolution.Model;
using System.Runtime.Caching;
using System.IO;

namespace DistributionSolution.Utils
{
    public static class Util
    {
        /// <summary>
        /// 全排列算法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nodes">需要进行全排列的元素列表</param>
        /// <param name="count">全排列元素数量</param>
        /// <param name="temp">临时列表，长度与nodes相同</param>
        /// <param name="results">全排列</param>
        public static void Permutation<T>(List<T> nodes, int count, List<T> temp, List<List<T>> results)
        {
            foreach (var node in nodes)
            {
                int a = nodes.Count();
                //将第count - nodes.Count()个元素放入临时列表
                temp[count - a] = node;
                var spare = new List<T>(nodes);
                spare.Remove(node);
                if (spare.Count() == 0)
                {
                    //将完成的排列存入结果集
                    results.Add(new List<T>(temp));
                    return;
                }
                Permutation(spare, count, temp, results);
            }
        }

        private static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        /// <summary>
        /// 字典序法获取下一个排列
        /// </summary>
        /// <param name="s"></param>
        /// <param name="first"></param>
        /// <param name="last"></param>
        /// <returns></returns>
        public static bool NextPermutation(ref int[] s, int first, int last)    //last最后一个元素的下一个位置 
        {
            if (last - first <= 1)    //保证至少2个元素
                return false;

            int i = last - 2, j = last - 1;

            while (true)
            {
                if (s[i] < s[j])    //只要存在这样的对，下一个排列就必定存在 
                {
                    int k = last;
                    while (s[i] >= s[--k]) ;
                    Swap(ref s[i], ref s[k]);

                    //j之前元素的列表
                    var part1 = s.Take(j).ToList();
                    //从j到末尾的数组
                    var temp = s.Skip(j).Reverse().ToList();
                    part1.AddRange(temp);
                    s = part1.ToArray();

                    return true;
                }
                if (i == first)        //找到第一个元素都找不到，说明该数组是降序排列的，下一个排列不存在 
                {
                    //j之前元素的列表
                    var part1 = s.Take(j).ToList();
                    //从j到末尾的数组
                    var temp = s.Skip(j).Reverse().ToList();
                    part1.AddRange(temp);
                    s = part1.ToArray();
                    return false;
                }
                i--;
                j--;
            }
        }

        /// <summary>
        /// 根据集合产生groupCount个真子集的划分
        /// 动态规划缓存结果可提高效率
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="set"></param>
        /// <param name="groupCount"></param>
        /// <returns></returns>
        public static List<List<List<T>>> SetPartition<T>(List<T> set, int groupCount)
        {
            int count = set.Count();
            var cache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy()
            {
                AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(30))
            };
            var key = $"Partition-{count}-{groupCount}";

            var value = cache[key] as List<List<List<T>>>;
            if (value != null)
            {
                return value;
            }

            List<List<List<T>>> results = new List<List<List<T>>>();
            if (count < groupCount || groupCount < 1 || count < 1)
            {
                return results;
            }

            //划分为1组时
            if (groupCount == 1)
            {
                results.Add(new List<List<T>> { new List<T>(set) });
                return results;
            }

            //将集合最后一个元素加入临时集合
            var lastElement = set.Last();
            List<List<T>> tempPartition = new List<List<T>>();
            List<T> tempSet = new List<T> { lastElement };
            set.Remove(lastElement);

            //计算n-1,m-1时的划分
            var temp = SetPartition(new List<T>(set), groupCount - 1);
            foreach (var item in temp)
            {
                item.Add(tempSet);
                results.Add(item);
            }

            //计算n-1,m时的划分
            temp = SetPartition(new List<T>(set), groupCount);
            foreach (var item in temp)
            {
                for (int i = 0; i < groupCount; i++)
                {
                    //深拷贝
                    var newPart = new List<List<T>>();
                    item.ForEach(p => newPart.Add(new List<T>(p)));
                    newPart[i].Add(lastElement);
                    results.Add(newPart);
                }
            }

            cache.Set(key, results, policy);
            return results;

        }

        public static void PrintPartitions<T>(List<List<List<T>>> partitions)
        {
            int count = partitions.Count;
            Console.WriteLine($"共有{count}种划分方案");

            StringBuilder sbu = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                int groupCount = partitions[i].Count;

                sbu.Append($"{i}:    {{");
                foreach (var set in partitions[i])
                {
                    sbu.Append(GetListString(set));
                }

                sbu.AppendLine("}");
            }

            Console.WriteLine(sbu.ToString());
        }

        public static string GetListString<T>(IEnumerable<T> list)
        {
            StringBuilder result = new StringBuilder();
            result.Append("{");
            foreach (var p in list)
            {
                result.Append(p + ",");
            }
            result.Append("}");
            return result.ToString();
        }

        public static string GetResourcePath()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Resource");
            return path;
        }

    }
}
