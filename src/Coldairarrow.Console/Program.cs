using Coldairarrow.DataRepository;
using Coldairarrow.Entity.Base_SysManage;
using Coldairarrow.Util;
using Nest;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Coldairarrow.Console1
{
    class Program
    {
        [ElasticsearchType(Name = "Tweet", IdProperty = "Id")]
        public class Tweet
        {
            public string Id { get; set; }
            public string User { get; set; }
            public DateTime PostDate { get; set; }
            public string Message { get; set; }
        }

        public class Tweet2
        {
            public string Id { get; set; }
            public string User { get; set; }
            public DateTime PostDate { get; set; }
            public string Message { get; set; }
        }
        public static void ElasticSearchTest()
        {
            var node = new Uri("http://localhost:9200/");
            var settings = new ConnectionSettings(node).DefaultIndex("111");
            var client = new ElasticClient(settings);
            var tweet = new Tweet
            {
                Id = Guid.NewGuid().ToString(),
                User = "kimchy",
                PostDate = DateTime.Now,
                Message = "按时打撒小明6666"
            };
            List<Tweet> insertList = new List<Tweet>();
            insertList.Add(new Tweet
            {
                Id = Guid.NewGuid().ToString(),
                User = "kimchy",
                PostDate = DateTime.Now,
                Message = "按时打撒小明6666"
            });
            insertList.Add(new Tweet
            {
                Id = Guid.NewGuid().ToString(),
                User = "kimchy",
                PostDate = DateTime.Now,
                Message = "按时打撒小明6666"
            });

            //var creatRes= client.CreateDocument(tweet);
            //var requestRes= client.IndexDocument(tweet); //or specify index via settings.DefaultIndex("mytweetindex");
            var descriptor = new BulkDescriptor();
            descriptor.IndexMany(insertList);
            client.Bulk(descriptor);
            var refreshRes= client.Refresh(Indices.Parse("111"));
            //client.Create(tweet,x=>x.Index("test"));
            //var res = client.Get<Tweet>(1);
            client = new ElasticClient(settings);
            var response = client.Search<Tweet>(x=>x.From(0).Take(100).Query(o=>o.Match(y=>y.Field(z=>z.Message).Query("小明"))));
            

            var list = response.Documents;

            string tmp = string.Empty;
        }
        static void Main(string[] args)
        {
            //var db = DbFactory.GetRepository();
            ElasticSearchTest();
            Console.WriteLine("完成");
            Console.ReadLine();
        }
    }
}
