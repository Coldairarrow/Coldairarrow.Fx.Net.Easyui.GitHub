// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Echo.Server
{
    using System;
    using System.Text;
    using DotNetty.Buffers;
    using DotNetty.Transport.Channels;

    public class EchoServerHandler : ChannelHandlerAdapter
    {
        
        int count = 0;
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var buffer = message as IByteBuffer;
            var bytes = buffer.Array;
            context.WriteAsync(message);
        }
        public override void ChannelRegistered(IChannelHandlerContext context)
        {
            //count++;
            Console.WriteLine($"收到新的注册:{context.Channel.Id}");
        }
        public override void ChannelActive(IChannelHandlerContext context)
        {
            count++;
        }
        public override void ChannelUnregistered(IChannelHandlerContext context)
        {
            Console.WriteLine($"取消注册注册:{context.Channel.Id}");
        }
        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine("Exception: " + exception);
            context.CloseAsync();
        }
    }
}