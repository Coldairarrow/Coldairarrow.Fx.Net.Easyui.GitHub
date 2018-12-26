// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Echo.Server
{
    using System;
    using System.Linq;
    using System.Text;
    using Coldairarrow.Util;
    using DotNetty.Buffers;
    using DotNetty.Transport.Channels;
    using DotNetty.Transport.Channels.Groups;

    public class EchoServerHandler : ChannelHandlerAdapter
    {
        public EchoServerHandler(string str)
        {
            Str = str;
        }
        public static IChannelGroup group;
        public string Str = string.Empty;
        public override void ChannelActive(IChannelHandlerContext contex)
        {
            IChannelGroup g = group;
            if (g == null)
            {
                lock (this)
                {
                    if (group == null)
                    {
                        g = group = new DefaultChannelGroup(contex.Executor);
                    }
                }
            }

            g.Add(contex.Channel);
        }
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var msg = message as IByteBuffer;
            //Console.WriteLine($"服务端收到:{msg.ToString(Encoding.UTF8)}");
            context.WriteAndFlushAsync(message);
            context.Channel.CloseAsync();
            //context.DisconnectAsync();
            //context.WriteAsync(message);
        }
        public override void ChannelRegistered(IChannelHandlerContext context)
        {
            //count++;
            //Console.WriteLine($"收到新的注册:{context.Channel.Id}");
        }
        public override void ChannelUnregistered(IChannelHandlerContext context)
        {
            //Console.WriteLine($"取消注册注册:{context.Channel.Id}");
        }
        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine("Exception: " + exception);
            context.CloseAsync();
        }
    }
}