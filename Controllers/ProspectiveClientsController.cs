using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        [HttpPost]
        public string Post([FromForm] string people)
        {
            JObject data = null;

            try
            {
                data = JObject.Parse(people.ToString());
                return data.ToString();
            }
            catch (JsonReaderException ex)
            {
                Console.WriteLine(ex.StackTrace);
                var jObj = JObject.Parse(JsonConvert.SerializeObject("Formato de entrada invalida."));
                return jObj.ToString(Formatting.Indented);
            }
        }
    }
}
