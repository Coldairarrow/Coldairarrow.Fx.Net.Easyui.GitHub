using System.Dynamic;

namespace Coldairarrow.Util.RPC
{
    class RPCClientProxy: DynamicObject
    {
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            try
            {
                result = "厉害了我的哥";
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }
    }
}
