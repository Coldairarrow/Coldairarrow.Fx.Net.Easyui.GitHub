using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coldairarrow.Entity.Base_SysManage;
using Coldairarrow.Util;
using Elasticsearch.Net;
using Nest;

namespace Coldairarrow.Business
{
    class ElasticSearchLogger : ILogger
    {
        static ElasticSearchLogger()
        {
            string index = $"{GlobalSwitch.ProjectName}.{typeof(Base_SysLog).Name}".ToLower();

            var pool = new StaticConnectionPool(GlobalSwitch.ElasticSearchNodes);
            _connectionSettings = new ConnectionSettings(pool).DefaultIndex(index);

            var client = new ElasticClient(_connectionSettings);
            if (!client.IndexExists(Indices.Parse(index)).Exists)
            {
                var descriptor = new CreateIndexDescriptor(index)
                    .Mappings(ms => ms
                        .Map<Base_SysLog>(m => m.AutoMap())
                    );
                var res = client.CreateIndex(descriptor);
            }
        }
        private static ConnectionSettings _connectionSettings { get; set; }
        public List<Base_SysLog> GetLogList(string logContent, string logType, string opUserName, DateTime? startTime, DateTime? endTime, Pagination pagination)
        {
            //var client = GetElasticClient();
            //Func<QueryContainerDescriptor<Base_SysLog>, QueryContainer> q=
            //var list = client.Search<Base_SysLog>(x =>
            //    x.Query(y =>
            //        y.Wildcard(z =>
            //            z.Field(o => o.LogContent).Value("*同步*")
            //        )
            //        && y.DateRange(dr => dr.Field(f => f.OpTime).LessThan(DateTime.Now))
            //        && y.Terms(t => t.Field(f => f.LogType).Terms("开发测试日志"))
            //    )
            //    .Size(30)
            //    .Sort(s => s.Descending(d => d.OpTime))
            //).Documents;

            return null;
        }

        public void WriteSysLog(Base_SysLog log)
        {
            GetElasticClient().IndexDocument(log);
        }

        private ElasticClient GetElasticClient()
        {
            return new ElasticClient(_connectionSettings);
        }
    }
}
