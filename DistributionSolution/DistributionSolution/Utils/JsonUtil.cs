using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DistributionSolution.Utils
{
    public class JsonUtil
    {
        public static string Serialize<T>(T item)
        {
            return JsonConvert.SerializeObject(item);
        }

        public static T DeSerialize<T>(string item)
        {
            return JsonConvert.DeserializeObject<T>(item);
        }

    }
}
