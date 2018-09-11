using Coldairarrow.Business.Base_SysManage;
using Coldairarrow.Util;
using System.Web.Mvc;

namespace Coldairarrow.Web
{
    /// <summary>
    /// 校验签名
    /// </summary>
    public class CheckSignAttribute : FilterAttribute, IActionFilter
    {
        /// <summary>
        /// Action执行之前执行
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            CheckSignBusiness _checkSignBusiness = new CheckSignBusiness();

            //若为本地测试，则不需要校验
            if (GlobalSwitch.RunModel == RunModel.LocalTest)
            {
                return;
            }

            //判断是否需要签名
            bool needSign = filterContext.ContainsAttribute<CheckSignAttribute>() && !filterContext.ContainsAttribute<IgnoreSignAttribute>();

            //不需要签名
            if (!needSign)
                return;

            //需要签名
            var checkSignRes = _checkSignBusiness.IsSecurity(filterContext.HttpContext.ApplicationInstance.Context);
            if (!checkSignRes.Success)
            {
                filterContext.Result = new ContentResult() { Content = checkSignRes.ToJson() };
            }
        }

        /// <summary>
        /// Action执行完毕之后执行
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {

        }
    }
}