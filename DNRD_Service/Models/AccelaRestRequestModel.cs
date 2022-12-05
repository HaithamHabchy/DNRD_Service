namespace DNRD_Service.Models
{
    public class AccelaRestRequestModel
    {
        public Header header { get; set; } = new Header();
        public dynamic body { get; set; }
    }

    public class Header
    {
        public string action { get; set; }
        public string lang { get; set; }
    }
}
