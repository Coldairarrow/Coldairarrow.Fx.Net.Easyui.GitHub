using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Coldairarrow.Console1
{
    public class ClientWait
    {
        private ConcurrentDictionary<string, AutoResetEvent> _waits { get; set; } = new ConcurrentDictionary<string, AutoResetEvent>();
        public void Start(string id)
        {
            _waits[id] = new AutoResetEvent(false);
        }
        public void Set(string id)
        {
            _waits[id].Set();
        }
        public void Wait(string id)
        {
            var waitHandler = _waits[id];
            waitHandler.WaitOne();
            Task.Run(() =>
            {
                _waits.TryRemove(id, out AutoResetEvent autoResetEvent);
            });
        }
    }
}
