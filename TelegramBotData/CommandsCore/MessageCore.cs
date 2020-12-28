using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BindedSources;
using BindedSources.JsonCore.Models;
using BindedSources.MemoryCacheCore;
using Newtonsoft.Json;
using Ninject;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotData.BotCore;
using TelegramBotData.CommandsCore.OpenWeatherApi;
using TelegramBotData.CommandsCore.WeatherApi;
using TelegramBotData.Models.OpenWeatherModels;
using TelegramBotData.Models.WeatherApiModels;

namespace TelegramBotData.CommandsCore
{
    public static class MessageCore
    {
        #region Переменные

        /// <summary>
        /// Фразы приветствия
        /// </summary>
        private static List<string> HelloWords = new List<string>
        {
            "* говорит по Английски * Hello",
            "* говорит по Арабски * Ahlan wa sahlan",
            "* говорит по Африкански * Hola",
            "* говорит по Белорусски * Прывитанне",
            "* говорит по Гавайски * Aloha",
            "* говорит по Украински * Привіт",
            "* говорит по Русски * Привіт",
        };

        /// <summary>
        /// Кеш памяти
        /// </summary>
        private static IStoreData mcache;
        private static LocalizationModel localization;
        /// <summary>
        /// Метод получения кеша из биндинга
        /// </summary>
        /// <param name="kernel">Ядро нинжекта</param>
        public static void GetStoreDataFromKernel(IKernel kernel)
        {
            mcache = kernel.Get<IStoreData>();
            localization = mcache.GetData<LocalizationModel>(ConstantStrings.Localization);
        }

        #endregion

        #region Ответы пользователям

        /// <summary>
        /// Обычный ответ
        /// </summary>
        /// <param name="msg">Сообщение</param>
        /// <param name="args">Аргументы</param>
        /// <param name="chat">Чат ИД</param>
        private static void StandartSending(string msg, MessageEventArgs args, ChatId chat = null)
        {
            TelegramBotClient Bot = SimpleTBot.GetBot();
            if (chat is null)
            {
                Bot.SendTextMessageAsync(args.Message.Chat.Id, msg);
            }
            else
            {
                Bot.SendTextMessageAsync(chat, msg);
            }
        }

        /// <summary>
        /// Ответ с клавиатурой
        /// </summary>
        /// <param name="msg">Сообщение</param>
        /// <param name="keyboard">Клавиатура</param>
        /// <param name="args">Аргументы</param>
        /// <param name="chat">Чат ИД</param>
        private static void WithKeyboardSending(string msg, ReplyKeyboardMarkup keyboard, MessageEventArgs args = null, ChatId chat = null)
        {
            TelegramBotClient Bot = SimpleTBot.GetBot();
            if (chat is null)
            {
                Bot.SendTextMessageAsync(args.Message.Chat.Id, msg, replyMarkup: keyboard);
            }
            else
            {
                Bot.SendTextMessageAsync(chat, msg, replyMarkup: keyboard);
            }
        }

        /// <summary>
        /// Ответ с вариантами
        /// </summary>
        /// <param name="msg">Сообщение</param>
        /// <param name="variants">Варианты</param>
        /// <param name="args">Аргументы</param>
        /// <param name="chat">Чат ИД</param>
        private static void WithVariantsSending(string msg, InlineKeyboardMarkup variants, MessageEventArgs args, ChatId chat = null)
        {
            TelegramBotClient Bot = SimpleTBot.GetBot();
            if (chat is null)
            {
                Bot.SendTextMessageAsync(args.Message.Chat.Id, msg, replyMarkup: variants);
            }
            else
            {
                Bot.SendTextMessageAsync(chat, msg, replyMarkup: variants);
            }
        }

        #endregion

        #region Обработка команд

        /// <summary>
        /// Обработка сообщения /start
        /// </summary>
        /// <param name="args">Аргументы</param>
        /// <param name="welcomingMessage">Приветственное сообщение</param>
        /// <param name="chat">Чат ИД</param>
        public static void StartMessage(MessageEventArgs args, string welcomingMessage, ChatId chat = null)
        {
            var keyboard = KeyboardCore.ConfigureStandartKeyboard();
            WithKeyboardSending(welcomingMessage, keyboard, args);
        }

