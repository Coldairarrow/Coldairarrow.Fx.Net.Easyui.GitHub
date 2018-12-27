using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System;
using System.Text;

namespace Coldairarrow.DotNettyRPC
{
    class ClientHandler : ChannelHandlerAdapter
    {
        private ClientWait _clientWait { get; }
        public ClientHandler(ClientWait clientWait)
        {
            _clientWait = clientWait;
        }
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var buffer = message as IByteBuffer;
            _clientWait.Set(context.Channel.Id.AsShortText(), buffer.ToString(Encoding.UTF8));
        }
        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine("Exception: " + exception);
            context.CloseAsync();
        }
    }
}