using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcServer.Services
{
    public class ProviderService : Provider.ProviderBase
    {
        private ILogger<ProviderService> _logger;
        public ProviderService(ILogger<ProviderService> logger)
        {
            _logger = logger;
        }

        public override Task<User> SendUser(User user, Grpc.Core.ServerCallContext context)
        {
            return Task.FromResult(new User
            {
                UserName = user.UserName

            });
        }
    }
}