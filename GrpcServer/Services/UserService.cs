using Domain;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcServer.Services
{
    public class UserService 
    {
        private ILogger<UserService> _logger;
        public UserService(ILogger<UserService> logger)
        {
            _logger = logger;
        }

        public Task<User> TransferUser(User user, Grpc.Core.ServerCallContext context)
        {
            return Task.FromResult(new User
            {
                UserName = "Usuario1"//,
                //Games = new List<Game>()
            });
        }
    }
}
