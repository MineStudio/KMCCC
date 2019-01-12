using System;
using System.Collections.Generic;
using System.Text;

namespace KMCCC.Pro.Modules.MojangAPI
{
    public interface IMojangAPI
    {
        Dictionary<string, ServiceStatus> GetServiceStatus();

        Statistics GetStatistics();

        Guid NameToUUID(string userName);
    }

    public class MojangAPI
    {
        private static IMojangAPI Api  = Api ?? new MojangAPIInternal();

        /// <summary>
        ///     获取MojangAPI服务状态，如果获取失败将抛异常
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, ServiceStatus> GetServiceStatus()
        {
            return Api.GetServiceStatus();
        }

        /// <summary>
        ///     获取销量等信息，如果获取失败将抛异常
        /// </summary>
        /// <returns></returns>
        public static Statistics GetStatistics()
        {
            return Api.GetStatistics();
        }

        /// <summary>
        ///     通过用户名获取UUID，如果获取失败将抛异常
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns></returns>
        public static Guid NameToUUID(string userName)
        {
            return Api.NameToUUID(userName);
        }
    }
}
