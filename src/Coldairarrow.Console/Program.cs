using Coldairarrow.Business.Base_SysManage;
using Coldairarrow.Entity.Base_SysManage;
using Coldairarrow.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coldairarrow.Console1
{
    class Program
    {
        static void Main(string[] args)
        {
            Base_UserBusiness base_UserBusiness = new Base_UserBusiness();
            base_UserBusiness.Service.GetDbContext().Database.Log = log =>
            {
                Console.WriteLine(log);
            };

            //筛选角色名为“超级管理员”的人员
            string roleName = "超级管理员";
            var q = (from a in base_UserBusiness.Service.GetIQueryable<Base_User>()
                     join b in base_UserBusiness.Service.GetIQueryable<Base_UserRoleMap>() on a.UserId equals b.UserId
                     join c in base_UserBusiness.Service.GetIQueryable<Base_SysRole>() on b.RoleId equals c.RoleId
                     where c.RoleName == roleName
                     select a).Distinct();
            var data= q.ToList();
            Console.WriteLine(data.ToJson());

            //base_UserBusiness.GetList();

            Console.WriteLine("完成");
            Console.ReadLine();
        }
    }
}
