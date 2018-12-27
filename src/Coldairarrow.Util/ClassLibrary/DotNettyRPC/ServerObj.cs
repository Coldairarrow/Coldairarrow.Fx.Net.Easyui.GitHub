using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Groups;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coldairarrow.DotNettyRPC
{
    class ServerObj
    {
        public IChannelGroup ChannelGroup { get; set; }
        private ConcurrentDictionary<string, IChannelId> _channelIdDic { get; } = new ConcurrentDictionary<string, IChannelId>();

    }
}
