using System.Configuration;
using Telegram.Bot.Types;

namespace TelegramBotData.CommandsCore.OpenWeatherApi
{
    public static class OpenWeatherCore
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
            => $"http://api.openweathermap.org/data/2.5/weather?lat={location.Latitude}&lon={location.Longitude}&appid={api_string}&units=metric&lang={ConfigurationManager.AppSettings[Localization]}";

        /// <summary>
        /// Создать апи запрос с городом
        /// </summary>
        /// <param name="city">Город</param>
        /// <returns>строка апи запроса</returns>
        public static string CreateApiStringWithCity(string city)
            => $"http://api.openweathermap.org/data/2.5/weather?q={city}&appid={api_string}&units=metric&lang={ConfigurationManager.AppSettings[Localization]}";
    }
}
