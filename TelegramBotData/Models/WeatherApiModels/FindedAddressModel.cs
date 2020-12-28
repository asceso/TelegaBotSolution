namespace TelegramBotData.Models.WeatherApiModels
{
    public class FindedAddressModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string region { get; set; }
        public string country { get; set; }
        public float lat { get; set; }
        public float lon { get; set; }
        public string url { get; set; }
    }
}
