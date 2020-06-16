using Newtonsoft.Json;
using System;

namespace prospectiveClientsAPI.Models
{
    public class Person
    {
        [JsonProperty("PersonId")]
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string CurrentRole { get; set; }

        public string Country { get; set; }

        public string Industry { get; set; }

        [JsonProperty("NumberOfRecommendations")]
        public int NumberOfRecommendations { get; set; }

        [JsonProperty("NumberOfConnections")]
        public int NumberOfConnections { get; set; }

        public int Priority => NumberOfConnections * NumberOfRecommendations;
    }
}
