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
            
            var db = DbHelperFactory.GetDbHelper(DatabaseType.Oracle, "Data Source=127.0.0.1/ORCL;User ID=DB;Password=123456;Connect Timeout=3");
            var tables = db.GetDbTableInfo("Base_User");
            Console.ReadLine();
        }
    }
}
