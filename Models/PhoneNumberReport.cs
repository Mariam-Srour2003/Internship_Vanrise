using System.Collections.Generic;
namespace MariamProject.Models
{
    public class PhoneNumberReport
    {
        public int Device { get; set; }
        public string DeviceName { get; set; }
        public int NumberOfPhoneNumbersReserved { get; set; }
        public int NumberOfPhoneNumbersUnReserved { get; set; }
    }
}