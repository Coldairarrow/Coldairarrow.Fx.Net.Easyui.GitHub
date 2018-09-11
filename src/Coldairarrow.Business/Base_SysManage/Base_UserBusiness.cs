using Coldairarrow.Business.Cache;
using Coldairarrow.Business.Common;
using Coldairarrow.Entity.Base_SysManage;
using Coldairarrow.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;

namespace Coldairarrow.Business.Base_SysManage
{
    public class Base_UserBusiness : BaseBusiness<Base_User>
    {
        static Base_UserModelCache _cache { get; } = new Base_UserModelCache();

        #region 外部接口

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <param name="condition">查询类型</param>
        /// <param name="keyword">关键字</param>
        /// <returns></returns>
        public List<Base_UserModel> GetDataList(string condition, string keyword, Pagination pagination)
        {
            var whereExpre = LinqHelper.True<Base_UserModel>();

            Expression<Func<Base_User, object, object, Base_UserModel>> selectExpre = (a, b, c) => new Base_UserModel
            {
                RoleNameList = (List<string>)b,
                RoleIdList = (List<string>)c
            };
            selectExpre = selectExpre.BuildExtendSelectExpre();

            var db_Base_UserRoleMap = Service.GetIQueryable<Base_UserRoleMap>();
            var db_Base_SysRole = Service.GetIQueryable<Base_SysRole>();
            var q = from a in GetIQueryable().AsExpandable()
                    let roleIds = db_Base_UserRoleMap.Where(x => x.UserId == a.UserId).Select(x => x.RoleId)
                    let roleNames = db_Base_SysRole.Where(x => roleIds.Contains(x.RoleId)).Select(x => x.RoleName)
                    select selectExpre.Invoke(a, roleNames, roleIds);

            //模糊查询
            if (!condition.IsNullOrEmpty() && !keyword.IsNullOrEmpty())
                q = q.Where($@"{condition}.Contains(@0)", keyword);

            return q.GetPagination(pagination).ToList();
        }

        /// <summary>
        /// 获取指定的单条数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public Base_User GetTheData(string id)
        {
            return GetEntity(id);
        }

        public void AddData(Base_User newData)
        {
            if (GetIQueryable().Any(x => x.UserName == newData.UserName))
                throw new Exception("该用户名已存在！");

            Insert(newData);
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        public void UpdateData(Base_User theData)
        {
            if (theData.UserId == "Admin" && Operator.UserId != theData.UserId)
                throw new Exception("禁止更改超级管理员！");

            Update(theData);
        }

        public void SetUserRole(string userId, List<string> roleIds)
        {
            Service.Delete_Sql<Base_UserRoleMap>(x => x.UserId == userId);
            var insertList = roleIds.Select(x => new Base_UserRoleMap
            {
                Id = GuidHelper.GenerateKey(),
                UserId = userId,
                RoleId = x
            }).ToList();

            Service.Insert(insertList);
            _cache.UpdateCache(userId);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="theData">删除的数据</param>
        public void DeleteData(List<string> ids)
        {
            var adminUser = GetTheUser("Admin");
            if (ids.Contains(adminUser.Id))
                throw new Exception("超级管理员是内置账号,禁止删除！");

            Delete(ids);
        }

        /// <summary>
        /// 获取当前操作者信息
        /// </summary>
        /// <returns></returns>
        public static Base_UserModel GetCurrentUser()
        {
            return GetTheUser(Operator.UserId);
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public static Base_UserModel GetTheUser(string userId)
        {
            return _cache.GetCache(userId);
        }

        public static List<string> GetUserRoleIds(string userId)
        {
            return GetTheUser(userId).RoleIdList;
        }

        /// <summary>
        /// 更改密码
        /// </summary>
        /// <param name="oldPwd">老密码</param>
        /// <param name="newPwd">新密码</param>
        public AjaxResult ChangePwd(string oldPwd,string newPwd)
        {
            AjaxResult res = new AjaxResult() { Success = true };
            string userId = Operator.UserId;
            oldPwd = oldPwd.ToMD5String();
            newPwd = newPwd.ToMD5String();
            var theUser = GetIQueryable().Where(x => x.UserId == userId && x.Password == oldPwd).FirstOrDefault();
            if (theUser == null)
            {
                res.Success = false;
                res.Msg = "原密码不正确！";
            }
            else
            {
                theUser.Password = newPwd;
                Update(theUser);
            }

            _cache.UpdateCache(userId);

            return res;
        }

        /// <summary>
        /// 保存权限
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="permissions">权限值</param>
        public void SavePermission(string userId, List<string> permissions)
        {
            Service.Delete_Sql<Base_PermissionUser>(x => x.UserId == userId);
            var roleIdList = Service.GetIQueryable<Base_UserRoleMap>().Where(x => x.UserId == userId).Select(x => x.RoleId).ToList();
            var existsPermissions = Service.GetIQueryable<Base_PermissionRole>()
                .Where(x => roleIdList.Contains(x.RoleId) && permissions.Contains(x.PermissionValue))
                .GroupBy(x => x.PermissionValue)
                .Select(x => x.Key)
                .ToList();
            permissions.RemoveAll(x => existsPermissions.Contains(x));

            List<Base_PermissionUser> insertList = new List<Base_PermissionUser>();
            permissions.ForEach(newPermission =>
            {
                insertList.Add(new Base_PermissionUser
                {
                    Id = Guid.NewGuid().ToSequentialGuid(),
                    UserId = userId,
                    PermissionValue = newPermission
                });
            });

            Service.Insert(insertList);
        }

        #endregion

        #region 私有成员

        #endregion

        #region 数据模型

        #endregion
    }

    public class Base_UserModel : Base_User
    {
        public string RoleNames { get => string.Join(",", RoleNameList); }

        public List<string> RoleIdList { get; set; }

        public List<string> RoleNameList { get; set; }

        public EnumType.RoleType RoleType
        {
            get
            {
                int type = 0;

                var values = typeof(EnumType.RoleType).GetEnumValues();
                foreach (var aValue in values)
                {
                    if (RoleNames.Contains(aValue.ToString()))
                        type += (int)aValue;
                }

                return (EnumType.RoleType)type;
            }
        }
    }
}