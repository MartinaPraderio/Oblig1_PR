using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LoggServer.Interfaces;
using LoggAPI.Models;

namespace LogsServer.Controllers
{
    [ApiController]
    [Route("logs")]
    [Consumes("application/json")]
    public class LoggController : Controller
    {
        private readonly ILoggServices _loggServices;

        public LoggController(ILoggServices loggServices)
        {
            this._loggServices = loggServices;
        }

        [HttpGet()]
        public IActionResult GetAll([FromQuery] string user, [FromQuery] string game, [FromQuery] string date)
        {
            var logs = this._loggServices.GetAll(user, game, date);
            return Ok(LoggModel.ToDto(logs));
        }
    }
}