using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributionSolution.Model
{
    //仓库
    public class Warehouse
    {
        public Warehouse(string name, decimal capacity, decimal stock, decimal dailyUsage, Location location)
        {
            Name = name;
            Capacity = capacity;
            Stock = stock;
            DailyUsage = dailyUsage;
            Location = location;
        }

        public Warehouse() { }

        public string Name { get; set; }

        //容量
        public decimal Capacity { get; set; }

        //库存
        public decimal Stock { get; set; }

        //每天的用量Kg/天
        public decimal DailyUsage { get; set; }

        //地理位置
        public Location Location { get; set; }

        //库存百分比
        public decimal StockPercent
        {
            get
            {
                return Stock / Capacity;
            }
        }

        /// <summary>
        /// 计算次日是否需要补货
        /// </summary>
        /// <returns></returns>
        public bool IsNeedCharge()
        {
            //在第三天存量会少于30%的仓库需要在第二天送货
            return Stock - DailyUsage * 2 < 0.3m * Capacity;
        }

        public Warehouse Clone()
        {
            return new Warehouse(Name, Capacity, Stock, DailyUsage, Location);
        }

        public decimal ChargeAmount
        {
            get
            {
                return Capacity * 0.9m - Stock;
            }
        }

    }
}
