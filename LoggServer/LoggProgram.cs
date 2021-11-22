using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace LoggServer
{
    public class LoggProgram
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

            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
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
                LoggServices.Instance().loggs.Add(logg);
            };
            channel.BasicConsume(
                queue: loggsQueue,
                autoAck: true,
                consumer: consumer);
        }
    }
}
