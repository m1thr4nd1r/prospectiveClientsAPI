using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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
        static volatile private List<Person> _prospectiveClients;

        private readonly ILogger<ProspectiveClientsController> _logger;

        public ProspectiveClientsController(ILogger<ProspectiveClientsController> logger)
        {
            if (_prospectiveClients == null)
                _prospectiveClients = new List<Person>();

            _logger = logger;
        }

        [HttpPost("[action]")]
        public IActionResult LoadData([FromForm] IFormCollection values)
        {
            var file = HttpContext.Request.Form.Files?[0];

            byte[] bytes = new byte[file.Length];
            var stream = file.OpenReadStream();
            stream.Read(bytes, 0, bytes.Length);
            var json = Encoding.UTF8.GetString(bytes);

            try
            {
                _prospectiveClients = JsonConvert.DeserializeObject<List<Person>>(json);
                return Ok();
            }
            catch (JsonReaderException ex)
            {
                _logger.LogWarning("Invalid input format.", ex.StackTrace);
                //var jObj = JObject.Parse(JsonConvert.SerializeObject("Formato de entrada invalida."));
                return StatusCode(400);
            }
        }

        [HttpGet("[action]/{topClients:int}")]
        public IActionResult TopClients(int topClients)
        {
            try
            {
                return Ok(_prospectiveClients.OrderByDescending(_ => _.Priority).Take(topClients).ToArray());
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError("Error obtaining top clients.", ex);
                return StatusCode(500);
            }
        }

        [HttpGet("[action]/{id:int}")]
        public IActionResult ClientPosition(int id)
        {
            try
            {
                int index = _prospectiveClients.FindIndex(_ => _.Id == id);
                string response = $"Position: {index + 1}";

                return Ok(value: JsonConvert.SerializeObject(response, Formatting.Indented));
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError("Error obtaining client.", ex);
                return StatusCode(500);
            }
        }

        [HttpPost("[action]")]
        public IActionResult InsertPerson([FromBody] object value)
        {
            try
            {
                _prospectiveClients.Add(JsonConvert.DeserializeObject<Person>(value.ToString()));
                return Ok();
            }
            catch (JsonException ex)
            {
                _logger.LogError("Error deserializing person's JSON.", ex);
                return StatusCode(400);
            }
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_prospectiveClients?.ToArray());
        }
    }
}
