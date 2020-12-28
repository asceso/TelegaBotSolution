using System.Collections.Generic;
using BindedSources;
using BindedSources.JsonCore.Models;
using BindedSources.MemoryCacheCore;
using Newtonsoft.Json;
using Ninject;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotData.Models.WeatherApiModels;

namespace TelegramBotData.CommandsCore
{
    public static class KeyboardCore
    {
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

        /// <summary>
        /// Настроить стандартную клавиатуру
        /// </summary>
        /// <returns>Стандартная клавиатура</returns>
        public static ReplyKeyboardMarkup ConfigureStandartKeyboard()
        {
            return new ReplyKeyboardMarkup()
            {
                Keyboard = new[]
                {
                    new[]
                    {
                        new KeyboardButton(localization.Current.HelloCommandMsg),
                        new KeyboardButton(localization.Current.WeatherCommandMsg),
                    }
                },
                ResizeKeyboard = true,
                OneTimeKeyboard = false
            };
        }

        /// <summary>
        /// Настроить клавиатуру с отменой погоды
        /// </summary>
        /// <returns>Клавиатура с кнопкой отмены погоды</returns>
        public static ReplyKeyboardMarkup ConfigureWeatherKeyboard()
        {
            return new ReplyKeyboardMarkup()
            {
                Keyboard = new[]
                {
                    new[]
                    {
                        new KeyboardButton(localization.Current.WeatherStopCommandMsg),
                    }
                },
                ResizeKeyboard = true,
                OneTimeKeyboard = false
            };
        }

        /// <summary>
        /// Настроить варианты погоды
        /// </summary>
        /// <param name="models">Модели найденных адрессов</param>
        /// <returns></returns>
        public static InlineKeyboardMarkup ConfigureWeatherKeyboardWithVariants(List<FindedAddressModel> models)
        {
            List<List<InlineKeyboardButton>> markup = new List<List<InlineKeyboardButton>>();
            foreach (FindedAddressModel model in models)
            {
                WeatherLocationModel weatherLocationModel = new WeatherLocationModel()
                {
                    lat = model.lat,
                    lon = model.lon
                };
                markup.Add(new List<InlineKeyboardButton>() {
                    InlineKeyboardButton.WithCallbackData($"{model.region}, {model.name}",
                    $"{nameof(WeatherLocationModel)} is :{JsonConvert.SerializeObject(weatherLocationModel)}")
                });
            }
            return new InlineKeyboardMarkup(markup);
        }
    }
}
