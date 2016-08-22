using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using DistributionSolution.Model;

namespace DistributionSolution.Utils
{
    public class RestClientUtil
    {
        /// <summary>
        /// 使用高德地图API获取两个点之间的最短行驶距离
        /// </summary>
        /// <param name="l1"></param>
        /// <param name="l2"></param>
        /// <returns></returns>
        public static decimal GetDistance(Location l1, Location l2)
        {
            string url = $"http://restapi.amap.com/v3/distance?origins={l1.x},{l1.y}&destination={l2.x},{l2.y}&output=json&key=ae13e521dd5c9b508d8d2fa2706aaefc";
            var client = new RestClient("http://restapi.amap.com");
            var request = new RestRequest(new Uri(url));

            var result = client.Execute<DistanceRespose>(request).Data;
            return result.results[0].distance;
        }

    }

    public class DistanceRespose
    {
        public int status { get; set; }

        public string info { get; set; }

        public string infocode { get; set; }

        public List<DistanceResult> results { get; set; }

    }

    public class DistanceResult
    {
        public int origin_id { get; set; }

        public int dest_id { get; set; }

        public decimal distance { get; set; }

        public int duration { get; set; }
    }

}
