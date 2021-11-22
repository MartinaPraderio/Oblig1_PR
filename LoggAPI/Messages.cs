using Domain;
using LoggAPI.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace LoggAPI
{
    public static class Messages
    {
        public static void ReceiveMessages()
        {
            var consumer = new EventingBasicConsumer(Repository.Lists.channel);
            consumer.Received += (model, ea) =>
            {
                byte[] body = ea.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);
                Logg logg = ByteArrayToLogg(body);
                Console.WriteLine("logg agregado " + logg);
                Repository.Lists.loggs.Add(logg);
                Console.WriteLine("cantidad de loggs " + Repository.Lists.loggs.Count());

            };
            Repository.Lists.channel.BasicConsume(
                queue: "loggsQueue",
                autoAck: true,
                consumer: consumer);
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
    }
}
