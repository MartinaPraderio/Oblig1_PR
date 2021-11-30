using Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace GrpcServer
{
    public static class Program
    {
        private const string loggsQueue = "loggsQueue";
        private const string ExitMessage = "exit";

        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using IConnection connection = factory.CreateConnection();
            using IModel channel = connection.CreateModel();
            DeclareQueue(channel);
            ChannelComunication.SetChannel(channel);
            CreateHostBuilder(args).Build().Run();
        }

        private static void DeclareQueue(IModel channel)
        {
            channel.QueueDeclare(
                queue: loggsQueue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }
        public static byte[] LoggToByteArray(Object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
        public static void PublishMessage(IModel channel, Logg log)
        {
            byte[] body = LoggToByteArray(log);
            channel.BasicPublish(
                exchange: "",
                routingKey: loggsQueue,
                basicProperties: null,
                body: body);
        }
        //private static void SendMessages(IModel channel)
        //{
        //    string message = string.Empty;
        //    while (!message.Equals(ExitMessage, StringComparison.InvariantCultureIgnoreCase))
        //    {
        //        message = Console.ReadLine();
        //        if (!string.IsNullOrEmpty(message) || !message.Equals(ExitMessage, StringComparison.InvariantCultureIgnoreCase))
        //            PublishMessage(channel, message);
        //    }
        //}


        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
