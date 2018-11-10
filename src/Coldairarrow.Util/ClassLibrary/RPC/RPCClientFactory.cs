using ImpromptuInterface;

namespace Coldairarrow.Util.RPC
{
    public class RPCClientFactory
    {
        public static T GetClient<T>() where T : class
        {
            return new RPCClientProxy().ActLike<T>();
        }
    }
}
