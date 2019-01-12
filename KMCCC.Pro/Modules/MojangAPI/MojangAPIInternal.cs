using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using LitJson;
using System.Linq;

namespace KMCCC.Pro.Modules.MojangAPI
{
    public class MojangAPIInternal : IMojangAPI
    {
        #region 获取服务器状态
        public Dictionary<string, ServiceStatus> GetServiceStatus()
        {
            Dictionary<string, ServiceStatus> status = new Dictionary<string, ServiceStatus>();
            try
            {
                using (WebClient webclient = new WebClient())
                {
                    webclient.Headers.Add("Content-Type", "application/json");
                    var value = JsonMapper.ToObject(webclient.DownloadString(MojangAPIProvider.apiStatus()));
                    foreach (JsonData sta in value)
                    {
                        var key = sta.Keys.Select(x => x.ToString()).First();
                        status.Add(key, (ServiceStatus)Enum.Parse(typeof(ServiceStatus), sta[key].ToString()));
                    }
                    return status;
                }
            }
            catch(Exception ex)
            {
                /*
                status.Add(MojangAPIProvider.apiStatus(), ServiceStatus.red);
                return status;
                */
                throw ex;
            }
        }
        #endregion

        #region 获取销量
        public Statistics GetStatistics()
        {
            try
            {
                using (WebClient webclient = new WebClient())
                {
                    webclient.Headers.Add("Content-Type", "application/json");
                    JsonData data = new JsonData
                    {
                        ["metricKeys"] = new JsonData()
                    };
                    JsonData metricKey_sdata = new JsonData
                    {
                        MetricKeys.ITEM_SOLD_MINECRAFT,
                        MetricKeys.PREPAID_CARD_REDEEMED_MINECRAFT
                    };
                    data["metricKeys"] = metricKey_sdata;
                    var value = JsonMapper.ToObject(webclient.UploadString(MojangAPIProvider.statistics(), data.ToJson()));
                    return new Statistics(long.Parse(value[0].ToString()), long.Parse(value[1].ToString()), double.Parse(value[2].ToString()));
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 通过名字获取UUID
        public Guid NameToUUID(string userName)
        {
            try
            {
                using (WebClient webclient = new WebClient())
                {
                    var value = JsonMapper.ToObject(webclient.DownloadString(MojangAPIProvider.nameToUuid(userName)));
                    return Guid.Parse(value["id"].ToString());
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
