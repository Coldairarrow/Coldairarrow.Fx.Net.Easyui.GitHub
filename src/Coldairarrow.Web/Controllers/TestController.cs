using Coldairarrow.Business;
using Coldairarrow.DotNettyRPC;
using System.Web.Mvc;

namespace Coldairarrow.Web
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

    public class TestController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Test()
        {
            IHello client = RPCClientFactory.GetClient<IHello>("127.0.0.1", 9999);
            var res = client.SayHello("aa");

            return Content(res);
        }
    }
}