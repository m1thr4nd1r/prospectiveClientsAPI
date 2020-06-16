using System;

namespace prospectiveClientsAPI.Models
{
    public class Person
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string CurrentRole { get; set; }

        public string Country { get; set; }

        public string Industry { get; set; }

        public int NumberOfRecommendations { get; set; }

        public int NumberOfConnections { get; set; }
}
}
