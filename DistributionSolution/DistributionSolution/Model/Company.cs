using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributionSolution.Model
{
    public class Company
    {
        public string Name { get; set; }

        //公司仓库
        public Warehouse Warehouse { get; set; }

        //公司货车
        public List<Van> Vans { get; set; }

    }
}
