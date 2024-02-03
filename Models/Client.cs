using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MariamProject.Models
{
    public class Client
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public ClientType ClientType { get; set; }
        public DateTime? ClientBirthday { get; set; }
        public List<int> ReservedPhoneNumbersId { get; set; }
        public int ZoneId { get; set; }
        public int SiteId { get; set; }
        public string ZoneName { get; set; }
    }

    public enum ClientType
    {
        Individual = 1,
        Organization = 2
    }
}
