using LoggServer;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace GrpcServer
{
    public static class ChannelComunication
    {
        public static IModel _channel; 

        public static void SetChannel(IModel channel)
        {
            _channel = channel;
        }

    }
}
