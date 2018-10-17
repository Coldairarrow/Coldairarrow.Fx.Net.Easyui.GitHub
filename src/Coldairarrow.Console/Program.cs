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

namespace Coldairarrow.Console1
{
    class Program
    {
        public static List<dynamic> GetSelectFields<T>(IQueryable<T> source, List<string> filds)
        {
            var ie = (IEnumerable)source.Select($"new ({string.Join(",", filds)})");
            return new List<dynamic>(ie.Cast<dynamic>());
        }

        static void Main(string[] args)
        {
            var bus = new Base_UserBusiness();
            bus.Service.GetDbContext().Database.Log = log =>
            {
                Console.WriteLine(log);
            };
            var q = bus.GetIQueryable();
            
            var list = GetSelectFields(q, new List<string> { "Id", "UserName" });

            Console.ReadLine();
        }
    }
}
