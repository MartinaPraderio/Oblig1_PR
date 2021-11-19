using Domain;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcServer.Services
{
    public class TransferUserService : 
    {
        private ILogger<TransferUserService> _logger;
        public TransferUserService(ILogger<TransferUserService> logger)
        {
            _logger = logger;
        }

        public Task<User> SendUser(User user, Grpc.Core.ServerCallContext context)
        {
            return Task.FromResult(new User
            {
                UserName = "Usuario1"//,
                //Games = new List<Game>()
            });
        }
    }
}
