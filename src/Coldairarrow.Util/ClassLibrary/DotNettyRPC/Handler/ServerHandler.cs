using Coldairarrow.Util;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System;
using System.Diagnostics;
using System.Text;

namespace Coldairarrow.DotNettyRPC
{
    public class ServerHandler : ChannelHandlerAdapter
    {
        public ServerHandler(RPCServer rPCServer)
        {
            _rpcServer = rPCServer;
        }
        RPCServer _rpcServer { get; }
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var msg = message as IByteBuffer;
            Stopwatch watch = new Stopwatch();
            watch.Start();
            ResponseModel response = _rpcServer.GetResponse(msg.ToString(Encoding.UTF8).ToObject<RequestModel>());
            watch.Stop();
            //Console.WriteLine($"服务端处理耗时:{(double)watch.ElapsedTicks/10000}ms");
            var sendMsg = response.ToJson().ToBytes(Encoding.UTF8);
            context.WriteAndFlushAsync(Unpooled.WrappedBuffer(sendMsg));
            context.CloseAsync();
        }
        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine("Exception: " + exception);
            context.CloseAsync();
        }
    }
}