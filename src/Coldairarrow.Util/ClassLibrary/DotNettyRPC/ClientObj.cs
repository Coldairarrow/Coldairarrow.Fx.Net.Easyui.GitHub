using System.Threading;

namespace Coldairarrow.DotNettyRPC
{
    class ClientObj
    {
        public AutoResetEvent WaitHandler { get; set; } = new AutoResetEvent(false);
        public string ResponseString { get; set; }
    }
}
