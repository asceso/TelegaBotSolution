using System.Net;
using System.Net.Http;
using Telegram.Bot;

namespace TelegramBotData.BotCore
{
    public static class SimpleTBot
    {
        /// <summary>
        /// Клиент телеги
        /// </summary>
        private static TelegramBotClient client;

        /// <summary>
        /// Настроить бота
        /// </summary>
        /// <param name="botToken">Токен телеги</param>
        /// <returns>Клиент бота</returns>
        public static TelegramBotClient ConfigureBot(string botToken) => client = new TelegramBotClient(botToken);

        /// <summary>
        /// Настроить бота с Http прокси
        /// </summary>
        /// <param name="botToken">Токен телеги</param>
        /// <param name="http">Http прокси</param>
        /// <returns>Клиент бота</returns>
        public static TelegramBotClient ConfigureBot(string botToken, HttpClient http) => client = new TelegramBotClient(botToken, http);

        /// <summary>
        /// Настроить бота с Web прокси
        /// </summary>
        /// <param name="botToken">Токен телеги</param>
        /// <param name="proxy">Web прокси</param>
        /// <returns>Клиент бота</returns>
        public static TelegramBotClient ConfigureBot(string botToken, IWebProxy proxy) => client = new TelegramBotClient(botToken, proxy);

        /// <summary>
        /// Получить настроенного бота
        /// </summary>
        /// <returns>Клиент телеги</returns>
        public static TelegramBotClient GetBot() => client;
    }
}
