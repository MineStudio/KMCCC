using System;
using System.Collections.Generic;
using System.Text;

namespace KMCCC.Pro.Modules.MojangAPI
{
    public class Statistics
    {
        private static long serialVersionUID = 1L;

        private long total;
        private long last24h;
        private double saleVelocityPerSeconds;

        public Statistics(long total, long last24h, double saleVelocityPerSeconds)
        {
            this.total = total;
            this.last24h = last24h;
            this.saleVelocityPerSeconds = saleVelocityPerSeconds;
        }

        /// <summary>
        ///     获取总销量
        /// </summary>
        /// <returns></returns>
        public long getTotal()
        {
            return total;
        }

        /// <summary>
        ///     获取过去24小时销量
        /// </summary>
        /// <returns></returns>
        public long getLast24h()
        {
            return last24h;
        }

        /// <summary>
        ///     获取每秒销售速度
        /// </summary>
        /// <returns></returns>
        public double getSaleVelocityPerSeconds()
        {
            return saleVelocityPerSeconds;
        }

        public override string ToString()
        {
            return string.Format("SalesStatistics [total={0}, last24h={1}, saleVelocityPerSeconds={2}]", total, last24h, saleVelocityPerSeconds);
        }
    }
    public static class MetricKeys
    {
        public static readonly string ITEM_SOLD_MINECRAFT = "item_sold_minecraft";
		public static readonly string PREPAID_CARD_REDEEMED_MINECRAFT = "prepaid_card_redeemed_minecraft";
		public static readonly string ITEM_SOLD_COBALT = "item_sold_cobalt";
		public static readonly string ITEM_SOLD_SCROLLS = "item_sold_scrolls";
    }

}
