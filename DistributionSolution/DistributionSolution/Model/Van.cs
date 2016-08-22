using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributionSolution.Model
{
    //货车
    public class Van
    {
        public Van(string name, decimal capacity, decimal unitCost)
        {
            Name = name;
            Capacity = capacity;
            UnitCost = unitCost;
        }

        public Van() { }

        public string Name { get; set; }

        public decimal Capacity { get; set; }

        //每公里费用
        public decimal UnitCost { get; set; }
    }
}
