using System.IO;
using System.Net;
using BindedSources;
using BindedSources.JsonCore.Models;
using BindedSources.MemoryCacheCore;
using Ninject;

namespace TelegramBotData.CommandsCore
{
    public static class WebApiCore
    {
        /// <summary>
        /// Кеш памяти
        /// </summary>
        private static IStoreData mcache;
        /// <summary>
        /// Метод получения кеша из биндинга
        /// </summary>
        /// <param name="kernel">Ядро нинжекта</param>
        public static void GetStoreDataFromKernel(IKernel kernel) => mcache = kernel.Get<IStoreData>();

        /// <summary>
        /// Выполнить Get Http запрос
        /// </summary>
        /// <param name="request_string">запрос</param>
        /// <returns>Ответ от апи сервера</returns>
        public static string ExecuteHttpRequest(string request_string)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(request_string);
            request.Method = WebRequestMethods.Http.Get;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using StreamReader stream = new StreamReader(response.GetResponseStream());
                return stream.ReadToEnd();
            }
            else
            {
                return mcache.GetData<LocalizationModel>(ConstantStrings.Localization).Current.HttpRequestErrorMsg;
            }
        }
    }
}
