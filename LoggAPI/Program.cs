using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Domain;
using LoggAPI.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LoggAPI
{
    public class Program
    {
        private const string loggsQueue = "loggsQueue";
        private static IModel channel;
        public static void Main(string[] args)
        {
            //var factory = new ConnectionFactory { HostName = "localhost" };
            //using IConnection connection = factory.CreateConnection();
            //channel = connection.CreateModel();
            channel = Repository.Lists.channel;
            DeclareQueue(channel);
            Messages.ReceiveMessages();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        private static void DeclareQueue(IModel channel)
        {
            channel.QueueDeclare(
                queue: loggsQueue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }
        
        
    }
}
