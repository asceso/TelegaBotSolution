namespace BindedSources.JsonCore.Models
{
    public class LocalizationModel
    {
        public LocalizationStructure Current { get; set; }
        public LocalizationStructure Ru { get; set; }
        public LocalizationStructure En { get; set; }
        public class LocalizationStructure
        {
            public string BotStartingConsole { get; set; }
            public string BotStoppedConsole { get; set; }
            public string StartCommandMsg { get; set; }
            public string WeatherBeginedMsg { get; set; }
            public string HelloCommandMsg { get; set; }
            public string WeatherCommandMsg { get; set; }
            public string WeatherStopCommandAnswer { get; set; }
            public string WeatherStopCommandMsg { get; set; }
            public string HttpRequestErrorMsg { get; set; }
            public string WeatherResultFindedMsg { get; set; }
            public string WeatherResultStart { get; set; }
            public string WeatherResultGradus { get; set; }
            public string WeatherResultFarengeit { get; set; }
            public string WeatherResultKMHour { get; set; }
            public string WeatherResultMeterSecond { get; set; }
            public string WeatherResultPlace { get; set; }
            public string WeatherResultOnStreet { get; set; }
            public string WeatherResultTemperature { get; set; }
            public string WeatherResultFeelsLike { get; set; }
            public string WeatherResultWindDescription { get; set; }
            public string WeatherErrorResponseMsg { get; set; }
            public string OtherStandartMessage { get; set; }
        }
    }
}
