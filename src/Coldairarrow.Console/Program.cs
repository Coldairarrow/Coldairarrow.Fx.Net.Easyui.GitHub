using Coldairarrow.Entity.Base_SysManage;
using Coldairarrow.Util;
using Coldairarrow.Util.Sockets;
using Coldairarrow.Util.Wcf;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Groups;
using DotNetty.Transport.Channels.Sockets;
using Echo.Client;
using Echo.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Coldairarrow.DotNettyRPC;

namespace Coldairarrow.Console1
{
    [ServiceContract]
    public interface IHello
    {
        [OperationContract]
        string SayHello(string msg);
    }
    public class Hello : IHello
    {
        public string SayHello(string msg)
        {
            return msg;
        }
    }

    public interface IOrderService
    {
        decimal CalculateFinalOrderSum(long userId, double originalSum);
    }
    public class OrderService : IOrderService
    {
        public decimal CalculateFinalOrderSum(long userId, double originalSum)
        {
            return 66666666666;
        }
    }
    class Program
    {
        static void RpcTest()
        {
            int port = 9999;
            int count = 1;
            int errorCount = 0;
            RPCServer rPCServer = new RPCServer(port);
            rPCServer.RegisterService<IHello, Hello>();
            rPCServer.Start();
            IHello client = null;
            client = RPCClientFactory.GetClient<IHello>("127.0.0.1", port);
            client.SayHello("aaa");
            Stopwatch watch = new Stopwatch();
            List<Task> tasks = new List<Task>();
            watch.Start();
            LoopHelper.Loop(1, () =>
            {
                tasks.Add(Task.Run(() =>
                {
                    LoopHelper.Loop(count, () =>
                    {
                        string msg = string.Empty;
                        try
                        {
                            msg = client.SayHello("Hello");
                            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff")}:{msg}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ExceptionHelper.GetExceptionAllMsg(ex));
                        }
                    });
                }));
            });
            Task.WaitAll(tasks.ToArray());
            watch.Stop();
            Console.WriteLine($"每次耗时:{(double)watch.ElapsedMilliseconds / count}ms");
            Console.WriteLine($"错误次数：{errorCount}");
        }

        static void WcfTest()
        {
            int threadCount = 4;
            int port = 9999;
            int count = 10000;
            int errorCount = 0;

            WcfHost<IHello, Hello> wcfHost = new WcfHost<IHello, Hello>();
            wcfHost.StartHost();
            IHello client = WcfClient.GetService<IHello>("http://127.0.0.1:14725");
            client.SayHello("Hello");
            Stopwatch watch = new Stopwatch();
            List<Task> tasks = new List<Task>();

            watch.Start();
            LoopHelper.Loop(threadCount, () =>
            {
                tasks.Add(Task.Run(() =>
                {
                    LoopHelper.Loop(count, index =>
                    {
                        var msg = client.SayHello("Hello" + index);
                        //Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss:ffffff")}:{msg}");
                    });
                }));
            });
            Task.WaitAll(tasks.ToArray());
            watch.Stop();
            Console.WriteLine($"并发数:{threadCount},运行:{count}次,每次耗时:{(double)watch.ElapsedMilliseconds / count}ms");
        }

        static void SocketTest()
        {
            int count = 1000;
            int port = 11111;
            Stopwatch watch = new Stopwatch();
            TcpSocketServer server = new TcpSocketServer(port);
            server.HandleRecMsg = (a, b, c) =>
            {
                b.Send(c);
            };
            server.StartServer();
            watch.Start();
            LoopHelper.Loop(count, () =>
            {
                //AutoResetEvent waitEvent = new AutoResetEvent(false);
                TcpSocketClient tcpSocketClient = new TcpSocketClient(port);
                tcpSocketClient.HandleRecMsg = (a, b) =>
                {
                    Console.WriteLine($"收到时间:{DateTime.Now.ToString("HH:mm:ss:ffffff")}");
                    //waitEvent.Set();
                };
                tcpSocketClient.StartClient();
                Console.WriteLine($"发送时间:{DateTime.Now.ToString("HH:mm:ss:ffffff")}");
                tcpSocketClient.Send(new byte[] { 0X01 });
                //waitEvent.WaitOne();
                Thread.Sleep(1000);
            });
            watch.Stop();
            Console.WriteLine($"每次耗时:{(double)watch.ElapsedMilliseconds / count}ms");
        }

        static void DotNettyTest()
        {
            int port = 8007;
            ClientWait clientWait = new ClientWait();
            RunServerAsync();
            RunClientAsync();
            void RunServerAsync()
            {
                try
                {
                    var bootstrap = new ServerBootstrap()
                        .Group(new MultithreadEventLoopGroup(1), new MultithreadEventLoopGroup(4))
                        .Channel<TcpServerSocketChannel>()
                        .Option(ChannelOption.SoBacklog, 100)
                        .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                        {
                            IChannelPipeline pipeline = channel.Pipeline;
                            pipeline.AddLast("framing-enc", new LengthFieldPrepender(2));
                            pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 2, 0, 2));

                            pipeline.AddLast(new EchoServerHandler("小明"));
                        }));

                    IChannel boundChannel = bootstrap.BindAsync(port).Result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            void RunClientAsync()
            {
                try
                {
                    Stopwatch watch = new Stopwatch();
                    var bootstrap = new Bootstrap()
                        .Group(new MultithreadEventLoopGroup())
                        .Channel<TcpSocketChannel>()
                        .Option(ChannelOption.TcpNodelay, true)
                        .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                        {
                            IChannelPipeline pipeline = channel.Pipeline;
                            pipeline.AddLast("framing-enc", new LengthFieldPrepender(2));
                            pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 2, 0, 2));

                            pipeline.AddLast(new EchoClientHandler(clientWait));
                        }));
                    
                    watch.Start();
                    watch.Stop();
                    Console.WriteLine($"初始化耗时:{watch.ElapsedMilliseconds}ms");

                    while (true)
                    {
                        try
                        {
                            watch = new Stopwatch();
                            watch.Restart();
                            var clientChannel = bootstrap.ConnectAsync($"127.0.0.1:{port}".ToIPEndPoint()).Result;
                            clientWait.Start(clientChannel.Id.AsShortText());
                            Task.Run(() =>
                            {
                                clientWait.Wait(clientChannel.Id.AsShortText());
                                watch.Stop();
                                Console.WriteLine($"耗时:{(double)watch.ElapsedTicks / 10000}ms");
                            });
                            IByteBuffer msg = Unpooled.WrappedBuffer(Encoding.UTF8.GetBytes($"Hello World"));
                            clientChannel.WriteAndFlushAsync(msg);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                        //Thread.Sleep(1);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            string GetTime()
            {
                return $"{(double)DateTime.Now.Ticks/10000}ms";
            }
        }

        static void DotNettyRPCTest()
        {
            RPCServer rpcServer = new RPCServer(9999);
            rpcServer.RegisterService<IOrderService, OrderService>();
            rpcServer.Start();
            IOrderService client = RPCClientFactory.GetClient<IOrderService>("127.0.0.1", 9999);
            var res = client.CalculateFinalOrderSum(9, 3);

            Console.ReadLine();
        }

        static void Main(string[] args)
        {
            //WcfTest();
            //SocketTest();
            //DotNettyTest();
            //RpcTest();
            //DotNettyRPCTest();
            RPCServer rpcServer = new RPCServer(9999);
            rpcServer.RegisterService<IHello, Hello>();
            rpcServer.Start();

            Console.WriteLine("完成");
            Console.ReadLine();
        }
    }
}
