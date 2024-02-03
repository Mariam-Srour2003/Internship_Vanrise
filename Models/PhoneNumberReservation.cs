using System;
namespace MariamProject.Models
{
    public class PhoneNumberReservation
    {
        public int ReservationID { get; set; }
        public int ClientID { get; set; }
        public int PhoneNumberID { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }

        public string ClientName { get; set; }

        public string Number { get; set; }
    }
}