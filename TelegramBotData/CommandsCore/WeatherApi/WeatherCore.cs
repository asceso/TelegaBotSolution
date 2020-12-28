using System.Configuration;
using Telegram.Bot.Types;

namespace TelegramBotData.CommandsCore.WeatherApi
{
    public static class WeatherCore
    {
        private const string Localization = nameof(Localization);

        /// <summary>
        /// апи токен
        /// </summary>
        private static string api_string;

        /// <summary>
        /// установить апи токен
        /// </summary>
        /// <param name="api">токен</param>
        public static void SetWeatherApi(string api) => api_string = api;

        /// <summary>
        /// Создать апи запрос с геометкой
        /// </summary>
        /// <param name="location">Гео метка</param>
        /// <returns>строка апи запроса</returns>
        public static string CreateApiStringWithLocation(Location location)
            => $"http://api.weatherapi.com/v1/current.json?key={api_string}&q={location.Latitude},{location.Longitude}&lang={ConfigurationManager.AppSettings[Localization]}";

        /// <summary>
        /// Создать апи запрос с городом
        /// </summary>
        /// <param name="city">Город</param>
        /// <returns>строка апи запроса</returns>
        public static string CreateApiStringWithCity(string city)
            => $"http://api.weatherapi.com/v1/search.json?key={api_string}&q={city}&lang={ConfigurationManager.AppSettings[Localization]}";
    }
}
