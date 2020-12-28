using System.Collections.Generic;

namespace TelegramBotData.Extensions
{
    public static class DictionaryExtension
    {
        /// <summary>
        /// Добавить в словарь с проверкой на наличие
        /// </summary>
        /// <param name="dic">Словарь</param>
        /// <param name="key">Ключ</param>
        /// <param name="value">Значение</param>
        public static void AddWithKey(this Dictionary<int, bool> dic, int key, bool value)
        {
            if (dic.ContainsKey(key))
            {
                dic[key] = value;
            }
            else
            {
                dic.Add(key, value);
            }
        }

        /// <summary>
        /// Вернуть значение из словаря если имеется
        /// </summary>
        /// <param name="dic">Словарь</param>
        /// <param name="key">Ключ</param>
        /// <returns>true если имеется и равно true</returns>
        public static bool GetIfContain(this Dictionary<int, bool> dic, int key)
        {
            if (dic.ContainsKey(key))
            {
                return dic[key];
            }
            else
            {
                return false;
            }
        }
    }
}
