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
            var client = new TransferUser.TransferUserClient(channel);
            string input = "";
            while (!input.Equals("exit", StringComparison.InvariantCultureIgnoreCase))
            {
                input = Console.ReadLine();

                var reply = await client.SendUserAsync(new User
                {
                    UserName = input
                });
                Console.WriteLine(reply);
                //User reply = await 
            }
            
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
