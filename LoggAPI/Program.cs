﻿using System;
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

namespace LogsServer
{
    public class Program
    {
        private const string loggsQueue = "loggsQueue";
        private static IModel channel;
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using IConnection connection = factory.CreateConnection();
            channel = connection.CreateModel();

            DeclareQueue(channel);
            ReceiveMessages();

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
        public static Logg ByteArrayToLogg(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return (Logg)obj;
            }
        }
        public static void ReceiveMessages()
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                byte[] body = ea.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);
                Logg logg = ByteArrayToLogg(body);
                Console.WriteLine("logg agregado " + logg);
                LoggServices.Instance().AddLogg(logg);
                Console.WriteLine("cantidad de loggs " + LoggServices.Instance().loggs.Count());

            };
            channel.BasicConsume(
                queue: loggsQueue,
                autoAck: true,
                consumer: consumer);
        }
    }
}
