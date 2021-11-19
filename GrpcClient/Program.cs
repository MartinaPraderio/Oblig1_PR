using System;
using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Net.Client;

namespace GrpcClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // The port number(5001) must match the port of the gRPC server.
            using var channel = GrpcChannel.ForAddress("https://localhost:7428");
            //var client = new Greeter.GreeterClient(channel);
            string input = "";
            while (!input.Equals("exit", StringComparison.InvariantCultureIgnoreCase))
            {
                input = Console.ReadLine();
                //HelloReply reply = await client.SayHelloAsync(new HelloRequest 
                //{ 
                //    Name = input 
                //});
                //Console.WriteLine(reply);
            }
            
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
