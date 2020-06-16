using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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
        static volatile private List<Person> _prospectiveClients;

        private readonly ILogger<ProspectiveClientsController> _logger;

        public ProspectiveClientsController(ILogger<ProspectiveClientsController> logger)
        {
            if (_prospectiveClients == null)
                _prospectiveClients = new List<Person>();

            _logger = logger;
        }

        [HttpPost("[action]")]
        public IActionResult LoadData([FromForm] IFormCollection _)
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
                return StatusCode(400, JObject.Parse(@"{ 'Response': 'Invalid JSON file provided' }").ToString());
            }
        }

        [HttpGet("[action]/{topClients:int}")]
        public IActionResult TopClients(int topClients)
        {
            try
            {
                if (_prospectiveClients.Count == 0)
                    return StatusCode(404, JObject.Parse(@"{ 'Response': 'No clients present' }").ToString());

                return Ok(_prospectiveClients.OrderByDescending(_ => _.Priority).Take(topClients).ToArray());
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError("Error obtaining top clients.", ex);
                return StatusCode(500, JObject.Parse(@"{ 'Response': 'Invalid number of top clients provided' }").ToString());
            }
        }

        [HttpGet("[action]/{id:int}")]
        public IActionResult ClientPosition(int id)
        {
            try
            {
                int index = _prospectiveClients.FindIndex(_ => _.Id == id);

                if (index < 0)
                    return StatusCode(404, JObject.Parse(@"{ 'Response': 'Client not found.' }").ToString());

                string response = $@"{{
                    'Position':'{index + 1}'
                }}";

                return Ok(value: JObject.Parse(response).ToString());
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError("Error obtaining client.", ex);
                return StatusCode(500, JObject.Parse(@"{ 'Response': 'Invalid JSON provided' }").ToString());
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
                return StatusCode(400, JObject.Parse(@"{ 'Response': 'Invalid client's JSON provided' }").ToString());
            }
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_prospectiveClients?.ToArray());
        }
    }
}
