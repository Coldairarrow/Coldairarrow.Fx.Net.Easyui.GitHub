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

namespace Coldairarrow.Console1
{
    class Program
    {
        static void Main(string[] args)
        {
            //var dbHelper = DbHelperFactory.GetDbHelper(DatabaseType.SqlServer, "BaseDb");
            //var tableInfo = dbHelper.GetDbTableInfo("AAAAAAAAA");
            //var tables = dbHelper.GetDbAllTables();
            RoleType role = RoleType.普通管理员 | RoleType.超级管理员 | RoleType.项目管理员;
            if (role.HasFlag(RoleType.普通管理员))
            {

            }
            if (role.HasFlag(RoleType.项目管理员))
            {

            }

            Console.WriteLine("完成");
            Console.ReadLine();
        }
    }
}
