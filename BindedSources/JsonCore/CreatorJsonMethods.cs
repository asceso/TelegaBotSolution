using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace BindedSources.JsonCore
{
    public class CreatorJsonMethods : ICreatorJson
    {
        private static string JsonPath;
        public void SetJsonPath(string path) => JsonPath = path;

        public JType ReadConfig<JType>() where JType : new()
        {
            string buffer = string.Empty;
            using (StreamReader reader = new StreamReader(JsonPath, Encoding.UTF8))
            {
                buffer = reader.ReadToEnd();
            }
            if (buffer.Equals(string.Empty))
            {
                return new JType();
            }
            JType jFile = JsonConvert.DeserializeObject<JType>(buffer);
            return jFile;
        }
        public bool SetConfig<JType>(JType model) where JType : new()
        {
            try
            {
                string json = JsonConvert.SerializeObject(model, Formatting.Indented);
                using StreamWriter writer = new StreamWriter(new FileStream(JsonPath, FileMode.OpenOrCreate), Encoding.UTF8);
                writer.Write(json);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
