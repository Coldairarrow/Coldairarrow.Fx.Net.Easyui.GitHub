using Coldairarrow.Util;
using Coldairarrow.Util.Sockets;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Collections.Generic;

namespace Coldairarrow.DotNettyRPC
{
    public class RPCServer
    {
        #region 构造函数

        public RPCServer(int port)
        {
            _port = port;

            _serverBootstrap = new ServerBootstrap()
                .Group(new MultithreadEventLoopGroup(), new MultithreadEventLoopGroup())
                .Channel<TcpServerSocketChannel>()
                .Option(ChannelOption.SoBacklog, 100)
                .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;
                    pipeline.AddLast("framing-enc", new LengthFieldPrepender(2));
                    pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 2, 0, 2));

                    pipeline.AddLast(new ServerHandler(this));
                }));
        }

        #endregion

        #region 私有成员

        private int _port { get; set; }
        private TcpSocketServer _socketServer { get; set; }
        private Dictionary<string, Type> _serviceHandle { get; set; } = new Dictionary<string, Type>();
        ServerBootstrap _serverBootstrap { get; }
        IChannel _serverChannel { get; set; }
        internal ResponseModel GetResponse(RequestModel request)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                var requestModel = request;
                if (!_serviceHandle.ContainsKey(requestModel.ServiceName))
                    throw new Exception("未找到该服务");
                var serviceType = _serviceHandle[requestModel.ServiceName];
                var service = Activator.CreateInstance(serviceType);
                var method = serviceType.GetMethod(requestModel.MethodName);
                if (method == null)
                    throw new Exception("未找到该方法");
                var res = method.Invoke(service, requestModel.Paramters.ToArray());

                response.Success = true;
                response.Data = res.ToJson();
                response.Msg = "请求成功";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Msg = ExceptionHelper.GetExceptionAllMsg(ex);
            }

            return response;
        }

        #endregion

        #region 外部接口

        public void RegisterService<IService, Service>() where Service : class, IService where IService : class
        {
            _serviceHandle.Add(typeof(IService).FullName, typeof(Service));
        }

        public void Start()
        {
            _serverChannel = _serverBootstrap.BindAsync(_port).Result;
        }
        public void Stop()
        {
            _serverChannel.CloseAsync();
        }

        #endregion
    }
}
