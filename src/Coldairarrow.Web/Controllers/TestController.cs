using Coldairarrow.Business;
using System.Web.Mvc;

namespace Coldairarrow.Web
{
    public class TestController : BaseController
    {
        TestBusiness testBusiness { get; } = new TestBusiness();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Test()
        {
            testBusiness.Test();

            return Success();
        }
    }
}