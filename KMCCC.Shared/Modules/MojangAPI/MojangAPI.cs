using System;
using System.Collections.Generic;
using System.Text;

namespace KMCCC.Modules.MojangAPI
{
    public interface IMojangAPI
    {
        Dictionary<string, ServiceStatus> GetServiceStatus();

        Statistics GetStatistics();
    }

    public class MojangAPI
    {
        public static IMojangAPI Api = new MojangAPIInternal();

        /// <summary>
        ///     获取MojangAPI服务状态
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, ServiceStatus> GetServiceStatus()
        {
            return Api.GetServiceStatus();
        }

        /// <summary>
        ///     获取销量等信息
        /// </summary>
        /// <returns></returns>
        public static Statistics GetStatistics()
        {
            return Api.GetStatistics();
        }
    }
}
