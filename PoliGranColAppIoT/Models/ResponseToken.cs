namespace PoliGranColAppIoT.Models
{

    public class ResponseToken
    {
        public string token { get; set; }
        public string refreshToken { get; set; }
        public object scope { get; set; }
    }
}