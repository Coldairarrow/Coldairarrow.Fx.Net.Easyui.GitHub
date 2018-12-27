using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Coldairarrow.DotNettyRPC
{
    class ClientWait
    {
        private ConcurrentDictionary<string, ClientObj> _waits { get; set; } = new ConcurrentDictionary<string, ClientObj>();
        public void Start(string id)
        {
            _waits[id] = new ClientObj();
        }
        public void Set(string id, string responseStriong)
        {
            var theObj = _waits[id];
            theObj.ResponseString = responseStriong;
            theObj.WaitHandler.Set();
        }
        public ClientObj Wait(string id)
        {
            var clientObj = _waits[id];
            clientObj.WaitHandler.WaitOne();
            Task.Run(() =>
            {
                _waits.TryRemove(id, out ClientObj value);
            });
            return clientObj;
        }
    }
}