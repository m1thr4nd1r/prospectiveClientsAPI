﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        static volatile private List<Person> prospectiveClients;

        private readonly ILogger<ProspectiveClientsController> _logger;

        public ProspectiveClientsController(ILogger<ProspectiveClientsController> logger)
        {
            _logger = logger;
        }

        [HttpPost("loadData")]
        public IActionResult Post([FromBody] object value)
        {
            try
            {
                prospectiveClients = JsonConvert.DeserializeObject<List<Person>>(value.ToString());
                return Ok();
            }
            catch (JsonException ex)
            {
                _logger.LogError("Error deserializing persons' JSON.", ex);
                return StatusCode(400);
            }
        }

        [HttpGet("[action]/{topClients:int}")]
        public IActionResult TopClients(int topClients)
        {
            try
            {
                return Ok(prospectiveClients.OrderByDescending(_ => _.Priority).Take(topClients).ToArray());
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
                int index = prospectiveClients.FindIndex(_ => _.Id == id);
                string response = $"Position: {index + 1}";

                return Ok(value: JsonConvert.SerializeObject(response, Formatting.Indented));
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError("Error obtaining client.", ex);
                return StatusCode(500);
            }
        }

        [HttpGet]
        public IEnumerable<Person> Get()
        {
            return prospectiveClients?.ToArray();
        }
    }
}
