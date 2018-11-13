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
        static void Main(string[] args)
        {
            //var dbHelper = DbHelperFactory.GetDbHelper(DatabaseType.SqlServer, "BaseDb");
            //var tableInfo = dbHelper.GetDbTableInfo("Dev_Project");
            //var tables = dbHelper.GetDbAllTables();
            TimerHelper.SetTimeout(() =>
            {
                Console.WriteLine("执行111111");
            }, new TimeSpan(0, 0, 0));


            Console.WriteLine("完成");
            Console.ReadLine();
        }
    }
}
