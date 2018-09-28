using Coldairarrow.Business.Base_SysManage;
using Coldairarrow.Entity.Base_SysManage;
using Coldairarrow.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Coldairarrow.Console1
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> nums = new List<string>() { "1", "2", "3" };

            while (true)
            {
                Console.WriteLine( RandomHelper.Next(nums));
                Thread.Sleep(1000);
            }

            Console.WriteLine("完成");
            Console.ReadLine();
        }
    }
}
