using System;
using System.Collections.Generic;
using System.Configuration;
using BindedSources;
using BindedSources.JsonCore;
using BindedSources.JsonCore.Models;
using BindedSources.MemoryCacheCore;
using Newtonsoft.Json;
using Ninject;
using Telegram.Bot;
using Telegram.Bot.Args;
using TelegramBotData.BotCore;
using TelegramBotData.CommandsCore;
using TelegramBotData.CommandsCore.OpenWeatherApi;
using TelegramBotData.CommandsCore.WeatherApi;
using TelegramBotData.Extensions;
using TelegramBotData.Models.WeatherApiModels;

namespace TBot
{
    class Program
    {
        #region Переменные

        private static ICreatorJson jCreator;
        private static LocalizationModel localization;

        /// <summary>
        /// Проверяется ли погода
        /// </summary>
        private static Dictionary<int, bool> IsWeatherBegin = new Dictionary<int, bool>();

        /// <summary>
        /// Клиент телеги
        /// </summary>
        private static TelegramBotClient Bot;

        #endregion
        #region Конструктор

        public static void Main()
        {
            #region Конфигурация
            StandardKernel kernel = new StandardKernel();
            kernel.Bind<IStoreData>().To<StoreDataMethods>();
            kernel.Bind<ICreatorJson>().To<CreatorJsonMethods>();
            Bot = SimpleTBot.ConfigureBot(ConfigurationManager.AppSettings[ConstantStrings.TelegaToken]);
            WeatherCore.SetWeatherApi(ConfigurationManager.AppSettings[ConstantStrings.WeatherToken]);
            OpenWeatherCore.SetWeatherApi(ConfigurationManager.AppSettings[ConstantStrings.OpenWeatherMapToken]);
            jCreator = kernel.Get<ICreatorJson>();
            jCreator.SetJsonPath(Environment.CurrentDirectory + "//LocalizationStrings.json");
            localization = jCreator.ReadConfig<LocalizationModel>();
            localization.Current = ConfigurationManager.AppSettings[ConstantStrings.Localization] == "ru" ? localization.Ru : localization.En;
            kernel.Get<IStoreData>().StoreData(localization, ConstantStrings.Localization);
            MessageCore.GetStoreDataFromKernel(kernel);
            WebApiCore.GetStoreDataFromKernel(kernel);
            KeyboardCore.GetStoreDataFromKernel(kernel);
            #endregion
            #region Запуск бота
            Bot.OnMessage += BotOnMessage;
            Bot.OnCallbackQuery += BotOnCallbackQuery;
            Bot.StartReceiving();
            Console.WriteLine(localization.Current.BotStartingConsole);
            #endregion
            #region Остановка бота
            Console.ReadLine();
            Bot.StopReceiving();
            Console.WriteLine(localization.Current.BotStoppedConsole);
            Console.ReadLine();
            #endregion
        }

        #endregion
        #region События бота

        /// <summary>
        /// Колбек нажатия кнопок
        /// </summary>
        /// <param name="sender">отправитель</param>
        /// <param name="e">аргументы</param>
        private static void BotOnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            if (e.CallbackQuery.Data.Contains(nameof(WeatherLocationModel)))
            {
                var modelData = e.CallbackQuery.Data.Replace($"{nameof(WeatherLocationModel)} is :", string.Empty);
                WeatherLocationModel locationModel = JsonConvert.DeserializeObject<WeatherLocationModel>(modelData);
                MessageCore.SendWeatherAnswer(locationModel, e.CallbackQuery.Message.Chat.Id);
            }
        }

        /// <summary>
        /// Проверка сообщений
        /// </summary>
        /// <param name="sender">Отправитель</param>
        /// <param name="args">Аргументы</param>
        private static void BotOnMessage(object sender, MessageEventArgs args)
        {
            int fromId = args.Message.From.Id;
            #region start checking
            if (args.Message.Text == "/start")
            {
                MessageCore.StartMessage(args, localization.Current.StartCommandMsg);
                return;
            }
            #endregion
            #region say hello checking
            else if (args.Message.Text == localization.Current.HelloCommandMsg)
            {
                MessageCore.SayHelloMessage(args);
                return;
            }
            #endregion
            #region weather checking
            else if (args.Message.Text == localization.Current.WeatherCommandMsg ||
                     args.Message.Text == localization.Current.WeatherStopCommandMsg ||
                     IsWeatherBegin.GetIfContain(fromId))
            {
                IsWeatherBegin.AddWithKey(fromId, MessageCore.WeatherMessage(args, IsWeatherBegin.GetIfContain(fromId),
                    UsingApiMethod: ConfigurationManager.AppSettings[ConstantStrings.UseWeatherApi]));
                return;
            }
            #endregion
            #region other checking
            else
            {
                MessageCore.OtherMessage(args);
                return;
            }
            #endregion
        }

        #endregion
    }
}
