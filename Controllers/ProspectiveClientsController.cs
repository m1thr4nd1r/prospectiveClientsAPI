using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using prospectiveClientsAPI.Models;

namespace prospectiveClientsAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProspectiveClientsController : ControllerBase
    {
        private List<Person> prospectiveClients;

        private readonly ILogger<ProspectiveClientsController> _logger;

        public ProspectiveClientsController(ILogger<ProspectiveClientsController> logger)
        {
            _logger = logger;
            prospectiveClients = new List<Person>();
        }

        [HttpGet]
        public IEnumerable<Person> Get()
        {
            return prospectiveClients?.ToArray();
        }
    }
}
