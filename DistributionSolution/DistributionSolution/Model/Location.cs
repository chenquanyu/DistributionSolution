using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using DistributionSolution.Utils;
using System.IO;

namespace DistributionSolution.Model
{
    //地理位置
    public class Location
    {
        //纬度
        public decimal x;

        //经度
        public decimal y;

        private static Dictionary<string, decimal> DistanceInfo { get; set; }

        public decimal GetDistince(Location end)
        {
            ////测试阶段直接计算点到点距离
            //return (decimal)Math.Sqrt((double)((x - end.x) * (x - end.x) + (y - end.y) * (y - end.y)));

            if (DistanceInfo == null)
            {
                DistanceInfo = JsonUtil.DeSerialize<Dictionary<string, decimal>>(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, @"Resource\DistanceInfo.json")));
            }

            //var cache = MemoryCache.Default;
            //CacheItemPolicy policy = new CacheItemPolicy()
            //{
            //    AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(30))
            //};
            string key = $"DISTANCE-{x}-{y}-{end.x}-{end.y}";

            //var value = cache[key];
            //if (value != null)
            //{
            //    return (decimal)value;
            //}

            var result = DistanceInfo[key];
            //cache.Set(key, result, policy);

            return result;

        }

    }
}
