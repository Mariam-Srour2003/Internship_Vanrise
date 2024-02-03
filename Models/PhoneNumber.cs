namespace MariamProject.Models
{
    public class PhoneNumber
    {
        public int PhoneNumberId { get; set; }
        public string Number { get; set; }
        public int DeviceId { get; set; }

        public string DeviceName { get; set; } 
    }
}
