using Coldairarrow.Business.Base_SysManage;
using Coldairarrow.Entity.Base_SysManage;
using Coldairarrow.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Dynamic;
using System.Collections;
using static Coldairarrow.Entity.Base_SysManage.EnumType;
using Coldairarrow.Util.RPC;
using System.Diagnostics;

namespace Coldairarrow.Console1
{
    public interface IHello
    {
        string SayHello(string msg);
    }
    public class Hello : IHello
    {
        public string SayHello(string msg)
        {
            return msg;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Action<Exception> handleException = ex =>
            {
                Console.WriteLine(ex.Message);
            };
            int port = 9999;
            int count = 1000000;
            RPCServer rPCServer = new RPCServer(port);
            rPCServer.RegisterService<IHello, Hello>();
            rPCServer.Start();
            IHello client = null;
            Stopwatch watch = new Stopwatch();
            watch.Start();
            LoopHelper.Loop(count, () =>
            {
                client = RPCClientFactory.GetClient<IHello>("127.0.0.1", port);
            });
            watch.Stop();

            //var res = client.SayHello("Hello");
            //Console.WriteLine($"客户端:{res}");

            Console.WriteLine($"耗时:{watch.ElapsedMilliseconds}ms");

            Console.ReadLine();
            Console.WriteLine("完成");
            Console.ReadLine();
        }
    }
}