        /// <summary>
        /// Обработка сообщения "Поздороваться"
        /// </summary>
        /// <param name="args">Аргументы</param>
        /// <param name="message">Ответ</param>
        public static void SayHelloMessage(MessageEventArgs args, string message = null)
        {
            Random rnd = new Random();
            if (message is null)
            {
                message = $"{HelloWords[rnd.Next(0, HelloWords.Count)]} {args.Message.From.FirstName} {args.Message.From.LastName}";
            }
            StandartSending(message, args);
        }

        /// <summary>
        /// Обработка сообщения "Погода"
        /// </summary>
        /// <param name="args">Аргументы</param>
        /// <param name="IsWeatherBegin">Ведется ли проверка погоды сейчас</param>
        /// <param name="chat">Чат ИД</param>
        public static bool WeatherMessage(MessageEventArgs args, bool IsWeatherBegin, ChatId chat = null, string UsingApiMethod = null)
        {
            TelegramBotClient Bot = SimpleTBot.GetBot();
            if (!IsWeatherBegin)
            {
                var keyboard = KeyboardCore.ConfigureWeatherKeyboard();

                WithKeyboardSending(localization.Current.WeatherBeginedMsg, keyboard, args);
                return true;
            }
            else
            {
                if (args.Message.Text == localization.Current.WeatherStopCommandMsg)
                {
                    var keyboard = KeyboardCore.ConfigureStandartKeyboard();

                    WithKeyboardSending(localization.Current.WeatherStopCommandAnswer, keyboard, args);
                    return false;
                }
                else
                {
                    var keyboard = KeyboardCore.ConfigureStandartKeyboard();
                    if (args.Message.Type == MessageType.Text)
                    {
                        if (UsingApiMethod == ConstantStrings.WeatherToken)
                        {
                            string result = WebApiCore.ExecuteHttpRequest(WeatherCore.CreateApiStringWithCity(args.Message.Text));
                            List<FindedAddressModel> models = JsonConvert.DeserializeObject<List<FindedAddressModel>>(result);

                            WithVariantsSending(localization.Current.WeatherResultFindedMsg,
                                KeyboardCore.ConfigureWeatherKeyboardWithVariants(models), args);
                            return false;
                        }
                        if (UsingApiMethod == ConstantStrings.OpenWeatherMapToken)
                        {
                            try
                            {
                                string result = WebApiCore.ExecuteHttpRequest(OpenWeatherCore.CreateApiStringWithCity(args.Message.Text));
                                GeoOpenWeatherModel model = JsonConvert.DeserializeObject<GeoOpenWeatherModel>(result);

                                WithKeyboardSending(CreateOpenWeatherAnswer(model), keyboard, args);
                            }
                            catch (Exception)
                            {
                                SendBadWeatherResponse(chat ?? args.Message.Chat.Id);
                            }
                        }
                    }
                    else if (args.Message.Type == MessageType.Location || args.Message.Type == MessageType.Venue)
                    {
                        if (UsingApiMethod == ConstantStrings.WeatherToken)
                        {
                            SendWeatherAnswer(new WeatherLocationModel()
                            {
                                lat = args.Message.Location.Latitude,
                                lon = args.Message.Location.Longitude
                            }, args.Message.Chat.Id);
                        }
                        if (UsingApiMethod == ConstantStrings.OpenWeatherMapToken)
                        {
                            try
                            {
                                string result = WebApiCore.ExecuteHttpRequest(OpenWeatherCore.CreateApiStringWithLocation(args.Message.Location));
                                GeoOpenWeatherModel model = JsonConvert.DeserializeObject<GeoOpenWeatherModel>(result);

                                WithKeyboardSending(CreateOpenWeatherAnswer(model), keyboard, args);
                            }
                            catch (Exception)
                            {
                                SendBadWeatherResponse(chat ?? args.Message.Chat.Id);
                            }
                        }
                    }
                    return false;
                }
            }
        }

        /// <summary>
        /// Обработка других сообщений
        /// </summary>
        /// <param name="args">Аргументы</param>
        /// <param name="otherMessage">Ответ</param>
        public static void OtherMessage(MessageEventArgs args, string otherMessage = null)
        {
            if (otherMessage is null)
            {
                otherMessage = localization.Current.OtherStandartMessage;
            }
            StandartSending(otherMessage, args);
        }

        #endregion

        #region create weather response strings

        /// <summary>
        /// Отправить ответ погоды
        /// </summary>
        /// <param name="coords">Координаты</param>
        /// <param name="chat">Чат ИД</param>
        /// <returns>True False</returns>
        public static bool SendWeatherAnswer(WeatherLocationModel coords, ChatId chat = null)
        {
            var keyboard = KeyboardCore.ConfigureStandartKeyboard();
            string result = WebApiCore.ExecuteHttpRequest(WeatherCore.CreateApiStringWithLocation(new Location()
            {
                Latitude = coords.lat,
                Longitude = coords.lon
            }));
            GeoWeatherModel model = JsonConvert.DeserializeObject<GeoWeatherModel>(result);

            if (model is null)
            {
                SendBadWeatherResponse(chat);
            }
            WithKeyboardSending(CreateWeatherAnswer(model), keyboard, chat: chat);
            return false;
        }
        private static string CreateWeatherAnswer(GeoWeatherModel model)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(localization.Current.WeatherResultStart);
            sb.AppendLine($"{localization.Current.WeatherResultPlace}: {model.location.region}");
            sb.AppendLine($"{localization.Current.WeatherResultOnStreet}: {model.current.condition.text}");
            sb.AppendLine($"{localization.Current.WeatherResultTemperature}: " +
                          $"{model.current.temp_c} {localization.Current.WeatherResultGradus}, " +
                          $"{model.current.temp_f} {localization.Current.WeatherResultFarengeit}");
            sb.AppendLine($"{localization.Current.WeatherResultFeelsLike}: " +
                          $"{model.current.feelslike_c} {localization.Current.WeatherResultGradus}, " +
                          $"{model.current.feelslike_f} {localization.Current.WeatherResultFarengeit}");
            sb.AppendLine($"{localization.Current.WeatherResultWindDescription}: " +
                          $"{model.current.wind_kph} {localization.Current.WeatherResultKMHour}, " +
                          $"{model.current.wind_mph} {localization.Current.WeatherResultMeterSecond}");
            return sb.ToString();
        }
        private static string CreateOpenWeatherAnswer(GeoOpenWeatherModel model)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(localization.Current.WeatherResultStart);
            sb.AppendLine($"{localization.Current.WeatherResultPlace}: {model.name}");
            sb.AppendLine($"{localization.Current.WeatherResultOnStreet}: {model.weather.FirstOrDefault().description}");
            sb.AppendLine($"{localization.Current.WeatherResultTemperature}: " +
                          $"{model.main.temp} {localization.Current.WeatherResultGradus}, " +
                          $"{(model.main.temp * 1.8f) + 32} {localization.Current.WeatherResultFarengeit}");
            sb.AppendLine($"{localization.Current.WeatherResultFeelsLike}: " +
                          $"{model.main.feels_like} {localization.Current.WeatherResultGradus}, " +
                          $"{(model.main.feels_like * 1.8f) + 32} {localization.Current.WeatherResultFarengeit}");
            sb.AppendLine($"{localization.Current.WeatherResultWindDescription}: " +
                          $"{model.wind.speed} {localization.Current.WeatherResultKMHour}, " +
                          $"{model.wind.speed * 3.6f} {localization.Current.WeatherResultMeterSecond}");
            return sb.ToString();
        }
        private static void SendBadWeatherResponse(ChatId id)
        {
            var keyboard = KeyboardCore.ConfigureStandartKeyboard();
            WithKeyboardSending(localization.Current.WeatherErrorResponseMsg, keyboard, chat: id);
        }

        #endregion
    }
}
